using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace SAHB.GraphQLClient.Subscription.Internal
{
    internal class GraphQLOperationSource
    {
        private readonly Func<Task> stop;

        public GraphQLOperationSource(Func<Task> stop)
        {
            this.stop = stop;
        }

        public void HandlePayload(JObject payload)
        {
            RecievePayload?.Invoke(this, new PayloadEventArgs(payload));
        }

        public void HandleCompleted()
        {
            Completed?.Invoke(this, new EventArgs());
        }

        public Task Stop()
        {
            return stop();
        }

        public event EventHandler<PayloadEventArgs> RecievePayload;
        public event EventHandler Completed;
    }
}
