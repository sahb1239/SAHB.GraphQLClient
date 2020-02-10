using Newtonsoft.Json.Linq;
using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.Deserialization;
using SAHB.GraphQLClient.Result;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SAHB.GraphQLClient.Subscription.Internal
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
                try
                {
                    var data = deserialization.DeserializeResult<T>(result.Data, selectionSet);
                    finalResult.Data = data;
                }
                catch
                {
                    // Ignored
                }
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
}
