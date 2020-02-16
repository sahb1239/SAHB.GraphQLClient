using System;
using System.Linq;

namespace SAHB.GraphQLClient
{
    public static class GraphQLResponseExtensions
    {
        public static bool HasData<TInput, TOutput, TInputRequest>(this IGraphQLResponse<TInput, TOutput, TInputRequest> response)
            where TInput : class
            where TOutput : class
            where TInputRequest : IGraphQLRequest<TInput>
        {
            if (response is null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            return response.Data != null;
        }

        public static bool HasErrors<TInput, TOutput, TInputRequest>(this IGraphQLResponse<TInput, TOutput, TInputRequest> response)
            where TInput : class
            where TOutput : class
            where TInputRequest : IGraphQLRequest<TInput>
        {
            if (response is null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            return response.Errors?.Any() ?? false;
        }

        public static bool HasAdditionalData<TInput, TOutput, TInputRequest>(this IGraphQLResponse<TInput, TOutput, TInputRequest> response)
           where TInput : class
           where TOutput : class
           where TInputRequest : IGraphQLRequest<TInput>
        {
            if (response is null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            return response.AdditionalData?.Any() ?? false;
        }
    }
}
