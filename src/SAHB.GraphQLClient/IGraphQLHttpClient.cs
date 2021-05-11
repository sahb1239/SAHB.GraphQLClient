﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using SAHB.GraphQLClient;
using SAHB.GraphQLClient.Batching;
using SAHB.GraphQLClient.Builder;
using SAHB.GraphQLClient.Exceptions;
using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.QueryGenerator;

namespace SAHB.GraphQLClient
{
    // ReSharper disable once InconsistentNaming
    /// <summary>
    /// GraphQL client which supports generating GraphQL queries and mutations from C# types
    /// </summary>
    public interface IGraphQLHttpClient
    {
        /// <summary>
        /// Executes a query on a GraphQL server using a specified type <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">The type to generate the query from</typeparam>
        /// <param name="operationType">The GraphQL Operation to execute</param>
        /// <param name="url">The endpoint to request the GraphQL server from</param>
        /// <param name="httpMethod">The httpMethod to use requesting the GraphQL server</param>
        /// <param name="headers">Headers to add to the request</param>
        /// <param name="filter">Filter used to generate the selectionSet</param>
        /// <param name="authorizationToken">Authorization token inserted in the Authorization header</param>
        /// <param name="authorizationMethod">The authorization method inserted in the Authorization header. This is only used when authorizationToken is not null</param>
        /// <param name="cancellationToken">A token that signals that the caller requested cancellation of this method invocation</param>
        /// <param name="arguments">The arguments used in the query which is inserted in the variables</param>
        /// <returns>The result of the query</returns>
        Task<T> Execute<T>(GraphQLOperationType operationType, string url = null, HttpMethod httpMethod = null, IDictionary<string, string> headers = null, Expression<Func<T, T>> filter = null, string authorizationToken = null, string authorizationMethod = "Bearer", CancellationToken cancellationToken = default, params GraphQLQueryArgument[] arguments) where T : class;

        /// <summary>
        /// Executes a query on a GraphQL server using the specified <paramref name="builder"/>
        /// </summary>
        /// <param name="operationType">The GraphQL Operation to execute</param>
        /// <param name="builder">The builder used to generate the query</param>
        /// <param name="url">The endpoint to request the GraphQL server from</param>
        /// <param name="httpMethod">The httpMethod to use requesting the GraphQL server</param>
        /// <param name="headers">Headers to add to the request</param>
        /// <param name="authorizationToken">Authorization token inserted in the Authorization header</param>
        /// <param name="authorizationMethod">The authorization method inserted in the Authorization header. This is only used when authorizationToken is not null</param>
        /// <param name="cancellationToken">A token that signals that the caller requested cancellation of this method invocation</param>
        /// <param name="arguments">The arguments used in the query which is inserted in the variables</param>
        /// <returns>The result of the query</returns>
        Task<dynamic> Execute(GraphQLOperationType operationType, Action<IGraphQLBuilder> builder, string url = null, HttpMethod httpMethod = null, IDictionary<string, string> headers = null, string authorizationToken = null, string authorizationMethod = "Bearer", CancellationToken cancellationToken = default, params GraphQLQueryArgument[] arguments);

        /// <summary>
        /// Generates a GraphQL batch
        /// </summary>
        /// <param name="operationType">The GraphQL Operation to execute</param>
        /// <param name="url">The endpoint to request the GraphQL server from</param>
        /// <param name="httpMethod">The httpMethod to use requesting the GraphQL server</param>
        /// <param name="headers">Headers to add to the request</param>
        /// <param name="authorizationToken">The token used to authenticate with the GraphQL server</param>
        /// <param name="authorizationMethod">The method used for authentication</param>
        /// <returns></returns>
        IGraphQLBatch CreateBatch(GraphQLOperationType operationType, string url, HttpMethod httpMethod = null, IDictionary<string, string> headers = null, string authorizationToken = null, string authorizationMethod = "Bearer");

        /// <summary>
        /// Generates a GraphQL batch
        /// </summary>
        /// <param name="operationType">The GraphQL Operation to execute</param>
        /// <param name="operationName">The operation name for this batch, null if no operation name to be set</param>
        /// <param name="url">The endpoint to request the GraphQL server from</param>
        /// <param name="httpMethod">The httpMethod to use requesting the GraphQL server</param>
        /// <param name="headers">Headers to add to the request</param>
        /// <param name="authorizationToken">The token used to authenticate with the GraphQL server</param>
        /// <param name="authorizationMethod">The method used for authentication</param>
        /// <returns></returns>
        IGraphQLBatch CreateBatch(GraphQLOperationType operationType, string operationName, string url, HttpMethod httpMethod = null, IDictionary<string, string> headers = null, string authorizationToken = null, string authorizationMethod = "Bearer");
        
        /// <summary>
        /// Generates a query to a GraphQL server using a query generated by the <see cref="IGraphQLBuilder"/>, the specified URL and the HttpMethod Post
        /// </summary>
        /// <param name="builder">The builder used to generate the query</param>
        /// <param name="url">The url to request the GraphQL server from using HTTP Post</param>
        /// <param name="authorizationToken">Authorization token inserted in the Authorization header</param>
        /// <param name="authorizationMethod">The authorization method inserted in the Authorization header. This is only used when authorizationToken is not null</param>
        /// <param name="arguments">The arguments used in the query which is inserted in the variables</param>
        /// <returns>The query generated ready to be executed</returns>
        /// <exception cref="GraphQLErrorException">Thrown when validation or GraphQL endpoint returns an error</exception>
        IGraphQLQuery CreateQuery(Action<IGraphQLBuilder> builder, string url, string authorizationToken = null, string authorizationMethod = "Bearer", params GraphQLQueryArgument[] arguments);

        /// <summary>
        /// Generates a query to a GraphQL server using a specified type, the specified URL and the HttpMethod Post
        /// </summary>
        /// <typeparam name="T">The type to generate the query from</typeparam>
        /// <param name="url">The url to request the GraphQL server from using HTTP Post</param>
        /// <param name="authorizationToken">Authorization token inserted in the Authorization header</param>
        /// <param name="authorizationMethod">The authorization method inserted in the Authorization header. This is only used when authorizationToken is not null</param>
        /// <param name="arguments">The arguments used in the query which is inserted in the variables</param>
        /// <returns>The query generated ready to be executed</returns>
        /// <exception cref="GraphQLErrorException">Thrown when validation or GraphQL endpoint returns an error</exception>
        IGraphQLQuery<T> CreateQuery<T>(string url, string authorizationToken = null, string authorizationMethod = "Bearer", params GraphQLQueryArgument[] arguments) where T : class;

        /// <summary>
        /// Generates a query to a GraphQL server using a specified type, the specified URL and the HttpMethod Post
        /// </summary>
        /// <typeparam name="T">The type to generate the query from</typeparam>
        /// <param name="url">The url to request the GraphQL server from using HTTP Post</param>
        /// <param name="filter">Filter used to generate the selectionSet</param>
        /// <param name="authorizationToken">Authorization token inserted in the Authorization header</param>
        /// <param name="authorizationMethod">The authorization method inserted in the Authorization header. This is only used when authorizationToken is not null</param>
        /// <param name="arguments">The arguments used in the query which is inserted in the variables</param>
        /// <returns>The query generated ready to be executed</returns>
        /// <exception cref="GraphQLErrorException">Thrown when validation or GraphQL endpoint returns an error</exception>
        IGraphQLQuery<T> CreateQuery<T>(string url, Expression<Func<T, T>> filter, string authorizationToken = null, string authorizationMethod = "Bearer", params GraphQLQueryArgument[] arguments) where T : class;

        /// <summary>
        /// Generates a mutation to a GraphQL server using a query generated by the <see cref="IGraphQLBuilder"/>, the specified URL and the HttpMethod Post
        /// </summary>
        /// <param name="builder">The builder used to generate the query</param>
        /// <param name="url">The url to request the GraphQL server from using HTTP Post</param>
        /// <param name="authorizationToken">Authorization token inserted in the Authorization header</param>
        /// <param name="authorizationMethod">The authorization method inserted in the Authorization header. This is only used when authorizationToken is not null</param>
        /// <param name="arguments">The arguments used in the query which is inserted in the variables</param>
        /// <returns>The query generated ready to be executed</returns>
        /// <exception cref="GraphQLErrorException">Thrown when validation or GraphQL endpoint returns an error</exception>
        IGraphQLQuery CreateMutation(Action<IGraphQLBuilder> builder, string url, string authorizationToken = null, string authorizationMethod = "Bearer", params GraphQLQueryArgument[] arguments);

        /// <summary>
        /// Generates a mutation to a GraphQL server using a specified type, the specified URL and the HttpMethod Post
        /// </summary>
        /// <typeparam name="T">The type to generate the mutation from</typeparam>
        /// <param name="url">The url to request the GraphQL server from using HTTP Post</param>
        /// <param name="authorizationToken">Authorization token inserted in the Authorization header</param>
        /// <param name="authorizationMethod">The authorization method inserted in the Authorization header. This is only used when authorizationToken is not null</param>
        /// <param name="arguments">The arguments used in the query which is inserted in the variables</param>
        /// <returns>The query generated ready to be executed</returns>
        /// <exception cref="GraphQLErrorException">Thrown when validation or GraphQL endpoint returns an error</exception>
        IGraphQLQuery<T> CreateMutation<T>(string url, string authorizationToken = null, string authorizationMethod = "Bearer", params GraphQLQueryArgument[] arguments) where T : class;

        /// <summary>
        /// Generates a mutation to a GraphQL server using a specified type, the specified URL and the HttpMethod Post
        /// </summary>
        /// <typeparam name="T">The type to generate the mutation from</typeparam>
        /// <param name="url">The url to request the GraphQL server from using HTTP Post</param>
        /// <param name="filter">Filter used to generate the selectionSet</param>
        /// <param name="authorizationToken">Authorization token inserted in the Authorization header</param>
        /// <param name="authorizationMethod">The authorization method inserted in the Authorization header. This is only used when authorizationToken is not null</param>
        /// <param name="arguments">The arguments used in the query which is inserted in the variables</param>
        /// <returns>The query generated ready to be executed</returns>
        /// <exception cref="GraphQLErrorException">Thrown when validation or GraphQL endpoint returns an error</exception>
        IGraphQLQuery<T> CreateMutation<T>(string url, Expression<Func<T, T>> filter, string authorizationToken = null, string authorizationMethod = "Bearer", params GraphQLQueryArgument[] arguments) where T : class;

        /// <summary>
        /// Generates a mutation to a GraphQL server using a query generated by the <see cref="IGraphQLBuilder"/>, the specified URL and the HttpMethod
        /// </summary>
        /// <param name="builder">The builder used to generate the query</param>
        /// <param name="url">The url to request the GraphQL server</param>
        /// <param name="httpMethod">The httpMethod to use requesting the GraphQL server</param>
        /// <param name="authorizationToken">Authorization token inserted in the Authorization header</param>
        /// <param name="authorizationMethod">The authorization method inserted in the Authorization header. This is only used when authorizationToken is not null</param>
        /// <param name="arguments">The arguments used in the query which is inserted in the variables</param>
        /// <returns>The query generated ready to be executed</returns>
        /// <exception cref="GraphQLErrorException">Thrown when validation or GraphQL endpoint returns an error</exception>
        IGraphQLQuery CreateMutation(Action<IGraphQLBuilder> builder, string url, HttpMethod httpMethod, string authorizationToken = null, string authorizationMethod = "Bearer", params GraphQLQueryArgument[] arguments);

        /// <summary>
        /// Generates a mutation to a GraphQL server using a specified type, the specified URL and the HttpMethod
        /// </summary>
        /// <typeparam name="T">The type to generate the mutation from</typeparam>
        /// <param name="url">The url to request the GraphQL server</param>
        /// <param name="httpMethod">The httpMethod to use requesting the GraphQL server</param>
        /// <param name="authorizationToken">Authorization token inserted in the Authorization header</param>
        /// <param name="authorizationMethod">The authorization method inserted in the Authorization header. This is only used when authorizationToken is not null</param>
        /// <param name="arguments">The arguments used in the query which is inserted in the variables</param>
        /// <returns>The query generated ready to be executed</returns>
        /// <exception cref="GraphQLErrorException">Thrown when validation or GraphQL endpoint returns an error</exception>
        IGraphQLQuery<T> CreateMutation<T>(string url, HttpMethod httpMethod, string authorizationToken = null, string authorizationMethod = "Bearer", params GraphQLQueryArgument[] arguments) where T : class;

        /// <summary>
        /// Generates a mutation to a GraphQL server using a specified type, the specified URL and the HttpMethod
        /// </summary>
        /// <typeparam name="T">The type to generate the mutation from</typeparam>
        /// <param name="url">The url to request the GraphQL server</param>
        /// <param name="httpMethod">The httpMethod to use requesting the GraphQL server</param>
        /// <param name="filter">Filter used to generate the selectionSet</param>
        /// <param name="authorizationToken">Authorization token inserted in the Authorization header</param>
        /// <param name="authorizationMethod">The authorization method inserted in the Authorization header. This is only used when authorizationToken is not null</param>
        /// <param name="arguments">The arguments used in the query which is inserted in the variables</param>
        /// <returns>The query generated ready to be executed</returns>
        /// <exception cref="GraphQLErrorException">Thrown when validation or GraphQL endpoint returns an error</exception>
        IGraphQLQuery<T> CreateMutation<T>(string url, HttpMethod httpMethod, Expression<Func<T, T>> filter, string authorizationToken = null, string authorizationMethod = "Bearer", params GraphQLQueryArgument[] arguments) where T : class;

        /// <summary>
        /// Generates a query to a GraphQL server using a query generated by the <see cref="IGraphQLBuilder"/>, the specified URL and the HttpMethod
        /// </summary>
        /// <param name="builder">The builder used to generate the query</param>
        /// <param name="url">The url to request the GraphQL server</param>
        /// <param name="httpMethod">The httpMethod to use requesting the GraphQL server</param>
        /// <param name="authorizationToken">Authorization token inserted in the Authorization header</param>
        /// <param name="authorizationMethod">The authorization method inserted in the Authorization header. This is only used when authorizationToken is not null</param>
        /// <param name="arguments">The arguments used in the query which is inserted in the variables</param>
        /// <returns>The query generated ready to be executed</returns>
        /// <exception cref="GraphQLErrorException">Thrown when validation or GraphQL endpoint returns an error</exception>
        IGraphQLQuery CreateQuery(Action<IGraphQLBuilder> builder, string url, HttpMethod httpMethod, string authorizationToken = null, string authorizationMethod = "Bearer", params GraphQLQueryArgument[] arguments);

        /// <summary>
        /// Generates a query to a GraphQL server using a specified type, the specified URL and the HttpMethod
        /// </summary>
        /// <typeparam name="T">The type to generate the query from</typeparam>
        /// <param name="url">The url to request the GraphQL server</param>
        /// <param name="httpMethod">The httpMethod to use requesting the GraphQL server</param>
        /// <param name="authorizationToken">Authorization token inserted in the Authorization header</param>
        /// <param name="authorizationMethod">The authorization method inserted in the Authorization header. This is only used when authorizationToken is not null</param>
        /// <param name="arguments">The arguments used in the query which is inserted in the variables</param>
        /// <returns>The query generated ready to be executed</returns>
        /// <exception cref="GraphQLErrorException">Thrown when validation or GraphQL endpoint returns an error</exception>
        IGraphQLQuery<T> CreateQuery<T>(string url, HttpMethod httpMethod, string authorizationToken = null, string authorizationMethod = "Bearer", params GraphQLQueryArgument[] arguments) where T : class;

        /// <summary>
        /// Generates a query to a GraphQL server using a specified type, the specified URL and the HttpMethod
        /// </summary>
        /// <typeparam name="T">The type to generate the query from</typeparam>
        /// <param name="url">The url to request the GraphQL server</param>
        /// <param name="httpMethod">The httpMethod to use requesting the GraphQL server</param>
        /// <param name="filter">Filter used to generate the selectionSet</param>
        /// <param name="authorizationToken">Authorization token inserted in the Authorization header</param>
        /// <param name="authorizationMethod">The authorization method inserted in the Authorization header. This is only used when authorizationToken is not null</param>
        /// <param name="arguments">The arguments used in the query which is inserted in the variables</param>
        /// <returns>The query generated ready to be executed</returns>
        /// <exception cref="GraphQLErrorException">Thrown when validation or GraphQL endpoint returns an error</exception>
        IGraphQLQuery<T> CreateQuery<T>(string url, HttpMethod httpMethod, Expression<Func<T, T>> filter, string authorizationToken = null, string authorizationMethod = "Bearer", params GraphQLQueryArgument[] arguments) where T : class;

        /// <summary>
        /// Generates a GraphQL batch using the specified server <paramref name="url"/>, the specified <paramref name="authorizationToken"/> and the specified <paramref name="authorizationMethod"/>
        /// Default HttpMethod is POST
        /// </summary>
        /// <param name="url">The endpoint to request the GraphQL server from</param>
        /// <param name="authorizationToken">The token used to authenticate with the GraphQL server</param>
        /// <param name="authorizationMethod">The method used for authentication</param>
        /// <returns></returns>
        IGraphQLBatch CreateBatch(string url, string authorizationToken = null, string authorizationMethod = "Bearer");

        /// <summary>
        /// Generates a GraphQL batch using the specified server <paramref name="url"/>, the specified <paramref name="authorizationToken"/> and the specified <paramref name="authorizationMethod"/>
        /// </summary>
        /// <param name="url">The endpoint to request the GraphQL server from</param>
        /// <param name="httpMethod">The HttpMethod to use to communicate with the server, for example POST</param>
        /// <param name="authorizationToken">The token used to authenticate with the GraphQL server</param>
        /// <param name="authorizationMethod">The method used for authentication</param>
        /// <returns></returns>
        IGraphQLBatch CreateBatch(string url, HttpMethod httpMethod, string authorizationToken = null, string authorizationMethod = "Bearer");
        
        /// <summary>
        /// Generates a GraphQL batch using the specified server <paramref name="url"/>, the specified <paramref name="authorizationToken"/> and the specified <paramref name="authorizationMethod"/>
        /// </summary>
        /// <param name="operationName">The operation name for the batch execution, can be null if no operation name to be set</param>
        /// <param name="url">The endpoint to request the GraphQL server from</param>
        /// <param name="httpMethod">The HttpMethod to use to communicate with the server, for example POST</param>
        /// <param name="authorizationToken">The token used to authenticate with the GraphQL server</param>
        /// <param name="authorizationMethod">The method used for authentication</param>
        /// <returns></returns>
        IGraphQLBatch CreateBatch(string operationName, string url, HttpMethod httpMethod, string authorizationToken = null, string authorizationMethod = "Bearer");
    }
}
