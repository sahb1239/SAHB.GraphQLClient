using System;
using System.Collections.Generic;
using System.Linq;
using SAHB.GraphQLClient.FieldBuilder;

namespace SAHB.GraphQLClient.Builder.Internal
{
    // ReSharper disable once InconsistentNaming
    internal class GraphQLBuilder : IGraphQLBuilder
    {
        protected List<GraphQLQueryFieldBuilder> Fields = new List<GraphQLQueryFieldBuilder>();

        /// <inheritdoc />
        public IGraphQLBuilder Field(string field)
        {
            return Field(field, null);
        }

        /// <inheritdoc />
        public IGraphQLBuilder Field(string field, Action<IGraphQLQueryFieldBuilder> generator)
        {
            var fieldGen = new GraphQLQueryFieldBuilder(field);
            generator?.Invoke(fieldGen);
            Fields.Add(fieldGen);
            return this;
        }

        internal IEnumerable<IGraphQLField> GetFields()
        {
            return Fields.Select(field => field.GetField());
        }
    }
}
