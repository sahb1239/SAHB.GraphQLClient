using System;
using System.Linq.Expressions;
using SAHB.GraphQLClient.QueryGenerator;
using SAHB.GraphQLClient.FieldBuilder.Attributes;

namespace SAHB.GraphQLClient
{
    /// <summary>
    /// Extension method for <see cref="GraphQLRequestInformation"/>
    /// </summary>
    public static class GraphQLRequestInformationExtentions
    {
        #region AddArgument

        /// <summary>
        /// Adds a argument to the GraphQL request
        /// </summary>
        /// <typeparam name="TRequest">The request type</typeparam>
        /// <param name="request">The request to add a argument to</param>
        /// <param name="argument">The argument to add to the request</param>
        /// <returns>The parameter <paramref name="request"/></returns>
        public static TRequest AddArgument<TRequest>(this TRequest request, GraphQLQueryArgument argument)
            where TRequest : IGraphQLRequestInformation
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (argument is null)
            {
                throw new ArgumentNullException(nameof(argument));
            }

            request.Arguments.Add(argument);

            return request;
        }

        /// <summary>
        /// Adds a argument to the GraphQL request
        /// </summary>
        /// <typeparam name="TRequest">The request type</typeparam>
        /// <param name="request">The request to add a argument to</param>
        /// <param name="variableName">The variable name which should be set used in the <see cref="GraphQLArgumentsAttribute"/></param>
        /// <param name="argumentValue">The value which is inserted in the variables part of the GraphQL query</param>
        /// <returns>The parameter <paramref name="request"/></returns>
        public static TRequest AddArgument<TRequest>(this TRequest request, string variableName, object argumentValue)
            where TRequest : IGraphQLRequestInformation
        {
            return request.AddArgument(
                new GraphQLQueryArgument(
                    variableName: variableName,
                    argumentValue: argumentValue));
        }

        /// <summary>
        /// Adds a argument to the GraphQL request
        /// </summary>
        /// <typeparam name="TRequest">The request type</typeparam>
        /// <param name="request">The request to add a argument to</param>
        /// <param name="variableName">The variable name which should be set used in the <see cref="GraphQLArgumentsAttribute"/></param>
        /// <param name="argumentValue">The value which is inserted in the variables part of the GraphQL query</param>
        /// <param name="field">The GraphQL field which should have applied the argument/param>
        /// <returns>The parameter <paramref name="request"/></returns>
        public static TRequest AddArgument<TRequest>(this TRequest request, string variableName, object argumentValue, string field)
            where TRequest : IGraphQLRequestInformation
        {
            return request.AddArgument(
                new GraphQLQueryArgument(
                    variableName: variableName,
                    field: field,
                    argumentValue: argumentValue));
        }

        /// <summary>
        /// Adds a argument to the GraphQL request
        /// </summary>
        /// <typeparam name="TRequest">The request type</typeparam>
        /// <param name="request">The request to add a argument to</param>
        /// <param name="variableName">The variable name which should be set used in the <see cref="GraphQLArgumentsAttribute"/></param>
        /// <param name="argumentValue">The value which is inserted in the variables part of the GraphQL query</param>
        /// <param name="field">The field which the argument should be applied to</param>
        /// <returns>The parameter <paramref name="request"/></returns>
        public static TRequest AddArgument<TInput, TRequest>(this TRequest request, string variableName, object argumentValue, Expression<Func<TInput, object>> field)
            where TInput : class
            where TRequest : IGraphQLRequestInformation
        {
            return request.AddArgument<TRequest>(
                new GraphQLQueryArgument<TInput>(
                    variableName: variableName,
                    argumentValue: argumentValue,
                    field: field));
        }

        #endregion

        #region AddDirectiveArgument

        /// <summary>
        /// Adds a directive to the GraphQL request
        /// </summary>
        /// <typeparam name="TRequest">The request type</typeparam>
        /// <param name="request">The request to add a argument to</param>
        /// <param name="argument">The directive argument to add to the request</param>
        /// <returns>The parameter <paramref name="request"/></returns>
        public static TRequest AddDirectiveArgument<TRequest>(this TRequest request, GraphQLQueryArgument argument)
            where TRequest : IGraphQLRequestInformation
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (argument is null)
            {
                throw new ArgumentNullException(nameof(argument));
            }

            request.DirectiveArguments.Add(argument);

            return request;
        }

        /// <summary>
        /// Adds a directive to the GraphQL request
        /// </summary>
        /// <typeparam name="TRequest">The request type</typeparam>
        /// <param name="request">The request to add a argument to</param>
        /// <param name="variableName">The variable name which should be set used in the <see cref="GraphQLArgumentsAttribute"/></param>
        /// <param name="directiveName">The directiveName which should have applied the argument</param>
        /// <param name="argumentValue">The value which is inserted in the variables part of the GraphQL query</param>
        /// <returns>The parameter <paramref name="request"/></returns>
        public static TRequest AddDirectiveArgument<TRequest>(this TRequest request, string variableName, string directiveName, object argumentValue)
            where TRequest : IGraphQLRequestInformation
        {
            return request.AddDirectiveArgument(
                new GraphQLQueryDirectiveArgument(
                    variableName: variableName,
                    directiveName: directiveName,
                    argumentValue: argumentValue));
        }

        /// <summary>
        /// Adds a directive to the GraphQL request
        /// </summary>
        /// <typeparam name="TRequest">The request type</typeparam>
        /// <param name="request">The request to add a argument to</param>
        /// <param name="variableName">The variable name which should be set used in the <see cref="GraphQLArgumentsAttribute"/></param>
        /// <param name="directiveName">The directiveName which should have applied the argument</param>
        /// <param name="argumentValue">The value which is inserted in the variables part of the GraphQL query</param>
        /// <param name="field">The GraphQL field which should have applied the directive argument/param>
        /// <returns>The parameter <paramref name="request"/></returns>
        public static TRequest AddDirectiveArgument<TRequest>(this TRequest request, string variableName, string directiveName, object argumentValue, string field)
            where TRequest : IGraphQLRequestInformation
        {
            return request.AddDirectiveArgument(
                new GraphQLQueryDirectiveArgument(
                    variableName: variableName,
                    directiveName: directiveName,
                    argumentValue: argumentValue,
                    field: field));
        }

        /// <summary>
        /// Adds a directive to the GraphQL request
        /// </summary>
        /// <typeparam name="TRequest">The request type</typeparam>
        /// <param name="request">The request to add a argument to</param>
        /// <param name="variableName">The variable name which should be set used in the <see cref="GraphQLArgumentsAttribute"/></param>
        /// <param name="directiveName">The directiveName which should have applied the argument</param>
        /// <param name="argumentValue">The value which is inserted in the variables part of the GraphQL query</param>
        /// <param name="field">The field which the argument should be applied to</param>
        /// <returns>The parameter <paramref name="request"/></returns>
        public static TRequest AddDirectiveArgument<TInput, TRequest>(this TRequest request, string variableName, string directiveName, object argumentValue, Expression<Func<TInput, object>> field)
            where TInput : class
            where TRequest : IGraphQLRequestInformation
        {
            return request.AddDirectiveArgument(
                new GraphQLQueryDirectiveArgument<TInput>(
                    variableName: variableName,
                    directiveName: directiveName,
                    argumentValue: argumentValue,
                    expression: field));
        }

        #endregion

        #region Transport

        /// <summary>
        /// Sets a bearer token to the request
        /// Sets the AuthorizationMethod to Bearer and Authorization token to the value in the <paramref name="token"/> parameter
        /// </summary>
        /// <typeparam name="TRequest">The request type</typeparam>
        /// <param name="request">The request</param>
        /// <param name="token">The token which should be set</param>
        /// <returns>The parameter <paramref name="request"/></returns>
        public static IGraphQLRequestInformation SetBearerToken<TRequest>(this TRequest request, string token)
            where TRequest : IGraphQLRequestInformation
        {
            return request.SetAuthorzation("Bearer", token);
        }

        /// <summary>
        /// Sets the AuthorizationMethod and AuthorizationToken
        /// </summary>
        /// <typeparam name="TRequest">The request type</typeparam>
        /// <param name="request">The request</param>
        /// <param name="authorizationMethod">The authorization method to set</param>
        /// <param name="authorizationToken">The authorization token to set</param>
        /// <returns>The parameter <paramref name="request"/></returns>
        public static IGraphQLRequestInformation SetAuthorzation<TRequest>(this TRequest request, string authorizationMethod, string authorizationToken)
            where TRequest : IGraphQLRequestInformation
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (string.IsNullOrWhiteSpace(authorizationToken))
            {
                throw new ArgumentException("Token cannot be null or whitespace", nameof(authorizationToken));
            }

            request.AuthorizationMethod = authorizationMethod;
            request.AuthorizationToken = authorizationToken;

            return request;
        }

        /// <summary>
        /// Sets the url for the request
        /// </summary>
        /// <typeparam name="TRequest">The request type</typeparam>
        /// <param name="request">The request</param>
        /// <param name="url">The url to set</param>
        /// <returns>The parameter <paramref name="request"/></returns>
        public static TRequest SetUrl<TRequest>(this TRequest request, string url)
            where TRequest : IGraphQLRequestInformation
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            request.Url = url;

            return request;
        }

        /// <summary>
        /// Adds the specified header to the request
        /// </summary>
        /// <typeparam name="TRequest">The request type</typeparam>
        /// <param name="request">The request</param>
        /// <param name="name">Name of the header to add</param>
        /// <param name="value">Value of the header to add</param>
        /// <returns>The parameter <paramref name="request"/></returns>
        public static TRequest AddHeader<TRequest>(this TRequest request, string name, string value)
            where TRequest : IGraphQLRequestInformation
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Header name cannot be null or whitespace", nameof(name));
            }

            request.Headers.Add(name, value);

            return request;
        }

        #endregion

        #region Settings

        /// <summary>
        /// Sets the value <see cref="IGraphQLRequestInformation.ShouldThrowIfQueryIsInvalid"/>
        /// If the value is true the request will be validated using GraphQL schema using introspection
        /// </summary>
        /// <typeparam name="TRequest">The request type</typeparam>
        /// <param name="request">The request</param>
        /// <param name="shouldThrow">The value to set</param>
        /// <returns>The parameter <paramref name="request"/></returns>
        public static TRequest SetShouldThrowIfQueryIsInvalid<TRequest>(this TRequest request, bool shouldThrow)
            where TRequest : IGraphQLRequestInformation
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            request.ShouldThrowIfQueryIsInvalid = shouldThrow;

            return request;
        }

        #endregion
    }
}
