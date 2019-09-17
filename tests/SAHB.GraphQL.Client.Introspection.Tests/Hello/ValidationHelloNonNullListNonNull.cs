using GraphQL.Types;
using SAHB.GraphQL.Client.Testserver.Schemas;
using SAHB.GraphQL.Client.TestServer;
using SAHB.GraphQLClient.Introspection;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SAHB.GraphQL.Client.Introspection.Tests.Hello
{
    public class ValidationHelloNonNullListNonNull : GenericHelloTest<ValidationHelloNonNullListNonNull.GraphQLQuery, ValidationHelloNonNullListNonNull.TestHelloQuery, ValidationHelloNonNullListNonNull.TestInvalidHelloQuery>
    {
        public ValidationHelloNonNullListNonNull(GraphQLWebApplicationFactory<GenericQuerySchema<GraphQLQuery>> factory) : base(factory)
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
            Assert.Equal(GraphQLTypeKind.NonNull, helloType.Type.Kind);
            Assert.Equal(GraphQLTypeKind.List, helloType.Type.OfType.Kind);
            Assert.Equal(GraphQLTypeKind.NonNull, helloType.Type.OfType.OfType.Kind);
            Assert.Equal("String", helloType.Type.OfType.OfType.OfType.Name);
            Assert.Equal(GraphQLTypeKind.Scalar, helloType.Type.OfType.OfType.OfType.Kind);
        }

        public class GraphQLQuery : ObjectGraphType
        {
            public GraphQLQuery()
            {
                Field<NonNullGraphType<ListGraphType<NonNullGraphType<StringGraphType>>>>("hello", resolve: context => new[] { "query" });
            }
        }

        public class TestHelloQuery
        {
            public List<string> Hello { get; set; }
        }

        public class TestInvalidHelloQuery
        {
            public List<string> NotHello { get; set; }
        }
    }
}
