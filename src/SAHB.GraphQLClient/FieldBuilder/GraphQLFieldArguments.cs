using System;

namespace SAHB.GraphQLClient.FieldBuilder
{
    // ReSharper disable once InconsistentNaming
    public class GraphQLFieldArguments
    {
        public GraphQLFieldArguments(string argumentName, string variableName)
        {
            ArgumentName = argumentName ?? throw new ArgumentNullException(nameof(argumentName));
            VariableName = variableName ?? throw new ArgumentNullException(nameof(variableName));
        }

        public string ArgumentName { get; set; }
        public string VariableName { get; set; }
    }
}