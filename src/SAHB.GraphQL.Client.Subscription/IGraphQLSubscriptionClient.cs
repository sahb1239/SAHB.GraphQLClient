using SAHB.GraphQLClient.QueryGenerator;
using System;
using System.Threading.Tasks;

namespace SAHB.GraphQLClient.Subscription
{
    /// <summary>
    /// Implements GraphQL subscriptions-transport-ws protocol
    /// https://github.com/apollographql/subscriptions-transport-ws/blob/master/PROTOCOL.md
    /// </summary>
    public interface IGraphQLSubscriptionClient
    {
        Task Connect();

        bool IsConnected { get; }

        Task<IGraphQLSubscriptionOperation<T>> ExecuteOperation<T>(params GraphQLQueryArgument[] arguments) where T : class;
    }
}
