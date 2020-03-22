using System;
using SAHB.GraphQL.Client.Introspection.Validation;
using SAHB.GraphQLClient.Builder;
using SAHB.GraphQLClient.Execution;
using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.Filtering;

namespace SAHB.GraphQLClient
{
    public class GraphQLClient : IGraphQLClient
    {
        /// <summary>
        /// The <see cref="IGraphQLFieldBuilder"/> used
        /// </summary>
        public IGraphQLFieldBuilder FieldBuilder { get; }

        /// <summary>
        /// The <see cref="IQueryGeneratorFilter"/>
        /// </summary>
        public IQueryGeneratorFilter FilterGenerator { get; set; }

        /// <summary>
        /// The <see cref="IGraphQLValidation"/>
        /// </summary>
        public IGraphQLValidation Validator { get; set; }

        /// <summary>
        /// The <see cref="IGraphQLExecutor"/>
        /// </summary>
        public IGraphQLExecutor Executor { get; set; }

        public GraphQLClient(
            IGraphQLFieldBuilder fieldBuilder,
            IQueryGeneratorFilter filterGenerator,
            IGraphQLValidation validator,
            IGraphQLExecutor executor)
        {
            FieldBuilder = fieldBuilder ?? throw new ArgumentNullException(nameof(fieldBuilder));
            FilterGenerator = filterGenerator ?? throw new ArgumentNullException(nameof(filterGenerator));
            Validator = validator ?? throw new ArgumentNullException(nameof(validator));
            Executor = executor ?? throw new ArgumentNullException(nameof(executor));
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
