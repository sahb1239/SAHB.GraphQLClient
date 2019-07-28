using SAHB.GraphQL.Client.Subscription;
using SAHB.GraphQLClient.QueryGenerator;
using System;
using System.Threading.Tasks;

namespace SAHB.GraphQLClient.Subscription
{
    /// <summary>
    /// Implements GraphQL subscriptions-transport-ws protocol
    /// https://github.com/apollographql/subscriptions-transport-ws/blob/master/PROTOCOL.md
    /// </summary>
    public interface IGraphQLSubscriptionWebSocketClient : IDisposable
    {
        Task<GraphQLSubscriptionClient> Connect(Uri url);

        Task Disconnect();

        bool IsConnected { get; }
    }
}
