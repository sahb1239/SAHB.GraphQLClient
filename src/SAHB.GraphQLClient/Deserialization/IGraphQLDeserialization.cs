using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.Result;
using System;
using System.Collections.Generic;
using System.Text;

namespace SAHB.GraphQL.Client.Deserialization
{
    /// <summary>
    /// Handles deserilization of GraphQL responses
    /// </summary>
    public interface IGraphQLDeserialization
    {
        /// <summary>
        /// Desilize a GraphQL result into a <see cref="GraphQLDataResult{T}"/>
        /// </summary>
        /// <typeparam name="T">The type to deserilize into</typeparam>
        /// <param name="graphQLResult">The GraphQL result from the server</param>
        /// <param name="fields">The GraphQL fields</param>
        /// <returns>The GraphQL Result</returns>
        GraphQLDataResult<T> DeserializeResult<T>(string graphQLResult, IEnumerable<GraphQLField> fields) where T : class;
    }
}
