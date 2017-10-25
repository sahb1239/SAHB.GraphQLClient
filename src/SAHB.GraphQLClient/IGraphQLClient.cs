using System;
using System.Net.Http;
using System.Threading.Tasks;
using SAHB.GraphQLClient.QueryBuilder;

namespace SAHB.GraphQLClient
{
    // ReSharper disable once InconsistentNaming
    public interface IGraphQLClient
    {
        [Obsolete("Use Query instead")]
        Task<T> Get<T>(string url, string authorizationToken = null, string authorizationMethod = "Bearer", params GraphQLQueryArgument[] arguments) where T : class;
        Task<T> Query<T>(string url, string authorizationToken = null, string authorizationMethod = "Bearer", params GraphQLQueryArgument[] arguments) where T : class;
        Task<T> Mutate<T>(string url, string authorizationToken = null, string authorizationMethod = "Bearer", params GraphQLQueryArgument[] arguments) where T : class;
        [Obsolete("Use Query instead")]
        Task<T> Get<T>(string url, HttpMethod httpMethod, string authorizationToken = null, string authorizationMethod = "Bearer", params GraphQLQueryArgument[] arguments) where T : class;
        Task<T> Query<T>(string url, HttpMethod httpMethod, string authorizationToken = null, string authorizationMethod = "Bearer", params GraphQLQueryArgument[] arguments) where T : class;
        Task<T> Mutate<T>(string url, HttpMethod httpMethod, string authorizationToken = null, string authorizationMethod = "Bearer", params GraphQLQueryArgument[] arguments) where T : class;
    }
}