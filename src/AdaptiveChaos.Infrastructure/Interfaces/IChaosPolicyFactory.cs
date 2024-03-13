using AdaptiveChaos.Shared.Domain;
using Polly;

namespace AdaptiveChaos.Infrastructure.Interfaces
{
    public interface IChaosPolicyFactory
    {
        void SaveChaosPolicy(string uri, OperationChaosSetting chaosPolicySettings);

        IAsyncPolicy<HttpResponseMessage> GetChaosPolicy(string uri);

        IAsyncPolicy<HttpResponseMessage> CreateChaosPolicy(OperationChaosSetting chaosPolicySettings);
    }
}
