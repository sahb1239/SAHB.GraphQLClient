using System;
using System.Threading.Tasks;

namespace SAHB.GraphQL.Client.Subscription
{
    public interface IGraphQLSubscriptionOperation<T> where T : class
    {
        event EventHandler<GraphQLDataReceivedEventArg<T>> DataRecieved;
        event EventHandler<GraphQLDataReceivedEventArg<T>> ErrorRecieved;
        event EventHandler Completed;

        Task Stop();
    }
}
