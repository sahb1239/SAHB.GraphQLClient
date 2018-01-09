using System;
using SAHB.GraphQLClient.FieldBuilder;

namespace SAHB.GraphQLClient.QueryGenerator
{
    // ReSharper disable once InconsistentNaming
    /// <inheritdoc cref="IGraphQLQueryGenerator" />
    public class GraphQLQueryGenerator : GraphQLQueryGeneratorFromFields, IGraphQLQueryGenerator, IGraphQLQueryGeneratorFromFields
    {
        private readonly IGraphQLFieldBuilder _graphQlFieldBuilder;

        public GraphQLQueryGenerator(IGraphQLFieldBuilder graphQlFieldBuilder)
        {
            _graphQlFieldBuilder = graphQlFieldBuilder;
        }

        /// <inheritdoc />
        public string GetQuery(Type type, params GraphQLQueryArgument[] arguments)
        {
            return GetQuery(_graphQlFieldBuilder.GetFields(type), arguments);
        }

        /// <inheritdoc />
        public string GetMutation(Type type, params GraphQLQueryArgument[] arguments)
        {
            return GetMutation(_graphQlFieldBuilder.GetFields(type), arguments);
        }
    }
}
