using Newtonsoft.Json.Linq;
using System;

namespace SAHB.GraphQL.Client.Subscription
{
    public class PayloadEventArgs : EventArgs
    {
        public PayloadEventArgs(JObject payload)
        {
            this.Payload = payload;
        }

        public JObject Payload { get; set; }
    }
}