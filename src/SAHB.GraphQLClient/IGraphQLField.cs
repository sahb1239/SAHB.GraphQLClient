using System;
using System.Collections.Generic;

namespace SAHB.GraphQLClient
{
    /// <summary>
    /// GraphQL field used to contain metadata which can be used for generating a GraphQL query
    /// </summary>
    public interface IGraphQLField
    {
        /// <summary>
        /// GraphQL field
        /// </summary>
        string Field { get; }

        /// <summary>
        /// GraphQL alias
        /// </summary>
        string Alias { get; }

        /// <summary>
        /// Arguments for the current field
        /// </summary>
        ICollection<IGraphQLArguments> Arguments { get; }

        /// <summary>
        /// The selection set for the field
        /// </summary>
        ICollection<IGraphQLField> SelectionSet { get; }

        /// <summary>
        /// Returns the deserilzation type which should be deserilized to if no match is found in <see cref="TargetTypes"/>
        /// </summary>
        Type BaseType { get; }

        /// <summary>
        /// Returns the type which should be deserilized to based on the __typename field
        /// </summary>
        IDictionary<string, IGraphQLTargetType> TargetTypes { get; }
    }
}