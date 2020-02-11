using SAHB.GraphQLClient.Exceptions;
using SAHB.GraphQLClient.FieldBuilder;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SAHB.GraphQL.Client.Tests.FieldBuilder.CircularReference
{
    public class TestCircularReference
    {
        private readonly IGraphQLFieldBuilder _fieldBuilder;

        public TestCircularReference()
        {
            _fieldBuilder = new GraphQLFieldBuilder();
        }

        [Fact]
        public void Should_Throw_Exception_When_Circular_Reference_IsFound()
        {
            // Arrange / Act / Assert
            Assert.Throws<GraphQLCircularReferenceException>(() =>
                _fieldBuilder.GenerateSelectionSet(typeof(Hello)));
        }

        public class Hello
        {
            public Hello SayHello { get; set; }
        }
    }
}
