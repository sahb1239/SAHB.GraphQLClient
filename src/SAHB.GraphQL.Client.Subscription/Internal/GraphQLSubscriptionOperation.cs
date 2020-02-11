using Newtonsoft.Json.Linq;
using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.Deserialization;
using SAHB.GraphQLClient.Result;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

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
                catch (Exception ex)
                {
                    Logger?.LogError(new EventId(2), ex, "Exception deserilizing message");

                    LogMessage(result);
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

        private void LogMessage(GraphQLDataResult<JObject> result)
        {
            try
            {
                Logger?.LogError(result.Data.ToString());
            }
            catch
            {
                // Ignored
            }
        }

        private bool _isStopped = false;
        public Task Stop()
        {
            _isStopped = true;
            return this.operationSource.Stop();
        }

        #region Logging

        private ILoggerFactory _loggerFactory;

        /// <summary>
        /// Contains a logger factory for the GraphQLHttpClient
        /// </summary>
        public ILoggerFactory LoggerFactory
        {
            internal get { return _loggerFactory; }
            set
            {
                _loggerFactory = value;
                if (_loggerFactory != null)
                {
                    Logger = _loggerFactory.CreateLogger<GraphQLSubscriptionOperation<T>>();
                }
            }
        }

        /// <summary>
        /// Contains the logger for the class
        /// </summary>
        private ILogger<GraphQLSubscriptionOperation<T>> Logger { get; set; }

        #endregion
    }
}
