using System;
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
        private readonly List<GraphQLPossibleType> _possibleTypes = new List<GraphQLPossibleType>();

        internal GraphQLQueryFieldBuilder(string field)
        {
            _field = field;
        }

        /// <inheritdoc />
        public IGraphQLQueryFieldBuilder Alias(string alias)
        {
            _alias = alias;   
            return this;
        }

        /// <inheritdoc />
        public IGraphQLQueryFieldBuilder Argument(string argumentName, string argumentType, string variableName)
        {
            _arguments.Add(new GraphQLFieldArguments(argumentName, argumentType, variableName));
            return this;
        }

        /// <inheritdoc />
        public IGraphQLQueryFieldBuilder Argument(string argumentName, string argumentType, string variableName, bool isRequired)
        {
            _arguments.Add(new GraphQLFieldArguments(argumentName, argumentType, variableName, isRequired));
            return this;
        }

        /// <inheritdoc />
        public IGraphQLQueryFieldBuilder Argument(string argumentName, string argumentType, string variableName, bool isRequired,
            bool? inlineArgument)
        {
            _arguments.Add(new GraphQLFieldArguments(argumentName, argumentType, variableName, isRequired, inlineArgument));
            return this;
        }

        /// <inheritdoc />
        public IGraphQLQueryFieldBuilder Argument(string argumentName, string argumentType, string variableName, bool isRequired,
            bool? inlineArgument, object defaultValue)
        {
            _arguments.Add(new GraphQLFieldArguments(argumentName, argumentType, variableName, isRequired, inlineArgument, defaultValue));
            return this;
        }

        public IGraphQLQueryFieldBuilder PossibleType(string typeName, Action<IGraphQLBuilder> typeGenerator)
        {
            if (typeName == null) throw new ArgumentNullException(nameof(typeName));
            if (typeGenerator == null) throw new ArgumentNullException(nameof(typeGenerator));

            // Build the possible type
            var typeBuilder = new GraphQLBuilder();
            typeGenerator(typeBuilder);

            // Add the possible types
            _possibleTypes.Add(new GraphQLPossibleType(typeBuilder.GetFields(), typeName));
            return this;
        }

        internal GraphQLField GetField()
        {
            return new GraphQLField(alias: _alias, field: _field, fields: GetFields(), arguments: _arguments, possibleTypes: _possibleTypes);
        }
    }
}