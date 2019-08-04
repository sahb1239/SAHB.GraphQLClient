using System;
using SAHB.GraphQLClient.FieldBuilder.Attributes;

namespace SAHB.GraphQLClient.FieldBuilder
{
    // ReSharper disable once InconsistentNaming
    /// <summary>
    /// GraphQL argument used to contain metadata which can be used for generating a GraphQL query
    /// </summary>
    public class GraphQLFieldArguments
    {
        /// <summary>
        /// Initializes a GraphQL argument used to contain metadata which can be used for generating a GraphQL query
        /// </summary>
        /// <param name="argument">The argument to initialize from</param>
        internal GraphQLFieldArguments(GraphQLArgumentsAttribute argument) 
            : this(argument.ArgumentName, argument.ArgumentType, argument.VariableName, argument.IsRequired, argument.InlineArgument, argument.DefaultValue)
        {
        }

        /// <summary>
        /// Initializes a GraphQL argument used to contain metadata which can be used for generating a GraphQL query
        /// </summary>
        /// <param name="argument">The directiveargument to initialize from</param>
        internal GraphQLFieldArguments(GraphQLDirectiveArgumentAttribute argument)
            : this(argument.ArgumentName, argument.ArgumentType, argument.VariableName, argument.IsRequired, argument.InlineArgument, argument.DefaultValue)
        {
        }

        /// <summary>
        /// Initializes a GraphQL argument used to contain metadata which can be used for generating a GraphQL query
        /// </summary>
        /// <param name="argumentName">GraphQL argument name</param>
        /// <param name="argumentType">GraphQL argument type of the variable</param>
        /// <param name="variableName">GraphQL variable name</param>
        public GraphQLFieldArguments(string argumentName, string argumentType, string variableName) 
            : this(argumentName: argumentName, argumentType: argumentType, variableName: variableName, isRequired: false)
        {
        }

        /// <summary>
        /// Initializes a GraphQL argument used to contain metadata which can be used for generating a GraphQL query
        /// </summary>
        /// <param name="argumentName">GraphQL argument name</param>
        /// <param name="argumentType">GraphQL argument type of the variable</param>
        /// <param name="variableName">GraphQL variable name</param>
        /// <param name="isRequired">Is the GraphQL argument required to execute the query</param>
        public GraphQLFieldArguments(string argumentName, string argumentType, string variableName, bool isRequired)
            : this(argumentName: argumentName, argumentType: argumentType, variableName: variableName, isRequired: isRequired, inlineArgument: null)
        {
        }

        /// <summary>
        /// Initializes a GraphQL argument used to contain metadata which can be used for generating a GraphQL query
        /// </summary>
        /// <param name="argumentName">GraphQL argument name</param>
        /// <param name="argumentType">GraphQL argument type of the variable</param>
        /// <param name="variableName">GraphQL variable name</param>
        /// <param name="isRequired">Is the GraphQL argument required to execute the query</param>
        /// <param name="inlineArgument">Should the GraphQL argument be inlined</param>
        public GraphQLFieldArguments(string argumentName, string argumentType, string variableName, bool isRequired,
            bool? inlineArgument)
            : this(argumentName: argumentName, argumentType: argumentType, variableName: variableName,
                isRequired: isRequired, inlineArgument: inlineArgument, defaultValue: null)
        {

        }

        /// <summary>
        /// Initializes a GraphQL argument used to contain metadata which can be used for generating a GraphQL query
        /// </summary>
        /// <param name="argumentName">GraphQL argument name</param>
        /// <param name="argumentType">GraphQL argument type of the variable</param>
        /// <param name="variableName">GraphQL variable name</param>
        /// <param name="isRequired">Is the GraphQL argument required to execute the query</param>
        /// <param name="inlineArgument">Should the GraphQL argument be inlined</param>
        /// <param name="defaultValue">The default value for the GraphQL argument</param>
        public GraphQLFieldArguments(string argumentName, string argumentType, string variableName, bool isRequired, bool? inlineArgument, object defaultValue)
        {
            ArgumentName = argumentName ?? throw new ArgumentNullException(nameof(argumentName));
            ArgumentType = argumentType ?? throw new ArgumentNullException(nameof(argumentType));
            VariableName = variableName ?? throw new ArgumentNullException(nameof(variableName));
            IsRequired = isRequired;
            InlineArgument = inlineArgument;
            DefaultValue = defaultValue;
        }

        /// <summary>
        /// GraphQL argument name
        /// </summary>
        public string ArgumentName { get; }

        /// <summary>
        /// The argument type of the variable
        /// </summary>
        public string ArgumentType { get; }

        /// <summary>
        /// GraphQL variable name
        /// </summary>
        public string VariableName { get; set; }

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

        /// <inheritdoc />
        public override string ToString()
        {
            return "Name: " + ArgumentName + " Type: " + ArgumentType + " IsRequired: " + IsRequired + " VariableName: " + (VariableName ?? "null") + " DefaultValue: " + (DefaultValue ?? "null");
        }
    }
}