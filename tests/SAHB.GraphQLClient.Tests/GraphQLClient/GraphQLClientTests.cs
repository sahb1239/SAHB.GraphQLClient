using System;
using System.Collections.Generic;
using System.Text;
using FakeItEasy;
using SAHB.GraphQLClient;
using SAHB.GraphQL.Client.Introspection.Validation;
using SAHB.GraphQLClient.Deserialization;
using SAHB.GraphQLClient.Executor;
using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.Filtering;
using SAHB.GraphQLClient.QueryGenerator;
using Xunit;

namespace SAHB.GraphQLClient.Tests.GraphQLClient
{
    public class GraphQLClientTests
    {
        [Fact]
        public void InitilizeProperties()
        {
            // Arrange
            var fieldBuilder = A.Fake<IGraphQLFieldBuilder>(x => x.Strict());
            var httpExecutor = A.Fake<IGraphQLHttpExecutor>(x => x.Strict());
            var queryGenerator = A.Fake<IGraphQLQueryGeneratorFromFields>(x => x.Strict());
            var deserialization = A.Fake<IGraphQLDeserialization>(x => x.Strict());
            var filterGenerator = A.Fake<IQueryGeneratorFilter>(x => x.Strict());
            var validator = A.Fake<IGraphQLValidation>(x => x.Strict());

            // Act
            var client = new SAHB.GraphQLClient.GraphQLClient(fieldBuilder, httpExecutor, 
                queryGenerator, deserialization, filterGenerator, validator);

            // Assert
            Assert.Equal(fieldBuilder, client.FieldBuilder);
            Assert.Equal(httpExecutor, client.HttpExecutor);
            Assert.Equal(queryGenerator, client.QueryGenerator);
            Assert.Equal(deserialization, client.Deserialization);
            Assert.Equal(filterGenerator, client.FilterGenerator);
            Assert.Equal(validator, client.Validator);
        }

        [Fact]
        public void Throws_If_FieldBuilder_IsNull()
        {
            // Arrange
            IGraphQLFieldBuilder fieldBuilder = null;
            var httpExecutor = A.Fake<IGraphQLHttpExecutor>(x => x.Strict());
            var queryGenerator = A.Fake<IGraphQLQueryGeneratorFromFields>(x => x.Strict());
            var deserialization = A.Fake<IGraphQLDeserialization>(x => x.Strict());
            var filterGenerator = A.Fake<IQueryGeneratorFilter>(x => x.Strict());
            var validator = A.Fake<IGraphQLValidation>(x => x.Strict());

            // Act / Assert
            var exception = Assert.Throws<ArgumentNullException>(() => 
                new SAHB.GraphQLClient.GraphQLClient(fieldBuilder, httpExecutor,
                    queryGenerator, deserialization, filterGenerator, validator));

            Assert.Equal("fieldBuilder", exception.ParamName);
            Assert.Equal($"Value cannot be null.{Environment.NewLine}Parameter name: fieldBuilder", exception.Message);
        }

        [Fact]
        public void Throws_If_HttpExecutor_IsNull()
        {
            // Arrange
            var fieldBuilder = A.Fake<IGraphQLFieldBuilder>(x => x.Strict());
            IGraphQLHttpExecutor httpExecutor = null;
            var queryGenerator = A.Fake<IGraphQLQueryGeneratorFromFields>(x => x.Strict());
            var deserialization = A.Fake<IGraphQLDeserialization>(x => x.Strict());
            var filterGenerator = A.Fake<IQueryGeneratorFilter>(x => x.Strict());
            var validator = A.Fake<IGraphQLValidation>(x => x.Strict());

            // Act / Assert
            var exception = Assert.Throws<ArgumentNullException>(() =>
                new SAHB.GraphQLClient.GraphQLClient(fieldBuilder, httpExecutor,
                    queryGenerator, deserialization, filterGenerator, validator));

            Assert.Equal("httpExecutor", exception.ParamName);
            Assert.Equal($"Value cannot be null.{Environment.NewLine}Parameter name: httpExecutor", exception.Message);
        }

        [Fact]
        public void Throws_If_QueryGenerator_IsNull()
        {
            // Arrange
            var fieldBuilder = A.Fake<IGraphQLFieldBuilder>(x => x.Strict());
            var httpExecutor = A.Fake<IGraphQLHttpExecutor>(x => x.Strict());
            IGraphQLQueryGeneratorFromFields queryGenerator = null;
            var deserialization = A.Fake<IGraphQLDeserialization>(x => x.Strict());
            var filterGenerator = A.Fake<IQueryGeneratorFilter>(x => x.Strict());
            var validator = A.Fake<IGraphQLValidation>(x => x.Strict());

            // Act / Assert
            var exception = Assert.Throws<ArgumentNullException>(() =>
                new SAHB.GraphQLClient.GraphQLClient(fieldBuilder, httpExecutor,
                    queryGenerator, deserialization, filterGenerator, validator));

            Assert.Equal("queryGenerator", exception.ParamName);
            Assert.Equal($"Value cannot be null.{Environment.NewLine}Parameter name: queryGenerator", exception.Message);
        }

        [Fact]
        public void Throws_If_deserialization_IsNull()
        {
            // Arrange
            var fieldBuilder = A.Fake<IGraphQLFieldBuilder>(x => x.Strict());
            var httpExecutor = A.Fake<IGraphQLHttpExecutor>(x => x.Strict());
            var queryGenerator = A.Fake<IGraphQLQueryGeneratorFromFields>(x => x.Strict());
            IGraphQLDeserialization deserialization = null;
            var filterGenerator = A.Fake<IQueryGeneratorFilter>(x => x.Strict());
            var validator = A.Fake<IGraphQLValidation>(x => x.Strict());

            // Act / Assert
            var exception = Assert.Throws<ArgumentNullException>(() =>
                new SAHB.GraphQLClient.GraphQLClient(fieldBuilder, httpExecutor,
                    queryGenerator, deserialization, filterGenerator, validator));

            Assert.Equal("deserialization", exception.ParamName);
            Assert.Equal($"Value cannot be null.{Environment.NewLine}Parameter name: deserialization", exception.Message);
        }

        [Fact]
        public void Throws_If_FilterGenerator_IsNull()
        {
            // Arrange
            var fieldBuilder = A.Fake<IGraphQLFieldBuilder>(x => x.Strict());
            var httpExecutor = A.Fake<IGraphQLHttpExecutor>(x => x.Strict());
            var queryGenerator = A.Fake<IGraphQLQueryGeneratorFromFields>(x => x.Strict());
            var deserialization = A.Fake<IGraphQLDeserialization>(x => x.Strict());
            IQueryGeneratorFilter filterGenerator = null;
            var validator = A.Fake<IGraphQLValidation>(x => x.Strict());

            // Act / Assert
            var exception = Assert.Throws<ArgumentNullException>(() =>
                new SAHB.GraphQLClient.GraphQLClient(fieldBuilder, httpExecutor,
                    queryGenerator, deserialization, filterGenerator, validator));

            Assert.Equal("filterGenerator", exception.ParamName);
            Assert.Equal($"Value cannot be null.{Environment.NewLine}Parameter name: filterGenerator", exception.Message);
        }

        [Fact]
        public void Throws_If_Validator_IsNull()
        {
            // Arrange
            var fieldBuilder = A.Fake<IGraphQLFieldBuilder>(x => x.Strict());
            var httpExecutor = A.Fake<IGraphQLHttpExecutor>(x => x.Strict());
            var queryGenerator = A.Fake<IGraphQLQueryGeneratorFromFields>(x => x.Strict());
            var deserialization = A.Fake<IGraphQLDeserialization>(x => x.Strict());
            var filterGenerator = A.Fake<IQueryGeneratorFilter>(x => x.Strict());
            IGraphQLValidation validator = null;

            // Act / Assert
            var exception = Assert.Throws<ArgumentNullException>(() =>
                new SAHB.GraphQLClient.GraphQLClient(fieldBuilder, httpExecutor,
                    queryGenerator, deserialization, filterGenerator, validator));

            Assert.Equal("validator", exception.ParamName);
            Assert.Equal($"Value cannot be null.{Environment.NewLine}Parameter name: validator", exception.Message);
        }
    }
}
