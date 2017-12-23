using System.Net.Http;

namespace SAHB.GraphQLClient.Batching
{
    // ReSharper disable once InconsistentNaming
    /// <summary>
    /// 
    /// </summary>
    public interface IGraphQLBatchHttpClient
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="authorizationToken"></param>
        /// <param name="authorizationMethod"></param>
        /// <returns></returns>
        IGraphQLBatch CreateBatch(string url, string authorizationToken = null, string authorizationMethod = "Bearer");
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="httpMethod"></param>
        /// <param name="authorizationToken"></param>
        /// <param name="authorizationMethod"></param>
        /// <returns></returns>
        IGraphQLBatch CreateBatch(string url, HttpMethod httpMethod, string authorizationToken = null, string authorizationMethod = "Bearer");
    }
}