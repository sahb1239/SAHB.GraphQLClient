using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SAHB.GraphQL.Client.Introspection.Validation;
using SAHB.GraphQLClient.Builder;
using SAHB.GraphQLClient.Execution;
using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.FieldBuilder.Attributes;
using SAHB.GraphQLClient.Filtering;

namespace SAHB.GraphQLClient
{
    public interface IGraphQLClient
    {
        /// <summary>
        /// The <see cref="IGraphQLFieldBuilder"/> used
        /// </summary>
        IGraphQLFieldBuilder FieldBuilder { get; }

        /// <summary>
        /// The <see cref="IQueryGeneratorFilter"/>
        /// </summary>
        IQueryGeneratorFilter FilterGenerator { get; }

        /// <summary>
        /// The <see cref="IGraphQLValidation"/>
        /// </summary>
        IGraphQLValidation Validator { get; set; }
        
        /// <summary>
        /// The <see cref="IGraphQLExecutor"/>
        /// </summary>
        IGraphQLExecutor Executor { get; set; }

        IGraphQLBatchRequest CreateBatchRequest(GraphQLOperationType operationType);
        IGraphQLHttpRequest<T> CreateHttpRequest<T>(GraphQLOperationType operationType) where T : class;
        IGraphQLHttpRequest<dynamic> CreateHttpRequest(GraphQLOperationType operationType, Action<IGraphQLBuilder> queryBuilder);
        IGraphQLSubscriptionRequest<T> CreateSubscriptionRequest<T>(GraphQLOperationType operationType) where T : class;
        IGraphQLSubscriptionRequest<dynamic> CreateSubscriptionRequest(GraphQLOperationType operationType, Action<IGraphQLBuilder> queryBuilder);
    }

    public class Test
    {
        public static async Task Test2(IGraphQLClient client)
        {
            var data = await client.Query<Query>()
                .AddArgument(Query.CountArgument, 10)
                .AddHeader("hello", "world")
                .SetShouldThrowIfQueryIsInvalid(true)
                .GetData();

            var data2 = await client.Query(builder =>
                builder
                    .Field("hello",
                        hello =>
                            hello
                                .Argument(Query.CountArgument, "Int!", "count")
                                .Field("item1")
                                .Field("item2")))
                .AddArgument(Query.CountArgument, 10)
                .AddHeader("hello", "world")
                .SetShouldThrowIfQueryIsInvalid(true)
                .GetData();

            var data3 = await client.Query<Query>()
                .AddArgument(Query.CountArgument, 10)
                .AddHeader("hello", "world")
                .SetShouldThrowIfQueryIsInvalid(true)
                .SetUrl("https://google.com/graphql")
                .GetData(e => e.Hello.Select(x => x.Item1));

            var batch = client.QueryBatch()
                .SetUrl("https://google.com/graphql")
                .AddHeader("hello", "world");

            var result1 = batch.CreateHttpRequest<Query>()
                .AddArgument(Query.CountArgument, 10)
                .SetShouldThrowIfQueryIsInvalid(true)
                .GetData()
                .ContinueWith(task => DoSomething(task.Result.Hello), TaskContinuationOptions.OnlyOnRanToCompletion);

            var result2 = batch.CreateHttpRequest<Query>()
                .AddArgument(Query.CountArgument, 10)
                .SetShouldThrowIfQueryIsInvalid(true)
                .GetData()
                .ContinueWith(task => DoSomething(task.Result.Hello), TaskContinuationOptions.OnlyOnRanToCompletion);

            await batch.Execute();
        }

        private static void DoSomething(List<Query.HelloType> hello)
        {
            throw new NotImplementedException();
        }

        public class Query
        {
            public const string CountArgument = "count";

            [GraphQLArguments("count", "Int!", CountArgument)]
            public List<HelloType> Hello { get; set; }

            public class HelloType
            {
                public string Item1 { get; set; }
                public string Item2 { get; set; }
            }
        }
    }
}
