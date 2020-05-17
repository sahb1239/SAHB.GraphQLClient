using System;

namespace SAHB.GraphQL.Client.Subscription.Integration.Tests.ChatSchema
{
    public class ReceivedMessage
    {
        public string FromId { get; set; }

        public string Content { get; set; }

        public DateTime SentAt { get; set; }
    }
}
