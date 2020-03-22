using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using SAHB.GraphQL.Client.Introspection.Validation;
using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.Introspection;

namespace SAHB.GraphQLClient
{
    public abstract class GraphQLRequest<T> : GraphQLRequestInformation, IGraphQLRequest<T>
        where T : class
    {
        private List<GraphQLField> selectionSet;

        public GraphQLRequest(IGraphQLClient client, IEnumerable<GraphQLField> selectionSet, GraphQLOperationType operationType)
            : base(client, operationType)
        {
            if (selectionSet is null)
            {
                throw new ArgumentNullException(nameof(selectionSet));
            }

            this.selectionSet = selectionSet.ToList();
        }

        public IReadOnlyCollection<GraphQLField> SelectionSet => new ReadOnlyCollection<GraphQLField>(selectionSet);

        /// <inheritdoc />
        public IEnumerable<ValidationError> Validate(GraphQLIntrospectionSchema schema)
        {
            return Client.Validator.ValidateGraphQLSelectionSet(schema, Operation, SelectionSet);
        }

        internal Func<GraphQLField, bool> GetQueryFilter<TOutput>(Expression<Func<T, TOutput>> filter)
            where TOutput : class
        {
            if (filter == null)
                return null;

            if (Client.FilterGenerator == null)
                throw new NotSupportedException("IQueryGeneratorFilter needs to be specified in constructer if filter is used");

            return Client.FilterGenerator.GetFilter(filter);
        }
    }
}
