using System;

namespace SAHB.GraphQLClient.FieldBuilder.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = true)]
    // ReSharper disable once InconsistentNaming
    public class GraphQLArgumentsAttribute : Attribute
    {
        [Obsolete]
        public GraphQLArgumentsAttribute(string argumentName) : this(argumentName, argumentName)
        {
        }

        public GraphQLArgumentsAttribute(string argumentName, string variableName)
        {
            VariableName = variableName ?? throw new ArgumentException(nameof(variableName));
            ArgumentName = argumentName ?? throw new ArgumentNullException(nameof(argumentName));
        }

        public string ArgumentName { get; }
        public string VariableName { get; }
    }
}