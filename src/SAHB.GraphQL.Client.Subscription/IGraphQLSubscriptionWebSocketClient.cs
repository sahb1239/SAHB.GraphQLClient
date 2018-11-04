using SAHB.GraphQLClient.QueryGenerator;
using System;
using System.Threading.Tasks;

namespace SAHB.GraphQL.Client.Subscription
{
    /// <summary>
    /// Implements GraphQL subscriptions-transport-ws protocol
    /// https://github.com/apollographql/subscriptions-transport-ws/blob/master/PROTOCOL.md
    /// </summary>
    public interface IGraphQLSubscriptionWebSocketClient : IDisposable
    {
        Task<bool> Connect(Uri url);

        Task Disconnect();

        bool IsConnected { get; }

        Task<IGraphQLSubscriptionOperation<T>> ExecuteOperation<T>(params GraphQLQueryArgument[] arguments) where T : class;
    }
}
