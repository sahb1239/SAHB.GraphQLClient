using System;
using System.Collections.Generic;
using System.Text;
using SAHB.GraphQLClient.FieldBuilder.Attributes;
using Xunit;

namespace SAHB.GraphQLClient.Tests.FieldBuilder.GraphQLArguments
{
    public class SpecificGraphQLAttributeArgumentTests
    {
        [Fact]
        public void Test_GraphQLFirstArgumentsAttribute_Name_First()
        {
            var argument = new GraphQLFirstArgumentsAttribute("variableName");

            Assert.Equal("first", argument.ArgumentName);
        }

        [Fact]
        public void Test_GraphQLFirstArgumentsAttribute_Type_Int()
        {
            var argument = new GraphQLFirstArgumentsAttribute("variableName");

            Assert.Equal("Int", argument.ArgumentType);
        }

        [Fact]
        public void Test_GraphQLLastArgumentsAttribute_Name_Last()
        {
            var argument = new GraphQLLastArgumentsAttribute("variableName");

            Assert.Equal("last", argument.ArgumentName);
        }

        [Fact]
        public void Test_GraphQLLastArgumentsAttribute_Type_Int()
        {
            var argument = new GraphQLLastArgumentsAttribute("variableName");

            Assert.Equal("Int", argument.ArgumentType);
        }

        [Fact]
        public void Test_GraphQLSkipArgumentsAttribute_Name_Skip()
        {
            var argument = new GraphQLSkipArgumentsAttribute("variableName");

            Assert.Equal("skip", argument.ArgumentName);
        }

        [Fact]
        public void Test_GraphQLSkipArgumentsAttribute_Type_Int()
        {
            var argument = new GraphQLSkipArgumentsAttribute("variableName");

            Assert.Equal("Int", argument.ArgumentType);
        }

        [Fact]
        public void Test_GraphQTakeArgumentsAttribute_Name_take()
        {
            var argument = new GraphQLTakeArgumentsAttribute("variableName");

            Assert.Equal("take", argument.ArgumentName);
        }

        [Fact]
        public void Test_GraphQLTakeArgumentsAttribute_Type_Int()
        {
            var argument = new GraphQLTakeArgumentsAttribute("variableName");

            Assert.Equal("Int", argument.ArgumentType);
        }
    }
}
