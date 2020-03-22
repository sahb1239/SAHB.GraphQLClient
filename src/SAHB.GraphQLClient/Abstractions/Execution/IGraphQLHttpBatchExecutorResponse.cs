namespace SAHB.GraphQLClient.Execution
{
    public interface IGraphQLHttpBatchExecutorResponse
    {
        IGraphQLHttpResponse<TInput, TOutput, IGraphQLBatchHttpRequest<TInput, TOutput>> GetResponse<TInput, TOutput>(IGraphQLBatchHttpRequest<TInput, TOutput> request)
            where TInput : class
            where TOutput : class;
    }
}
