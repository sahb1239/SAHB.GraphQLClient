using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SAHB.GraphQLClient.Exceptions;
using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.Introspection;
using SAHB.GraphQLClient.QueryGenerator;

namespace SAHB.GraphQLClient
{
    public interface IGraphQLRequestInformation
    {
        /// <summary>
        /// GraphQL operation to execute on the server.
        /// </summary>
        GraphQLOperationType Operation { get; }

        /// <summary>
        /// The arguments to add to the GraphQL request.
        /// </summary>
        ICollection<GraphQLQueryArgument> Arguments { get; }

        /// <summary>
        /// The directive arguments to add to the GraphQL request.
        /// </summary>
        ICollection<GraphQLQueryArgument> DirectiveArguments { get; }

        /// <summary>
        /// If set to true, the query will be validated using the GraphQL schema using introspection and the execution will throw a <see cref="GraphQLValidationErrorException"/> if the query has any validation errors
        /// </summary>
        bool? ShouldThrowIfQueryIsInvalid { get; set; }

        /// <summary>
        /// The request url to execute against.
        /// </summary>
        string Url { get; set; }

        /// <summary>
        /// Headers to add to the request.
        /// </summary>
        IDictionary<string, string> Headers { get; }

        /// <summary>
        /// Authorization method for the request. It is ignored if <see cref="AuthorizationToken"/> is null or whitespace
        /// </summary>
        string AuthorizationMethod { get; set; }

        /// <summary>
        /// Authorization token for the request.
        /// </summary>
        string AuthorizationToken { get; set; }

        /// <summary>
        /// Returns the GraphQL introspection schema
        /// </summary>
        /// <returns>The GraphQL introspection schema</returns>
        Task<GraphQLIntrospectionSchema> GetIntrospectionSchema(CancellationToken cancellationToken = default);
    }
}
