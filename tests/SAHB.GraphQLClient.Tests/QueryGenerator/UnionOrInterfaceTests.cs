using SAHB.GraphQLClient;
using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.QueryGenerator;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SAHB.GraphQLClient.Tests.QueryGenerator
{
    public class UnionOrInterfaceTests
    {
        [Fact]
        public void Test_Single_Other_Possible_Type_With_Same_Fields()
        {
            var fields = new[]
            {
                new GraphQLField(
                    "alias",
                    "field",
                    null,
                    null,
                    null,
                    new Dictionary<string, GraphQLTargetType> {
                        {
                            "interfaceConcreteType",
                            new GraphQLTargetType(typeof(string), new List<GraphQLField>() {
                                new GraphQLField("alias", "field", null, null, null, null)
                                })
                        }
                    }
                )
            };
            var queryGenerator = new GraphQLQueryGeneratorFromFields();
            var expected = "{\"query\":\"query{alias:field ... on interfaceConcreteType{alias:field}}\"}";

            var actual = queryGenerator.GenerateQuery(GraphQLOperationType.Query, fields);

            Assert.Equal(expected, actual);
        }
        
        [Fact]
        public void Test_Single_Other_Possible_Type_With_Extra_Field()
        {
            var fields = new GraphQLField[]
            {
                new GraphQLField("alias", "field",  null, null, null,
                    new Dictionary<string, GraphQLTargetType>
                    {
                        {
                            "interfaceConcreteType",
                            new GraphQLTargetType
                            (
                                typeof(string),
                                new List<GraphQLField>
                                {
                                    new GraphQLField("alias", "field", null, null),
                                    new GraphQLField("alias2", "field2", null, null)
                                }
                            )
                        }
                    }),
            };
            var queryGenerator = new GraphQLQueryGeneratorFromFields();
            var expected = "{\"query\":\"query{alias:field ... on interfaceConcreteType{alias:field alias2:field2}}\"}";

            var actual = queryGenerator.GenerateQuery(GraphQLOperationType.Query, fields);

            Assert.Equal(expected, actual);
        }
        
        [Fact]
        public void Test_Multiple_Other_Possible_Type_With_Same_Field()
        {
            var fields = new[]
            {
                new GraphQLField("alias", "field", null, null, null,
                     new Dictionary<string, GraphQLTargetType>
                    {
                        {
                            "interfaceConcreteType1",
                            new GraphQLTargetType
                            (
                                typeof(string),
                                new List<GraphQLField>
                                {
                                    new GraphQLField("alias", "field", null, null, null, null)
                                }
                            )
                        },
                        {

                             "interfaceConcreteType2",
                             new GraphQLTargetType
                            (
                                typeof(int),
                                new List<GraphQLField>
                                {
                                    new GraphQLField("alias", "field", null, null, null, null)
                                }
                            )

                        }
                     }
                    )
            };
            var queryGenerator = new GraphQLQueryGeneratorFromFields();
            var expected = "{\"query\":\"query{alias:field ... on interfaceConcreteType1{alias:field} ... on interfaceConcreteType2{alias:field}}\"}";

            var actual = queryGenerator.GenerateQuery(GraphQLOperationType.Query, fields);

            Assert.Equal(expected, actual);
        }
        
        [Fact]
        public void Test_Multiple_Other_Possible_Type_With_Extra_Field()
        {
            var fields = new[]
           {
                new GraphQLField("alias", "field", null, null, null,
                     new Dictionary<string, GraphQLTargetType>
                    {
                        {
                            "interfaceConcreteType1",
                            new GraphQLTargetType
                            (
                                typeof(string),
                                new List<GraphQLField>
                                {
                                    new GraphQLField("alias", "field", null, null, null, null),
                                    new GraphQLField("alias2", "field2", null, null, null, null)
                                }
                            )
                        },
                        {

                             "interfaceConcreteType2",
                             new GraphQLTargetType
                            (
                                typeof(int),
                                new List<GraphQLField>
                                {
                                    new GraphQLField("alias", "field", null, null, null, null),
                                    new GraphQLField("alias2", "field2", null, null, null, null)
                                }
                            )

                        }
                     }
                    )
            };
            var queryGenerator = new GraphQLQueryGeneratorFromFields();
            var expected = "{\"query\":\"query{alias:field ... on interfaceConcreteType1{alias:field alias2:field2} ... on interfaceConcreteType2{alias:field alias2:field2}}\"}";

            var actual = queryGenerator.GenerateQuery(GraphQLOperationType.Query, fields);

            Assert.Equal(expected, actual);
        }
    }
}
