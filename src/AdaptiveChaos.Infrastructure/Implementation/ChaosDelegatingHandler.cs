using AdaptiveChaos.Infrastructure.Interfaces;
using Microsoft.Extensions.Configuration;

namespace AdaptiveChaos.Infrastructure.Implementation
{
    public class ChaosDelegatingHandler : DelegatingHandler
    {
        private readonly IConfiguration _configuration;
        private readonly IChaosPolicyFactory _chaosPolicyFactory;

        public ChaosDelegatingHandler(IConfiguration configuration, IChaosPolicyFactory chaosPolicyFactory)
        {
            _configuration = configuration;
            _chaosPolicyFactory = chaosPolicyFactory;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var requestUri = request.RequestUri.Host.ToString();
            var chaosPolicy = _chaosPolicyFactory.GetChaosPolicy(requestUri);
            return await chaosPolicy.ExecuteAsync(() => base.SendAsync(request, cancellationToken));
        }
    }

}
