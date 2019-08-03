using SAHB.GraphQLClient.Result;
using System;

namespace SAHB.GraphQLClient.Subscription
{
    /// <summary>
    /// EventArgs for recieved GraphQL message
    /// </summary>
    /// <typeparam name="T">Type of the recieved message</typeparam>
    public class GraphQLDataReceivedEventArg<T> : EventArgs
        where T : class
    {
        /// <summary>
        /// Initilizes a new instance of the <see cref="GraphQLDataReceivedEventArg"/>
        /// </summary>
        /// <param name="data">The received data</param>
        public GraphQLDataReceivedEventArg(GraphQLDataResult<T> data)
        {
            ReceivedData = data;
        }

        /// <summary>
        /// ReceivedData from the GraphQL server
        /// </summary>
        public GraphQLDataResult<T> ReceivedData { get; }
    }
}
