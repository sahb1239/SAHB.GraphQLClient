using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SAHB.GraphQLClient.Exceptions
{ 
    // ReSharper disable once InconsistentNaming
    /// <summary>
    /// Exception thrown when duplicate variablenames has been detected in the query arguments. Please doublecheck that you don't supply multiple arguments with the same variableName
    /// </summary>
    public class GraphQLDuplicateVariablesException : GraphQLException
    {
        public GraphQLDuplicateVariablesException(ICollection<string> duplicateVariableNames) : base("Exception thrown when duplicate variablenames has been detected in the query arguments. Please doublecheck that you don't supply multiple arguments with the same variableName. The duplicated variable names was: " + Environment.NewLine + String.Join(Environment.NewLine, duplicateVariableNames))
        {
            DuplicateVariableNames = new ReadOnlyCollection<string>(duplicateVariableNames.ToList());
        }

        public IReadOnlyCollection<string> DuplicateVariableNames { get; }
    }
}
