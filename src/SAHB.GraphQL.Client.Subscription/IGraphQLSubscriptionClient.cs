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
        /// <summary>
        /// Initilizes the GraphQL subscription connection
        /// </summary>
        /// <returns></returns>
        Task Initilize();

        /// <summary>
        /// Returns true if the websocket is connected
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// Returns true if the connection is initilized
        /// </summary>
        bool IsInitilized { get; }

        /// <summary>
        /// Execute a GraphQL subscription operation using the specified type <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">The type used for generating the GraphQL subscription query</typeparam>
        /// <param name="arguments">The arguments sent to the GraphQL query</param>
        /// <returns>Returns a <see cref="IGraphQLSubscriptionOperation{T}"/></returns>
        Task<IGraphQLSubscriptionOperation<T>> ExecuteOperation<T>(params GraphQLQueryArgument[] arguments) where T : class;

        /// <summary>
        /// Fired when the subscription is disconnected
        /// </summary>
        event EventHandler Disconnected;

        /// <summary>
        /// Restart GraphQL operations which has not been completed. 
        /// This should for example be used when reconnecting the websocket after a connection failure.
        /// </summary>
        /// <returns></returns>
        Task RestartActiveGraphQLOperations();
    }
}
