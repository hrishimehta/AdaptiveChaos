using AdaptiveChaos.Infrastructure.Interfaces;
using AdaptiveChaos.Shared.Domain;
using Microsoft.Extensions.Configuration;
using Polly;
using Polly.Contrib.Simmy;
using Polly.Contrib.Simmy.Latency;
using Polly.Contrib.Simmy.Outcomes;
using System.Net;

namespace AdaptiveChaos.Infrastructure.Implementation
{

    public class ChaosPolicyFactory : IChaosPolicyFactory
    {
        private readonly IConfiguration _configuration;

        private static Dictionary<string, IAsyncPolicy<HttpResponseMessage>> urlToChaosSetting = new Dictionary<string, IAsyncPolicy<HttpResponseMessage>>();

        public ChaosPolicyFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void SaveChaosPolicy(string uri, OperationChaosSetting chaosPolicySettings)
        {
            // todo save and retrieve to do from DB or runtime via API
            if (urlToChaosSetting.ContainsKey(uri))
            {
                urlToChaosSetting[uri] = this.CreateChaosPolicy(chaosPolicySettings);
            }
            else
            {
                urlToChaosSetting.Add(uri, this.CreateChaosPolicy(chaosPolicySettings));
            }
        }

        public IAsyncPolicy<HttpResponseMessage> GetChaosPolicy(string uri)
        {
            if (urlToChaosSetting.ContainsKey(uri))
            {
                return urlToChaosSetting[uri];
            }

            return Policy.NoOpAsync<HttpResponseMessage>();
        }


        public IAsyncPolicy<HttpResponseMessage> CreateChaosPolicy(OperationChaosSetting chaosPolicySettings)
        {
            if (chaosPolicySettings == null)
            {
                return Policy.NoOpAsync<HttpResponseMessage>();
            }

            var latencyPolicy = GetLatencyMonkeyPolicy(chaosPolicySettings);
            var resultPolicy = GetResultMonkeyPolicy(chaosPolicySettings);
            var exceptionPolicy = GetInjectExceptionMonkeyPolicy(chaosPolicySettings);


            return Policy.WrapAsync<HttpResponseMessage>(latencyPolicy, resultPolicy, exceptionPolicy);

            //var resultPolicy = GetResultMonkeyPolicy(chaosPolicySettings);
            //return resultPolicy;
        }



        private static AsyncInjectLatencyPolicy<HttpResponseMessage> GetLatencyMonkeyPolicy(OperationChaosSetting chaosPolicySettings)
        {
            return MonkeyPolicy.InjectLatencyAsync<HttpResponseMessage>(with =>
            {
                with.Latency(chaosPolicySettings.GetLatency())
                .InjectionRate(chaosPolicySettings.GetInjectionRate())
                .Enabled(chaosPolicySettings.GetLatencyEnabled());
            });

        }

        private static AsyncInjectOutcomePolicy<HttpResponseMessage> GetResultMonkeyPolicy(OperationChaosSetting chaosPolicySettings)
        {
            return MonkeyPolicy.InjectResultAsync<HttpResponseMessage>(with =>
                                             with.Result(chaosPolicySettings.GetHttpResponseMessage())
                                                 .InjectionRate(chaosPolicySettings.GetInjectionRate())
                                                 .Enabled(chaosPolicySettings.GetHttpResponseEnabled()));
        }

        private static AsyncInjectOutcomePolicy<HttpResponseMessage> GetInjectExceptionMonkeyPolicy(OperationChaosSetting chaosPolicySettings)
        {
            return MonkeyPolicy.InjectResultAsync<HttpResponseMessage>(with =>
                                            with.Fault(chaosPolicySettings.GetException())
                                                .InjectionRate(chaosPolicySettings.GetInjectionRate())
                                                .Enabled(chaosPolicySettings.GetExceptionEnabled()));
            //return MonkeyPolicy.InjectExceptionAsync(with =>
            //                                with.Fault(chaosPolicySettings.GetException())
            //                                    .InjectionRate(chaosPolicySettings.GetInjectionRate())
            //                                    .Enabled(chaosPolicySettings.GetEnabled()));
        }
    }
}
