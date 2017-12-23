using System.Threading.Tasks;

namespace SAHB.GraphQLClient
{
    // ReSharper disable once InconsistentNaming
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IGraphQLQuery<T>
        where T : class
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<T> Execute();
    }
}