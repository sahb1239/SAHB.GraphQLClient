using GraphQL.Types;
using SAHB.GraphQL.Client.Introspection.Validation;
using SAHB.GraphQL.Client.TestServer;
using SAHB.GraphQLClient;
using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.FieldBuilder.Attributes;
using SAHB.GraphQLClient.Introspection;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Xunit;

namespace SAHB.GraphQL.Client.Introspection.Tests.Issues
{
    public class Issue80 : IClassFixture<GraphQLWebApplicationFactory<Issue80Schema>>
    {
        private readonly GraphQLWebApplicationFactory<Issue80Schema> _factory;

        public Issue80(GraphQLWebApplicationFactory<Issue80Schema> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Validate_Query_IsValid()
        {
            // Arrange
            var client = _factory.CreateClient();
            var graphQLClient = GraphQLHttpClient.Default(client);

            // Act
            var introspectionQuery = await graphQLClient.CreateQuery<GraphQLIntrospectionQuery>("http://localhost/graphql").Execute();
            var validationOutput = introspectionQuery.ValidateGraphQLType<EmQuery>(GraphQLOperationType.Query);

            // Assert
            Assert.Empty(validationOutput);
        }
    }

    public class EmQuery
    {
        [GraphQLFieldName("qryRdByEm")]
        [GraphQLArguments("src", "RdSrc!", "fromSrc", IsRequired = true)]
        [GraphQLArguments("em", "String", "fromEm")]
        public IEnumerable<string> QryRdByEm { get; set; }
    }

    public enum RdSrc
    {
        [EnumMember(Value = "SB")]
        SB,
        [EnumMember(Value = "XB")]
        XB
    }

    public class Issue80Schema : Schema
    {
        public Issue80Schema()
        {
            Query = new GraphQLQuery();
        }

        private class RdSrcGraphType : EnumerationGraphType<RdSrc>
        {
            public RdSrcGraphType()
            {
                Name = "RdSrc";
            }
        }

        private class GraphQLQuery : ObjectGraphType
        {
            public GraphQLQuery()
            {
                Field<ListGraphType<StringGraphType>>(
                    "qryRdByEm",
                    resolve: context => new List<string>(),
                    arguments: new QueryArguments(
                        new QueryArgument<NonNullGraphType<RdSrcGraphType>>
                        {
                            Name = "src"
                        },
                        new QueryArgument<StringGraphType>
                        {
                            Name = "em"
                        }
                    ));
            }
        }
    }
}
