using Newtonsoft.Json.Linq;
using System;

namespace SAHB.GraphQLClient.Subscription.Internal
{
    internal class PayloadEventArgs : EventArgs
    {
        public PayloadEventArgs(JObject payload)
        {
            this.Payload = payload;
        }

        public JObject Payload { get; set; }
    }
}