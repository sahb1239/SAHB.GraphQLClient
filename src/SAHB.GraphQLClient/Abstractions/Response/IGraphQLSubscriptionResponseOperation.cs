using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SAHB.GraphQLClient
{
    public interface IGraphQLSubscriptionResponseOperation<TInput, TOutput>
        where TInput : class
        where TOutput : class
    {
        IAsyncEnumerable<IGraphQLSubscriptionResponse<TInput, TOutput>> GetEnumerable();

        Task Reconnect(CancellationToken cancellationToken = default);
        Task Stop(CancellationToken cancellationToken = default);

        event EventHandler Reconnected;
        event EventHandler Disconnected;
    }
}
