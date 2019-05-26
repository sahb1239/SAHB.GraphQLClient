using System;
using System.Collections.Generic;
using System.Text;

namespace SAHB.GraphQLClient.FieldBuilder.Attributes
{ 
    /// <summary>
    /// Attribute which defines that another class should be deserilized based on the __typename on a GraphQL result. This is useful for handling union or interface GraphQL types
    /// </summary>    
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = true)]
    public class GraphQLUnionOrInterfaceAttribute : Attribute
    {
        /// <summary>
        /// The name of the type returned in the __typename GraphQL field
        /// </summary>
        public string TypeName { get; }

        /// <summary>
        /// The type which should be initilized when the __typename field is equal to <see cref="TypeName"/>
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// Initilizes a new instance of the <see cref="GraphQLUnionOrInterfaceAttribute"/>
        /// </summary>
        /// <param name="typeName">The name of the type returned in the __typename GraphQL field</param>
        /// <param name="type">The type which should be initilized when the __typename field is equal to <paramref name="typeName"/></param>
        public GraphQLUnionOrInterfaceAttribute(string typeName, Type type)
        {
            TypeName = typeName;
            Type = type;
        }
    }
}
