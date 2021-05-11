using System.Collections.Generic;
using System.Net.Http;
using SAHB.GraphQLClient.Deserialization;
using SAHB.GraphQLClient.Exceptions;
using SAHB.GraphQLClient.Executor;
using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.QueryGenerator;

namespace SAHB.GraphQLClient.Batching.Internal
{
    // ReSharper disable once InconsistentNaming
    /// <inheritdoc />
    internal class GraphQLBatch : IGraphQLBatch
    {
        private readonly GraphQLBatchMerger _batch;

        internal GraphQLBatch(GraphQLOperationType graphQLOperationType, string operationName, string url,
            HttpMethod httpMethod, IDictionary<string, string> headers, string authorizationToken,
            string authorizationMethod, IGraphQLHttpExecutor executor, IGraphQLFieldBuilder fieldBuilder,
            IGraphQLQueryGeneratorFromFields queryGenerator, IGraphQLDeserialization deserialization)
        {
            _batch = new GraphQLBatchMerger(graphQLOperationType, operationName, url, httpMethod, headers,
                authorizationToken, authorizationMethod, executor, fieldBuilder, queryGenerator, deserialization);
        }

        internal GraphQLBatch(GraphQLOperationType graphQLOperationType, string url, HttpMethod httpMethod,
            IDictionary<string, string> headers, string authorizationToken, string authorizationMethod,
            IGraphQLHttpExecutor executor, IGraphQLFieldBuilder fieldBuilder,
            IGraphQLQueryGeneratorFromFields queryGenerator, IGraphQLDeserialization deserialization) : this(
            graphQLOperationType, null, url, httpMethod, headers, authorizationToken, authorizationMethod, executor,
            fieldBuilder, queryGenerator, deserialization)
        {
        }

        /// <inheritdoc />
        public IGraphQLQuery<T> Query<T>(params GraphQLQueryArgument[] arguments) where T : class
        {
            if (_batch.Executed)
            {
                throw new GraphQLBatchAlreadyExecutedException();
            }

            return _batch.AddQuery<T>(arguments);
        }

        /// <inheritdoc />
        public bool IsExecuted()
        {
            return _batch.Executed;
        }
    }
}
