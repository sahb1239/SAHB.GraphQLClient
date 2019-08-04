using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace SAHB.GraphQL.Client.Subscription.Integration.Tests.ChatSchema
{
    public class Chat : IChat
    {
        private readonly ISubject<Message> _messageStream = new ReplaySubject<Message>(1);
        private readonly ISubject<List<Message>> _allMessageStream = new ReplaySubject<List<Message>>(1);

        public Chat()
        {
            AllMessages = new ConcurrentStack<Message>();
            Users = new ConcurrentDictionary<string, string>
            {
                ["1"] = "developer",
                ["2"] = "tester"
            };
        }

        public ConcurrentDictionary<string, string> Users { get; set; }

        public ConcurrentStack<Message> AllMessages { get; }

        public Message AddMessage(ReceivedMessage message)
        {
            if (!Users.TryGetValue(message.FromId, out var displayName))
            {
                displayName = "(unknown)";
            }

            return AddMessage(new Message
            {
                Content = message.Content,
                SentAt = message.SentAt,
                From = new MessageFrom
                {
                    DisplayName = displayName,
                    Id = message.FromId
                }
            });
        }

        public async Task<IObservable<Message>> MessagesAsync()
        {
            //pretend we are doing something async here
            await Task.Delay(100);
            return Messages();
        }

        public List<Message> AddMessageGetAll(Message message)
        {
            AllMessages.Push(message);
            var l = new List<Message>(AllMessages);
            _allMessageStream.OnNext(l);
            return l;
        }

        public Message AddMessage(Message message)
        {
            AllMessages.Push(message);
            _messageStream.OnNext(message);
            return message;
        }

        public IObservable<Message> Messages()
        {
            return _messageStream.AsObservable();
        }

        public IObservable<List<Message>> MessagesGetAll()
        {
            return _allMessageStream.AsObservable();
        }

        public void AddError(Exception exception)
        {
            _messageStream.OnError(exception);
        }
    }
}
