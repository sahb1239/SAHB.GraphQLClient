using Newtonsoft.Json;
using SAHB.GraphQLClient.Deserialization;
using System.Linq;
using Xunit;

namespace SAHB.GraphQL.Client.Tests.Deserialization
{
    public class AdditionalDataTest
    {
        [Fact]
        public void GraphQLDataResultAdditionalData()
        {
            // Arrange
            var deserilizer = new GraphQLDeserilization();
            var jsonToDeserilize = JsonConvert.SerializeObject(new
            {
                Data = new
                {
                    Field = "FieldValue"
                },
                Extentions = new
                {
                    Data = nameof(GraphQLDataResultAdditionalData)
                }
            });

            // Act
            var dataResult = deserilizer.DeserializeResult<dynamic>(jsonToDeserilize, null);

            // Assert
            var extentions = dataResult.AdditionalData["Extentions"];
            Assert.Equal(extentions.Value<string>("Data"), nameof(GraphQLDataResultAdditionalData));
        }

        [Fact]
        public void GraphQLDataErrorAdditionalData()
        {
            // Arrange
            var deserilizer = new GraphQLDeserilization();
            var jsonToDeserilize = JsonConvert.SerializeObject(new
            {
                Errors = new[]
                {
                    new {
                        Extentions = new
                        {
                            Data = nameof(GraphQLDataErrorAdditionalData)
                        }
                    }
                }
            });

            // Act
            var dataResult = deserilizer.DeserializeResult<dynamic>(jsonToDeserilize, null);

            // Assert
            var extentions = dataResult.Errors.Single().AdditionalData["Extentions"];
            Assert.Equal(extentions.Value<string>("Data"), nameof(GraphQLDataErrorAdditionalData));
        }

        public void GraphQLDataErrorLocationAdditionalData()
        {
            // Arrange
            var deserilizer = new GraphQLDeserilization();
            var jsonToDeserilize = JsonConvert.SerializeObject(new
            {
                Errors = new[]
                {
                    new {
                        Locations = new []
                        {
                            new
                            {
                                Extentions = new
                                {
                                    Data = nameof(GraphQLDataErrorLocationAdditionalData)
                                }
                            }
                        }
                    }
                }
            });

            // Act
            var dataResult = deserilizer.DeserializeResult<dynamic>(jsonToDeserilize, null);

            // Assert
            var extentions = dataResult.Errors.Single().Locations.Single().AdditionalData["Extentions"];
            Assert.Equal(extentions.Value<string>("Data"), nameof(GraphQLDataErrorLocationAdditionalData));
        }
    }
}
