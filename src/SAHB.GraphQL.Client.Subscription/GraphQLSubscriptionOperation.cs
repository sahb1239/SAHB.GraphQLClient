using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SAHB.GraphQLClient;
using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.Deserialization;
using SAHB.GraphQLClient.Result;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SAHB.GraphQLClient.Subscription
{
    internal class GraphQLSubscriptionOperation<T> : IGraphQLSubscriptionOperation<T>
        where T : class
    {
        private readonly GraphQLOperationSource operationSource;
        private readonly IEnumerable<GraphQLField> selectionSet;
        private readonly IGraphQLDeserialization deserialization;

        public event EventHandler<GraphQLDataReceivedEventArg<T>> DataRecieved;
        public event EventHandler<GraphQLDataReceivedEventArg<T>> ErrorRecieved;
        public event EventHandler Completed;

        public GraphQLSubscriptionOperation(GraphQLOperationSource operationSource, IEnumerable<GraphQLField> selectionSet, IGraphQLDeserialization deserialization)
        {
            this.operationSource = operationSource;
            this.selectionSet = selectionSet;
            this.deserialization = deserialization;
            this.operationSource.RecievePayload += OperationSource_RecievePayload;
            this.operationSource.Completed += OperationSource_Completed;
        }

        private void OperationSource_Completed(object sender, EventArgs e)
        {
            Completed?.Invoke(this, new EventArgs());
        }

        private void OperationSource_RecievePayload(object sender, PayloadEventArgs e)
        {
            if (_isStopped)
                return;

            // Get GraphQLResult
            var result = e.Payload.ToObject<GraphQLDataResult<JObject>>();

            // Add final result
            var finalResult = new GraphQLDataResult<T>()
            {
                Errors = result.Errors
            };

            // Deserilize data
            if (result.Data != null)
            {
                var data = deserialization.DeserializeResult<T>(result.Data, selectionSet);
                finalResult.Data = data;
            }

            // Send event
            if (result.ContainsErrors)
            {
                ErrorRecieved?.Invoke(this, new GraphQLDataReceivedEventArg<T>(finalResult));
            }
            else
            {
                DataRecieved?.Invoke(this, new GraphQLDataReceivedEventArg<T>(finalResult));
            }
        }

        private bool _isStopped = false;
        public Task Stop()
        {
            _isStopped = true;
            return this.operationSource.Stop();
        }
    }

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
