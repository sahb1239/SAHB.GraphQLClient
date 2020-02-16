using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using SAHB.GraphQL.Client.Introspection.Validation;
using SAHB.GraphQLClient.Exceptions;

namespace SAHB.GraphQLClient
{
    /// <summary>
    /// Extension method for <see cref="IGraphQLRequest{TInput}"/>
    /// </summary>
    public static class GraphQLRequestExtensions
    {
        #region Validation

        /// <summary>
        /// Validates the GraphQL request
        /// </summary>
        /// <typeparam name="TInput">Input type for the GraphQL request</typeparam>
        /// <param name="request">The request to validate</param>
        /// <returns>Validation errors</returns>
        public static async Task<IEnumerable<ValidationError>> Validate<TInput>(this IGraphQLRequest<TInput> request)
           where TInput : class
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var introspectionSchema = await request.GetIntrospectionSchema().ConfigureAwait(false);
            return request.Validate(introspectionSchema);
        }

        /// <summary>
        /// Validates the GraphQL request
        /// </summary>
        /// <typeparam name="TInput">Input type for the GraphQL request</typeparam>
        /// <param name="request">The request to validate</param>
        /// <returns>If the request is valid</returns>
        public static async Task<bool> IsValid<TInput>(this IGraphQLRequest<TInput> request)
           where TInput : class
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var validationErrors = await request.Validate() ?? new List<ValidationError>();

            return !validationErrors.Any();
        }

        /// <summary>
        /// Validates the GraphQL request and throws a <see cref="GraphQLValidationErrorException"/> if the request is not valid
        /// </summary>
        /// <typeparam name="TInput">Input type for the GraphQL request</typeparam>
        /// <param name="request">The request to validate</param>
        /// <returns>The parameter <paramref name="request"/></returns>
        /// <exception cref="GraphQLValidationExtentions">Thrown if the request is not valid</exception>
        public static async Task<TRequest> ThrowIfNotValid<TRequest>(this TRequest request)
            where TRequest : IGraphQLRequest<object>
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var validationErrors = await request.Validate() ?? new List<ValidationError>();

            if (validationErrors.Any())
            {
                throw new GraphQLValidationErrorException(validationErrors);
            }
            else
            {
                return request;
            }
        }

        #endregion

        #region Settings

        /// <summary>
        /// Sets the <see cref="HttpMethod"/> for the request
        /// </summary>
        /// <typeparam name="TInput">Input type for the GraphQL request</typeparam>
        /// <param name="request">The request to add the <see cref="HttpMethod"/> to</param>
        /// <param name="method">The HttpMethod</param>
        /// <returns>The parameter <paramref name="request"/></returns>
        public static TRequest SetHttpMethod<TRequest>(this TRequest request, HttpMethod method)
           where TRequest : IGraphQLHttpRequest<object>
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            request.Method = method;

            return request;
        }

        #endregion
    }
}
