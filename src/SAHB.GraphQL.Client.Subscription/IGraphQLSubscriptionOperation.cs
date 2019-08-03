using System;
using System.Threading.Tasks;

namespace SAHB.GraphQLClient.Subscription
{
    /// <summary>
    /// Used to manage a GraphQL subscription operation
    /// </summary>
    /// <typeparam name="T">The type which the is used for deserilization</typeparam>
    public interface IGraphQLSubscriptionOperation<T> where T : class
    {
        /// <summary>
        /// Event which is raised when data is recieved successfully with no errors
        /// </summary>
        event EventHandler<GraphQLDataReceivedEventArg<T>> DataRecieved;

        /// <summary>
        /// Event which is raised when data is recieved with errors
        /// </summary>
        event EventHandler<GraphQLDataReceivedEventArg<T>> ErrorRecieved;

        /// <summary>
        /// Event which is raised when operation is completed (no more messages will be recieved)
        /// </summary>
        event EventHandler Completed;

        /// <summary>
        /// Method for stopping the subscription operation
        /// </summary>
        /// <returns></returns>
        Task Stop();
    }
}
