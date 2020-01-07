using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using SAHB.GraphQLClient;
using SAHB.GraphQLClient.Deserialization;
using SAHB.GraphQLClient.Executor;
using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.Filtering;
using SAHB.GraphQLClient.QueryGenerator;
using Xunit;

namespace SAHB.GraphQL.Client.Tests.GraphQLClient
{
    public class GraphQLClientCallTest
    {
        [Fact]
        public void Default_Creates_New_GraphQLClient()
        {
            // Arrange / Act
            var actual = GraphQLHttpClient.Default();

            // Assert
            Assert.NotNull(actual);
            Assert.NotNull(actual.FieldBuilder);
            Assert.NotNull(actual.HttpExecutor);
            Assert.NotNull(actual.QueryGenerator);
            Assert.NotNull(actual.Deserialization);
            Assert.NotNull(actual.FilterGenerator);
        }

        [Fact]
        public void Default_WithClient_Creates_New_GraphQLClient()
        {
            // Arrange
            var client = new HttpClient() { BaseAddress = new Uri("https://google.com/") };

            // Act
            var actual = GraphQLHttpClient.Default(client);

            // Assert
            Assert.Equal(client, actual.HttpExecutor.Client);
            Assert.Equal(client.BaseAddress.OriginalString, actual.HttpExecutor.Client.BaseAddress.OriginalString);
        }

        [Fact]
        public async Task Execute_With_OperationType_Calls_FieldBuilder_Correct()
        {
            // Arrange
            var executor = A.Fake<IGraphQLHttpExecutor>();
            var fieldBuilder = A.Fake<IGraphQLFieldBuilder>(x => x.Strict());
            var queryGenerator = A.Fake<IGraphQLQueryGeneratorFromFields>();
            var deserilization = A.Fake<IGraphQLDeserialization>();
            var filterGenerator = A.Fake<IQueryGeneratorFilter>();

            A.CallTo(() => fieldBuilder.GenerateSelectionSet(typeof(Query))).Returns(new List<GraphQLField>
            {
                new GraphQLField("hello", "hello", null, null)
            });

            var client = new GraphQLHttpClient(executor, fieldBuilder, queryGenerator, deserilization, filterGenerator);

            // Act
            var actual = await client.Execute<Query>(GraphQLOperationType.Query, "Some url", HttpMethod.Head, new Dictionary<string, string> {
                { "header1", "value1" } }, e => new Query { Hello = "hello world" }, "token", "authorizationMethod");

            // Assert
            A.CallTo(() => fieldBuilder.GenerateSelectionSet(typeof(Query))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task Execute_With_OperationType_Calls_QueryGenerator_Correct()
        {
            // Arrange
            var executor = A.Fake<IGraphQLHttpExecutor>();
            var fieldBuilder = A.Fake<IGraphQLFieldBuilder>(x => x.Strict());
            var queryGenerator = A.Fake<IGraphQLQueryGeneratorFromFields>();
            var deserilization = A.Fake<IGraphQLDeserialization>();
            var filterGenerator = A.Fake<IQueryGeneratorFilter>();

            A.CallTo(() => fieldBuilder.GenerateSelectionSet(typeof(Query))).Returns(new List<GraphQLField>
            {
                new GraphQLField("hello", "world", null, null)
            });
            A.CallTo(() => queryGenerator.GenerateQuery(GraphQLOperationType.Query, A<IEnumerable<GraphQLField>>.Ignored, (Func<GraphQLField, bool>)null))
                .Returns("some query");
            A.CallTo(() => filterGenerator.GetFilter(A<Expression<Func<Query, Query>>>.Ignored)).Returns(null);

            var client = new GraphQLHttpClient(executor, fieldBuilder, queryGenerator, deserilization, filterGenerator);

            // Act
            var actual = await client.Execute<Query>(GraphQLOperationType.Query, "Some url", HttpMethod.Head, new Dictionary<string, string> {
                { "header1", "value1" } }, e => new Query { Hello = "hello world" }, "token", "authorizationMethod");

            // Assert
            A.CallTo(() => queryGenerator.GenerateQuery(GraphQLOperationType.Query,
                A<IEnumerable<GraphQLField>>.That.Matches(e => e.Single().Alias == "hello" && e.Single().Field == "world"),
                A<Func<GraphQLField, bool>>.That.IsNull(), A<GraphQLQueryArgument[]>.That.Matches(e => e.Length == 0))).MustHaveHappenedOnceExactly();
        }

        private class Query
        {
            public string Hello { get; set; }
        }
    }
}
