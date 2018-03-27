using System;

namespace SAHB.GraphQLClient.FieldBuilder.Attributes
{
    // ReSharper disable once InconsistentNaming
    /// <summary>
    /// Attribute which defines a argument which is used for a GraphQL field
    /// </summary>    
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = true)]
    public class GraphQLArgumentsAttribute : Attribute
    {
        /// <summary>
        /// Initilizes a attribute which defines a argument which is used for a GraphQL field
        /// </summary>
        /// <param name="argumentName">The argument name used in the GraphQL query</param>
        /// <param name="argumentType">The argument type of the argument in the GraphQL query</param>
        /// <param name="variableName">The variable name used in the GraphQL query</param>
        public GraphQLArgumentsAttribute(string argumentName, string argumentType, string variableName) 
            : this(argumentName: argumentName, argumentType: argumentType, variableName: variableName, isRequired: false)
        {
        }

        /// <summary>
        /// Initilizes a attribute which defines a argument which is used for a GraphQL field
        /// </summary>
        /// <param name="argumentName">The argument name used in the GraphQL query</param>
        /// <param name="argumentType">The argument type of the argument in the GraphQL query</param>
        /// <param name="variableName">The variable name used in the GraphQL query</param>
        /// <param name="isRequired">Is the GraphQL argument required to execute the query</param>
        public GraphQLArgumentsAttribute(string argumentName, string argumentType, string variableName, bool isRequired)
        {
            ArgumentName = argumentName ?? throw new ArgumentNullException(nameof(argumentName));
            ArgumentType = argumentType ?? throw new ArgumentNullException(nameof(argumentType));
            VariableName = variableName ?? throw new ArgumentNullException(nameof(variableName));
            IsRequired = isRequired;
        }

        /// <summary>
        /// Initilizes a attribute which defines a argument which is used for a GraphQL field
        /// </summary>
        /// <param name="argumentName">The argument name used in the GraphQL query</param>
        /// <param name="argumentType">The argument type of the argument in the GraphQL query</param>
        /// <param name="variableName">The variable name used in the GraphQL query</param>
        /// <param name="isRequired">Is the GraphQL argument required to execute the query</param>
        /// <param name="inlineArgument">Should the GraphQL argument be inlined</param>
        public GraphQLArgumentsAttribute(string argumentName, string argumentType, string variableName, bool isRequired, bool inlineArgument)
        {
            ArgumentName = argumentName ?? throw new ArgumentNullException(nameof(argumentName));
            ArgumentType = argumentType ?? throw new ArgumentNullException(nameof(argumentType));
            VariableName = variableName ?? throw new ArgumentNullException(nameof(variableName));
            IsRequired = isRequired;
            InlineArgument = inlineArgument;
        }

        /// <summary>
        /// Initilizes a attribute which defines a argument which is used for a GraphQL field
        /// </summary>
        /// <param name="argumentName">The argument name used in the GraphQL query</param>
        /// <param name="argumentType">The argument type of the argument in the GraphQL query</param>
        /// <param name="variableName">The variable name used in the GraphQL query</param>
        /// <param name="isRequired">Is the GraphQL argument required to execute the query</param>
        /// <param name="inlineArgument">Should the GraphQL argument be inlined</param>
        /// <param name="defaultValue">The default value for the GraphQL argument</param>
        public GraphQLArgumentsAttribute(string argumentName, string argumentType, string variableName, bool isRequired, bool inlineArgument, object defaultValue)
        {
            ArgumentName = argumentName ?? throw new ArgumentNullException(nameof(argumentName));
            ArgumentType = argumentType ?? throw new ArgumentNullException(nameof(argumentType));
            VariableName = variableName ?? throw new ArgumentNullException(nameof(variableName));
            IsRequired = isRequired;
            InlineArgument = inlineArgument;
            DefaultValue = defaultValue;
        }

        /// <summary>
        /// The argument name used in the GraphQL query
        /// </summary>
        public string ArgumentName { get; }

        /// <summary>
        /// The argument type of the variable
        /// </summary>
        public string ArgumentType { get; set; }

        /// <summary>
        /// The variable name used in the GraphQL query
        /// </summary>
        public string VariableName { get; }

        /// <summary>
        /// Is the argument required for execution of the query
        /// </summary>
        public bool IsRequired { get; set; }

        /// <summary>
        /// Should the argument be inlined
        /// </summary>
        public bool? InlineArgument { get; set; }

        /// <summary>
        /// Default value for the argument
        /// </summary>
        public object DefaultValue { get; }
    }
}