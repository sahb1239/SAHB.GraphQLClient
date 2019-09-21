using GraphQL.Types;
using SAHB.GraphQL.Client.Testserver.Schemas;
using SAHB.GraphQL.Client.TestServer;
using SAHB.GraphQLClient.Introspection;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SAHB.GraphQL.Client.Introspection.Tests.Hello
{
    public class ValidationHello : GenericHelloTest<ValidationHello.GraphQLQuery, ValidationHello.TestHelloQuery, ValidationHello.TestInvalidHelloQuery>
    {
        public ValidationHello(GraphQLWebApplicationFactory<GenericQuerySchema<GraphQLQuery>> factory) : base(factory)
        {
        }

        [Fact]
        public async Task Hello_Has_Correct_Type()
        {
            // Arrange
            var graphQLClient = this.GraphQLHttpClient;

            // Act
            var introspectionQuery = await graphQLClient.CreateQuery<GraphQLIntrospectionQuery>("http://localhost/graphql").Execute();

            // Assert
            var queryType = introspectionQuery.Schema.Types.Single(type => type.Name == introspectionQuery.Schema.QueryType.Name);
            var helloType = queryType.Fields.Single(field => field.Name == "hello");
            Assert.Equal("String", helloType.Type.Name);
            Assert.Equal(GraphQLTypeKind.Scalar, helloType.Type.Kind);
        }

        public class GraphQLQuery : ObjectGraphType
        {
            public GraphQLQuery()
            {
                Field<StringGraphType>("hello", resolve: context => "query");
            }
        }

        public class TestHelloQuery
        {
            public string Hello { get; set; }
        }

        public class TestInvalidHelloQuery
        {
            public string NotHello { get; set; }
        }
    }
}
