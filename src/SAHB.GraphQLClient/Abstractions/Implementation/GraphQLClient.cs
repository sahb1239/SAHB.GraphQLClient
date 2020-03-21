using System;
using SAHB.GraphQL.Client.Introspection.Validation;
using SAHB.GraphQLClient.Builder;
using SAHB.GraphQLClient.Deserialization;
using SAHB.GraphQLClient.Executor;
using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.Filtering;
using SAHB.GraphQLClient.QueryGenerator;

namespace SAHB.GraphQLClient
{
    public class GraphQLClient : IGraphQLClient
    {
        /// <summary>
        /// The <see cref="IGraphQLFieldBuilder"/> used
        /// </summary>
        public IGraphQLFieldBuilder FieldBuilder { get; }

        /// <summary>
        /// The <see cref="IGraphQLHttpExecutor"/> used
        /// </summary>
        public IGraphQLHttpExecutor HttpExecutor { get; }

        /// <summary>
        /// The <see cref="IGraphQLQueryGeneratorFromFields"/> used
        /// </summary>
        public IGraphQLQueryGeneratorFromFields QueryGenerator { get; }

        /// <summary>
        /// The <see cref="IGraphQLDeserialization"/> used
        /// </summary>
        public IGraphQLDeserialization Deserialization { get; }

        /// <summary>
        /// The <see cref="IQueryGeneratorFilter"/>
        /// </summary>
        public IQueryGeneratorFilter FilterGenerator { get; }

        /// <summary>
        /// The <see cref="IGraphQLValidation"/>
        /// </summary>
        public IGraphQLValidation Validator { get; set; }

        public GraphQLClient(
            IGraphQLFieldBuilder fieldBuilder, 
            IGraphQLHttpExecutor httpExecutor, 
            IGraphQLQueryGeneratorFromFields queryGenerator,
            IGraphQLDeserialization deserialization, 
            IQueryGeneratorFilter filterGenerator,
            IGraphQLValidation validator)
        {
            FieldBuilder = fieldBuilder ?? throw new ArgumentNullException(nameof(fieldBuilder));
            HttpExecutor = httpExecutor ?? throw new ArgumentNullException(nameof(httpExecutor));
            QueryGenerator = queryGenerator ?? throw new ArgumentNullException(nameof(queryGenerator));
            Deserialization = deserialization ?? throw new ArgumentNullException(nameof(deserialization));
            FilterGenerator = filterGenerator ?? throw new ArgumentNullException(nameof(filterGenerator));
            Validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        public IGraphQLBatchRequest CreateBatchRequest(GraphQLOperationType operationType)
        {
            return new GraphQLBatchRequest(this, operationType);
        }

        public IGraphQLHttpRequest<T> CreateHttpRequest<T>(GraphQLOperationType operationType) where T : class
        {
            return new GraphQLHttpRequest<T>(this, this.GetSelectionSet<T>(), operationType);
        }

        public IGraphQLHttpRequest<dynamic> CreateHttpRequest(GraphQLOperationType operationType, Action<IGraphQLBuilder> queryBuilder)
        {
            return new GraphQLHttpRequest<dynamic>(this, queryBuilder.GetSelectionSet(), operationType);
        }

        public IGraphQLSubscriptionRequest<T> CreateSubscriptionRequest<T>(GraphQLOperationType operationType) where T : class
        {
            return new GraphQLSubscriptionRequest<T>(this, this.GetSelectionSet<T>(), operationType);
        }

        public IGraphQLSubscriptionRequest<dynamic> CreateSubscriptionRequest(GraphQLOperationType operationType, Action<IGraphQLBuilder> queryBuilder)
        {
            return new GraphQLSubscriptionRequest<dynamic>(this, queryBuilder.GetSelectionSet(), operationType);
        }
    }
}
