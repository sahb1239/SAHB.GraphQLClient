namespace SAHB.GraphQL.Client.Introspection.Validation
{
    public enum ValidationType
    {
        Operation_Type_Not_Found,
        Field_Not_Found,
        Field_Deprecated,
        Field_Cannot_Have_SelectionSet,
        Field_Should_Have_SelectionSet,
        Argument_Not_Found,
        Argument_Invalid_Type,
        PossibleType_Not_Found
    }
}
