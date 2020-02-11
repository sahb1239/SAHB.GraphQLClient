using GraphQL.Types;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SAHB.GraphQL.Client.TestServer;
using SAHB.GraphQLClient;
using SAHB.GraphQLClient.FieldBuilder.Attributes;
using SAHB.GraphQLClient.QueryGenerator;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Xunit;

namespace SAHB.GraphQL.Client.Integration.Tests.Issues
{
    public class Issue77 : IClassFixture<GraphQLWebApplicationFactory<Issue77Schema>>
    {
        private readonly GraphQLWebApplicationFactory<Issue77Schema> _factory;

        public Issue77(GraphQLWebApplicationFactory<Issue77Schema> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Can_Get_Result()
        {
            // Arrange
            var client = _factory.CreateClient();
            var graphQLClient = GraphQLHttpClient.Default(client);

            // Act
            var query = await graphQLClient.CreateQuery<EmQuery>("http://localhost/graphql", arguments: new GraphQLQueryArgument("fromSrc", RdSrc.XB)).Execute();

            // Assert
            Assert.Single(query.QryRdByEm);
        }

        [Fact]
        public async Task Can_Get_Result_Without_Inline()
        {
            // Arrange
            var client = _factory.CreateClient();
            var graphQLClient = GraphQLHttpClient.Default(client);

            // Act
            var query = await graphQLClient.CreateQuery<EmQueryWUInline>("http://localhost/graphql", arguments: new GraphQLQueryArgument("fromSrc", RdSrc.XB)).Execute();

            // Assert
            Assert.Single(query.QryRdByEm);
        }

        [Fact]
        public async Task Can_Get_Result_Without_Enummember()
        {
            // Arrange
            var client = _factory.CreateClient();
            var graphQLClient = GraphQLHttpClient.Default(client);

            // Act
            var query = await graphQLClient.CreateQuery<EmQuery>("http://localhost/graphql", arguments: new GraphQLQueryArgument("fromSrc", RdSrcWithoutEnumMember.XB)).Execute();

            // Assert
            Assert.Single(query.QryRdByEm);
        }

        [Fact]
        public async Task Can_Get_Result_With_Enummember()
        {
            // Arrange
            var client = _factory.CreateClient();
            var graphQLClient = GraphQLHttpClient.Default(client);

            // Act
            var query = await graphQLClient.CreateQuery<EmQuery>("http://localhost/graphql", arguments: new GraphQLQueryArgument("fromSrc", RdSrcWithEnumMember.correctValue)).Execute();

            // Assert
            Assert.Single(query.QryRdByEm);
        }
    }

    public class EmQuery
    {
        [GraphQLFieldName("qryRdByEm")]
        [GraphQLArguments("src", "RdSrc!", "fromSrc", inlineArgument: true, isRequired: true)]
        public IEnumerable<string> QryRdByEm { get; set; }
    }

    public class EmQueryWUInline
    {
        [GraphQLFieldName("qryRdByEm")]
        [GraphQLArguments("src", "RdSrc!", "fromSrc", inlineArgument: false, isRequired: true)]
        public IEnumerable<string> QryRdByEm { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum RdSrc
    {
        [EnumMember(Value = "SB")]
        SB,
        [EnumMember(Value = "XB")]
        XB
    }

    public enum RdSrcWithoutEnumMember
    {
        SB,
        XB
    }

    public enum RdSrcWithEnumMember
    {
        [EnumMember(Value = "SB")]
        otherValue,
        [EnumMember(Value = "XB")]
        correctValue
    }

    public class Issue77Schema : Schema
    {
        public Issue77Schema()
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
                    resolve: context =>
                    {
                        if (context.GetArgument<RdSrc>("src", RdSrc.SB) == RdSrc.SB)
                        {
                            throw new NotSupportedException();
                        }
                        return new List<string>
                        {
                            "Element 1"
                        };
                    },
                    arguments: new QueryArguments(
                        new QueryArgument<NonNullGraphType<RdSrcGraphType>>
                        {
                            Name = "src"
                        }
                    ));
            }
        }
    }
}
