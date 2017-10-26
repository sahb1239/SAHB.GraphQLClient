using System;

namespace SAHB.GraphQLClient.QueryBuilder
{
    // ReSharper disable once InconsistentNaming
    public class GraphQLQueryArgument
    {
        public GraphQLQueryArgument(string variableName, string argumentType, string argumentValue)
        {
            VariableName = variableName ?? throw new ArgumentNullException(nameof(variableName));
            ArgumentType = argumentType ?? throw new ArgumentNullException(nameof(argumentType));
            ArgumentValue = argumentValue ?? throw new ArgumentNullException(nameof(argumentValue));
        }

        public string VariableName { get; set; }
        public string ArgumentType { get; set; }
        public string ArgumentValue { get; set; }
    }
}