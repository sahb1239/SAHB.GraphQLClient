using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SAHB.GraphQL.Client.Subscription.Integration.Tests.ChatSchema
{
    public interface IChat
    {
        ConcurrentStack<Message> AllMessages { get; }

        Message AddMessage(Message message);

        IObservable<Message> Messages();
        IObservable<List<Message>> MessagesGetAll();

        Message AddMessage(ReceivedMessage message);

        Task<IObservable<Message>> MessagesAsync();
    }
}
