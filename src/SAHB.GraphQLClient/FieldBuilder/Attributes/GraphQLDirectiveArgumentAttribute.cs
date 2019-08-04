using System;

namespace SAHB.GraphQLClient.FieldBuilder.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = true)]
    public class GraphQLDirectiveArgumentAttribute : Attribute
    {
        /// <summary>
        /// Initializes a attribute which defines a directive argument which is used for a GraphQL field
        /// </summary>
        /// <param name="directiveName">The directive name used in the GraphQL query</param>
        /// <param name="argumentName">The argument name used in the GraphQL query</param>
        /// <param name="argumentType">The argument type of the argument in the GraphQL query</param>
        /// <param name="variableName">The variable name used in the GraphQL query</param>
        public GraphQLDirectiveArgumentAttribute(string directiveName, string argumentName, string argumentType, string variableName)
            : this(directiveName: directiveName, argumentName: argumentName, argumentType: argumentType, variableName: variableName, isRequired: false)
        {
        }

        /// <summary>
        /// Initializes a attribute which defines a directive argument which is used for a GraphQL field
        /// </summary>
        /// <param name="directiveName">The directive name used in the GraphQL query</param>
        /// <param name="argumentName">The argument name used in the GraphQL query</param>
        /// <param name="argumentType">The argument type of the argument in the GraphQL query</param>
        /// <param name="variableName">The variable name used in the GraphQL query</param>
        /// <param name="isRequired">Is the GraphQL argument required to execute the query</param>
        public GraphQLDirectiveArgumentAttribute(string directiveName, string argumentName, string argumentType, string variableName, bool isRequired)
        {
            DirectiveName = directiveName ?? throw new ArgumentNullException(nameof(directiveName));
            ArgumentName = argumentName ?? throw new ArgumentNullException(nameof(argumentName));
            ArgumentType = argumentType ?? throw new ArgumentNullException(nameof(argumentType));
            VariableName = variableName ?? throw new ArgumentNullException(nameof(variableName));
            IsRequired = isRequired;
        }

        /// <summary>
        /// Initializes a attribute which defines a directive argument which is used for a GraphQL field
        /// </summary>
        /// <param name="directiveName">The directive name used in the GraphQL query</param>
        /// <param name="argumentName">The argument name used in the GraphQL query</param>
        /// <param name="argumentType">The argument type of the argument in the GraphQL query</param>
        /// <param name="variableName">The variable name used in the GraphQL query</param>
        /// <param name="isRequired">Is the GraphQL argument required to execute the query</param>
        /// <param name="inlineArgument">Should the GraphQL argument be inlined</param>
        public GraphQLDirectiveArgumentAttribute(string directiveName, string argumentName, string argumentType, string variableName, bool isRequired, bool inlineArgument)
        {
            DirectiveName = directiveName ?? throw new ArgumentNullException(nameof(directiveName));
            ArgumentName = argumentName ?? throw new ArgumentNullException(nameof(argumentName));
            ArgumentType = argumentType ?? throw new ArgumentNullException(nameof(argumentType));
            VariableName = variableName ?? throw new ArgumentNullException(nameof(variableName));
            IsRequired = isRequired;
            InlineArgument = inlineArgument;
        }

        /// <summary>
        /// Initializes a attribute which defines a directive argument which is used for a GraphQL field
        /// </summary>
        /// <param name="directiveName">The directive name used in the GraphQL query</param>
        /// <param name="argumentName">The argument name used in the GraphQL query</param>
        /// <param name="argumentType">The argument type of the argument in the GraphQL query</param>
        /// <param name="variableName">The variable name used in the GraphQL query</param>
        /// <param name="isRequired">Is the GraphQL argument required to execute the query</param>
        /// <param name="inlineArgument">Should the GraphQL argument be inlined</param>
        /// <param name="defaultValue">The default value for the GraphQL argument</param>
        public GraphQLDirectiveArgumentAttribute(string directiveName, string argumentName, string argumentType, string variableName, bool isRequired, bool inlineArgument, object defaultValue)
        {
            DirectiveName = directiveName ?? throw new ArgumentNullException(nameof(directiveName));
            ArgumentName = argumentName ?? throw new ArgumentNullException(nameof(argumentName));
            ArgumentType = argumentType ?? throw new ArgumentNullException(nameof(argumentType));
            VariableName = variableName ?? throw new ArgumentNullException(nameof(variableName));
            IsRequired = isRequired;
            InlineArgument = inlineArgument;
            DefaultValue = defaultValue;
        }

        /// <summary>
        /// Name of the Directive
        /// </summary>
        public string DirectiveName { get; }

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
