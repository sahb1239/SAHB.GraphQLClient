using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using SAHB.GraphQL.Client.Introspection.Validation;
using SAHB.GraphQLClient.Builder;
using SAHB.GraphQLClient.Deserialization;
using SAHB.GraphQLClient.Executor;
using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.FieldBuilder.Attributes;
using SAHB.GraphQLClient.Filtering;
using SAHB.GraphQLClient.QueryGenerator;

namespace SAHB.GraphQLClient
{
    public interface IGraphQLClient
    {
        /// <summary>
        /// The <see cref="IGraphQLFieldBuilder"/> used
        /// </summary>
        IGraphQLFieldBuilder FieldBuilder { get; }

        /// <summary>
        /// The <see cref="IGraphQLHttpExecutor"/> used
        /// </summary>
        IGraphQLHttpExecutor HttpExecutor { get; }

        /// <summary>
        /// The <see cref="IGraphQLQueryGeneratorFromFields"/> used
        /// </summary>
        IGraphQLQueryGeneratorFromFields QueryGenerator { get; }

        /// <summary>
        /// The <see cref="IGraphQLDeserialization"/> used
        /// </summary>
        IGraphQLDeserialization Deserialization { get; }

        /// <summary>
        /// The <see cref="IQueryGeneratorFilter"/>
        /// </summary>
        IQueryGeneratorFilter FilterGenerator { get; }

        /// <summary>
        /// The <see cref="IGraphQLValidation"/>
        /// </summary>
        IGraphQLValidation Validator { get; set; }

        IGraphQLHttpRequest<T> CreateHttpRequest<T>(GraphQLOperationType operationType) where T : class;
        IGraphQLHttpRequest<dynamic> CreateHttpRequest(GraphQLOperationType operationType, Action<IGraphQLBuilder> queryBuilder);
        IGraphQLBatchRequest CreateBatchRequest(GraphQLOperationType operationType);
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
