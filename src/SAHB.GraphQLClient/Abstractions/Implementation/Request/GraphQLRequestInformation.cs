using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.Introspection;
using SAHB.GraphQLClient.QueryGenerator;

namespace SAHB.GraphQLClient
{
    public abstract class GraphQLRequestInformation : IGraphQLRequestInformation
    {
        public GraphQLRequestInformation(IGraphQLClient client, GraphQLOperationType operationType)
        {
            this.Client = client;
            this.Operation = operationType;
        }

        public IGraphQLClient Client { get; }

        public ICollection<GraphQLQueryArgument> Arguments { get; } = new List<GraphQLQueryArgument>();

        public ICollection<GraphQLQueryArgument> DirectiveArguments { get; } = new List<GraphQLQueryArgument>();

        /// <inheritdoc />
        public bool? ShouldThrowIfQueryIsInvalid { get; set; }

        /// <inheritdoc />
        public GraphQLOperationType Operation { get; }

        /// <inheritdoc />
        public string Url { get; set; }

        /// <inheritdoc />
        public IDictionary<string, string> Headers { get; } = new Dictionary<string, string>();

        /// <inheritdoc />
        public string AuthorizationMethod { get; set; }

        /// <inheritdoc />
        public string AuthorizationToken { get; set; }

        /// <inheritdoc />
        public Task<GraphQLIntrospectionSchema> GetIntrospectionSchema(CancellationToken cancellationToken = default)
        {
            return Client.GetIntrospectionSchema(cancellationToken);
        }
    }
}
