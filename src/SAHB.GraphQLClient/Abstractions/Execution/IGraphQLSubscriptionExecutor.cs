using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SAHB.GraphQLClient.Execution
{
    public interface IGraphQLSubscriptionExecutor
    {
        Task<GraphQLSubscriptionExecutorResponse> ExecuteQuery(string query, string url, string authorizationToken, string authorizationMethod, IDictionary<string, string> headers, CancellationToken cancellationToken);
    }

    public class GraphQLSubscriptionExecutorResponse
    {
    }
}
