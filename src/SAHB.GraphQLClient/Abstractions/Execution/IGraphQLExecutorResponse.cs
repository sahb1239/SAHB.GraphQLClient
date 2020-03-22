using System;
using SAHB.GraphQLClient.Result;

namespace SAHB.GraphQLClient.Execution
{
    public interface IGraphQLExecutorResponse<TInput, TOutput> 
        where TInput : class
        where TOutput : class
    {
        Exception DeserilizationException { get; }
        GraphQLDataResult<TInput> InputDeserilizedResponse { get; }
        GraphQLDataResult<TOutput> OutputDeserilizedResponse { get; }
        string Query { get; }
        string Response { get; }
    }
}
