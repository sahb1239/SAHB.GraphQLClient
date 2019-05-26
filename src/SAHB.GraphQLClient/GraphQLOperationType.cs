namespace SAHB.GraphQLClient.FieldBuilder
{
    /// <summary>
    /// The GraphQL Operation to execute
    /// </summary>
    public enum GraphQLOperationType
    {
        /// <summary>
        /// Query OperationType
        /// </summary>
        Query,
        /// <summary>
        /// Mutation OperationType
        /// </summary>
        Mutation,
        /// <summary>
        /// Subscription OperationType
        /// </summary>
        Subscription
    }
}
