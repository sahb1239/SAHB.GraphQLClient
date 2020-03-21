using System;

namespace SAHB.GraphQL.Client.Subscription.Integration.Tests.ChatSchema
{
    public class Message
    {
        public MessageFrom From { get; set; }

        public string Content { get; set; }

        public DateTime SentAt { get; set; }
    }
}
