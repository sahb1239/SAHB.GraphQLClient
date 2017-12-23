using SAHB.GraphQLClient.QueryBuilder;

namespace SAHB.GraphQLClient.Batching
{
    // ReSharper disable once InconsistentNaming
    /// <summary>
    /// 
    /// </summary>
    public interface IGraphQLBatch
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arguments"></param>
        /// <returns></returns>
        IGraphQLQuery<T> Query<T>(params GraphQLQueryArgument[] arguments) where T : class;
    }
}