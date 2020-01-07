namespace SAHB.GraphQL.Client.Introspection.Validation
{
    /// <summary>
    /// The type of the validation used by <see cref="ValidationError"/>
    /// </summary>
    public enum ValidationType
    {
        /// <summary>
        /// This validationType is used if the operationType such as Query, Mutate or Subscription was not found in the list of types
        /// </summary>
        Operation_Type_Not_Found,

        /// <summary>
        /// This validationType is used if the specified field was not found on the type
        /// </summary>
        Field_Not_Found,

        /// <summary>
        /// This validationType is used if the specified field is deprecated
        /// </summary>
        Field_Deprecated,

        /// <summary>
        /// This validationType is used if the specified field cannot have a selection set but has it on the selectionSet which is being validated
        /// </summary>
        Field_Cannot_Have_SelectionSet,

        /// <summary>
        /// This validationType is used if the specified field should have a selection set but does not have it on the selectionSet which is being validated
        /// </summary>
        Field_Should_Have_SelectionSet,

        /// <summary>
        /// This validationType is used if the specified argument was not found on the field
        /// </summary>
        Argument_Not_Found,

        /// <summary>
        /// This validationType is used if the specified argument has the wrong type
        /// </summary>
        Argument_Invalid_Type,

        /// <summary>
        /// This validationType is used if the specified type of the field is not valid
        /// </summary>
        Field_Invalid_Type,

        /// <summary>
        /// This validationType is used if the specified type should be nullable
        /// </summary>
        Field_Should_Be_Nullable,

        /// <summary>
        /// This validationType is used if the specified type should be not nullable
        /// </summary>
        Field_Should_Be_NonNull,

        /// <summary>
        /// This validationType is used if the specified type of the field is not an enum and therefore not valid
        /// </summary>
        Field_Type_Not_Enum,

        /// <summary>
        /// This validationType is used if the one of the possible type for a union or interface was not found
        /// </summary>
        PossibleType_Not_Found,

        /// <summary>
        /// The enum value was not found
        /// </summary>
        EnumValue_Not_Found,

        /// <summary>
        /// The enum value is deprecated
        /// </summary>
        EnumValue_Deprecated
    }
}
