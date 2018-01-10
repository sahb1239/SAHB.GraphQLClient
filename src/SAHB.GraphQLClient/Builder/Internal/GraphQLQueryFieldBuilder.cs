using System.Collections.Generic;
using SAHB.GraphQLClient.FieldBuilder;

namespace SAHB.GraphQLClient.Builder.Internal
{
    // ReSharper disable once InconsistentNaming
    internal class GraphQLQueryFieldBuilder : GraphQLBuilder, IGraphQLQueryFieldBuilder
    {
        private readonly string _field;
        private string _alias;
        private readonly List<GraphQLFieldArguments> _arguments = new List<GraphQLFieldArguments>();

        public GraphQLQueryFieldBuilder(string field)
        {
            _field = field;
        }

        public IGraphQLQueryFieldBuilder Alias(string alias)
        {
            _alias = alias;   
            return this;
        }

        public IGraphQLQueryFieldBuilder Argument(string argumentName, string argumentType, string variableName)
        {
            _arguments.Add(new GraphQLFieldArguments(argumentName, argumentType, variableName));
            return this;
        }

        public GraphQLField GetField()
        {
            return new GraphQLField(alias: _alias, field: _field, fields: GetFields(), arguments: _arguments);
        }
    }
}