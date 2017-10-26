using System.Net.Http;
using System.Threading.Tasks;
using SAHB.GraphQLClient.Result;

namespace SAHB.GraphQLClient.Executor
{
    // ReSharper disable once InconsistentNaming
    public interface IGraphQLHttpExecutor
    {
        Task<GraphQLDataResult<T>> ExecuteQuery<T>(string query, string url, HttpMethod method, string authorizationToken = null, string authorizationMethod = "Bearer") where T : class;
    }
}