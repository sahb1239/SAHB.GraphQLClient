using SAHB.GraphQLClient.FieldBuilder;
using System;

namespace SAHB.GraphQL.Client.Introspection.Validation
{
    /// <summary>
    /// ValidationError used for the <see cref="GraphQLValidation"/>
    /// </summary>
    public class ValidationError
    {
        internal ValidationError(ValidationType validationType, GraphQLOperationType operationType)
        {
            this.ValidationType = validationType;
            this.OperationType = operationType;
        }

        internal ValidationError(string path, ValidationType validationType, GraphQLField field)
        {
            this.Path = path;
            this.ValidationType = validationType;
            this.Field = field;
        }

        internal ValidationError(string path, ValidationType validationType, GraphQLField field, string expected, string actual)
        {
            this.Path = path;
            this.ValidationType = validationType;
            this.Field = field;
            this.Expected = expected;
            this.Actual = actual;
        }

        /// <summary>
        /// The path to the GraphQL field where the validationerror happened
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// The validation error type
        /// </summary>
        public ValidationType ValidationType { get; }

        internal GraphQLField Field { get; }
        internal string Expected { get; }
        internal string Actual { get; }
        internal GraphQLOperationType OperationType { get; }

        /// <summary>
        /// The validation error message
        /// </summary>
        public string Message
        {
            get
            {
                switch (ValidationType)
                {
                    case ValidationType.Argument_Invalid_Type:
                        return $"Argument at {Path} has a invalid type. Expected is {Expected}, actual is {Actual}.";
                    case ValidationType.Argument_Not_Found:
                        return $"Argument at {Path} was not found";
                    case ValidationType.Field_Cannot_Have_SelectionSet:
                        return $"Field at {Path} cannot have a selectionSet";
                    case ValidationType.Field_Deprecated:
                        return $"Field at {Path} is deprecated";
                    case ValidationType.Field_Not_Found:
                        return $"Field at {Path} was not found";
                    case ValidationType.Field_Should_Have_SelectionSet:
                        return $"Field at {Path} should have a selectionSet";
                    case ValidationType.Operation_Type_Not_Found:
                        return $"OperationType {OperationType} was not found";
                    case ValidationType.PossibleType_Not_Found:
                        return $"Possible type at {Path} was not found";
                    case ValidationType.Field_Invalid_Type:
                        return $"Type at {Path} is invalid. Expected is {Expected}, actual is {Actual}.";
                    case ValidationType.Field_Type_Not_Enum:
                        return $"Type at {Path} is not an enum";
                    case ValidationType.EnumValue_Not_Found:
                        return $"Enumvalue at {Path} was not found";
                    case ValidationType.EnumValue_Deprecated:
                        return $"Enumvalue at {Path} is deprecated";
                    default:
                        throw new NotImplementedException($"ValidationType {ValidationType} not implemented");
                }
            }
        }

        /// <summary>
        /// Returns the message
        /// </summary>
        /// <returns>Returns the message</returns>
        public override string ToString()
        {
            return Message;
        }
    }
}
