namespace SAHB.GraphQLClient.Subscription
{
    /// <summary>
    ///     GraphQL operation messages
    ///     File from: https://github.com/graphql-dotnet/server/blob/develop/src/Transports.Subscriptions.Abstractions/OperationMessage.cs
    /// </summary>
    public class OperationMessage
    {
        /// <summary>
        ///     Nullable Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        ///     Type <see cref="MessageType" />
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        ///     Nullable payload
        /// </summary>
        public object Payload { get; set; }


        /// <inheritdoc />
        public override string ToString()
        {
            return $"Type: {Type} Id: {Id} Payload: {Payload}";
        }
    }
}
