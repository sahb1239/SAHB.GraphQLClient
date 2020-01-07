using System.Linq;
using SAHB.GraphQLClient.FieldBuilder;
using Xunit;

namespace SAHB.GraphQLClient.Tests.FieldBuilder.Path
{
    public class PathTest
    {
        private readonly IGraphQLFieldBuilder _fieldBuilder;

        public PathTest()
        {
            _fieldBuilder = new GraphQLFieldBuilder();
        }

        [Fact]
        public void Test_Path_Field()
        {
            // Arrange / Act
            var fields = _fieldBuilder.GenerateSelectionSet(typeof(HelloQuery)).ToList();

            // Assert
            Assert.Single(fields);

            // Get helloField
            var helloField = fields.First();
            Assert.Equal("Hello", helloField.Path);

            // Get Field
            Assert.Single(helloField.SelectionSet);
            Assert.Equal("Hello.Field", helloField.SelectionSet.First().Path);
        }

        [Fact]
        public void Test_Parent_Path()
        {
            // Arrange / Act
            var fields = _fieldBuilder.GenerateSelectionSet(typeof(HelloQuery)).ToList();

            // Assert
            Assert.Single(fields);

            // Get helloField
            var helloField = fields.First();
            Assert.Equal(null, helloField.ParentPath);

            // Get Field
            Assert.Single(helloField.SelectionSet);
            Assert.Equal("Hello", helloField.SelectionSet.First().ParentPath);
        }

        public class HelloQuery
        {
            public HelloSelectionSet Hello { get; set; }
        }

        public class HelloSelectionSet
        {
            public string Field { get; set; }
        }
    }
}
