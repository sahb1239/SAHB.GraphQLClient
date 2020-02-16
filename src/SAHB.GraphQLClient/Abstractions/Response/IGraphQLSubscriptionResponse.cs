namespace SAHB.GraphQLClient
{
    public interface IGraphQLSubscriptionResponse<TInput, TOutput>
        : IGraphQLResponse<TInput, TOutput, IGraphQLSubscriptionRequest<TInput>>
        where TInput : class
        where TOutput : class
    {
    }
}
