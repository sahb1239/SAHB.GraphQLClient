using System.Collections.Generic;
using SAHB.GraphQLClient.Result;

namespace SAHB.GraphQLClient
{
    public interface IGraphQLResponse<in TInput, out TOutput, out TInputRequest>
        where TInput : class
        where TOutput : class
        where TInputRequest : IGraphQLRequest<object>
    {
        TInputRequest Request { get; }

        /// <summary>
        /// Contains the output from the GraphQL server.
        /// </summary>
        TOutput Data { get; }

        /// <summary>
        /// The errors which occured on execution of the query
        /// </summary>
        IReadOnlyCollection<GraphQLDataError> Errors { get; }

        /// <summary>
        /// Contains additional data
        /// </summary>
        IReadOnlyDictionary<string, object> AdditionalData { get; }

        /// <summary>
        /// Returns the raw output from the server
        /// </summary>
        /// <returns>Raw output from the server</returns>
        string GetRawOutput();
    }
}
