using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using SAHB.GraphQLClient.Exceptions;

namespace SAHB.GraphQL.Client.Exceptions
{
    /// <summary>
    /// Exception thrown when duplicate type names in interface or union attributes
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class GraphQLDuplicateTypeNameException : GraphQLException
    {
        public IEnumerable<string> DuplicateNames { get; }

        public GraphQLDuplicateTypeNameException(params string[] duplicateNames) : base($"Duplicate union or interface type names found:{Environment.NewLine}{string.Join(Environment.NewLine, duplicateNames)}")
        {
            DuplicateNames = new ReadOnlyCollection<string>(duplicateNames.ToList());
        }
    }
}
