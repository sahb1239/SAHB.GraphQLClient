using System.Runtime.Serialization;

namespace SAHB.GraphQLClient.Subscription.Internal
{
    /// <summary>
    ///     GraphQL operation messages
    ///     File from: https://github.com/graphql-dotnet/server/blob/develop/src/Transports.Subscriptions.Abstractions/OperationMessage.cs
    ///     Protocol defined in https://github.com/apollographql/subscriptions-transport-ws/blob/master/PROTOCOL.md
    /// </summary>
    [DataContract]
    internal class OperationMessage
    {
        /// <summary>
        ///     Nullable Id
        /// </summary>
        [DataMember(Name = "id")]
        public string Id { get; set; }

        /// <summary>
        ///     Type <see cref="MessageType" />
        /// </summary>
        [DataMember(Name = "type")]
        public string Type { get; set; }

        /// <summary>
        ///     Nullable payload
        /// </summary>
        [DataMember(Name = "payload")]
        public object Payload { get; set; }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"Type: {Type} Id: {Id} Payload: {Payload}";
        }
    }
}
