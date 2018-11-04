using SAHB.GraphQLClient.Result;
using System;

namespace SAHB.GraphQL.Client.Subscription
{
    public class GraphQLDataReceivedEventArg<T> : EventArgs
        where T : class
    {
        public GraphQLDataReceivedEventArg(GraphQLDataResult<T> data)
        {
            ReceivedData = data;
        }

        public GraphQLDataResult<T> ReceivedData { get; }
    }
}
