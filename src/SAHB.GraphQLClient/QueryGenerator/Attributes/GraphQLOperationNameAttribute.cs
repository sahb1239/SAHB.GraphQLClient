using System;

namespace SAHB.GraphQLClient.QueryGenerator.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class GraphQLOperationNameAttribute : Attribute
    {
        /// <summary>
        /// Initializes an attribute that describes the corresponding GraphQL operation name for the attributed class
        /// </summary>
        /// <param name="operationName">The operation name used in the GraphQL query</param>
        public GraphQLOperationNameAttribute(string operationName)
        {
            OperationName = operationName;
        }
        
        public string OperationName { get; set; }
    }
}
