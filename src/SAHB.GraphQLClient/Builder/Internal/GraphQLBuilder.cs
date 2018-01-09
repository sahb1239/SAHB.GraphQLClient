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
        
        public IGraphQLBuilder Field(string field, Action<IGraphQLQueryFieldBuilder> generator)
        {
            var fieldGen = new GraphQLQueryFieldBuilder(field);
            generator(fieldGen);
            Fields.Add(fieldGen);
            return this;
        }

        public IEnumerable<GraphQLField> GetFields()
        {
            return Fields.Select(field => field.GetField());
        }
    }
}
