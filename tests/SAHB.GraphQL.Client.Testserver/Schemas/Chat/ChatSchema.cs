using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using GraphQL.Resolvers;
using GraphQL.Subscription;
using GraphQL.Types;

namespace SAHB.GraphQL.Client.Subscription.Integration.Tests.ChatSchema
{
    // This example is from https://github.com/graphql-dotnet/graphql-dotnet/blob/cf296ac7ee27b91f04c95bd32c0db40de6a82a87/src/GraphQL.Tests/Subscription/SubscriptionSchema.cs
    public class ChatSchema : Schema
    {
        public ChatSchema(IChat chat)
        {
            Query = new ChatQuery(chat);
            Mutation = new ChatMutation(chat);
            Subscription = new ChatSubscriptions(chat);
        }

        private class ChatSubscriptions : ObjectGraphType
        {
            private readonly IChat _chat;

            public ChatSubscriptions(IChat chat)
            {
                _chat = chat;
                AddField(new EventStreamFieldType
                {
                    Name = "messageAdded",
                    Type = typeof(MessageType),
                    Resolver = new FuncFieldResolver<Message>(ResolveMessage),
                    Subscriber = new EventStreamResolver<Message>(Subscribe)
                });

                AddField(new EventStreamFieldType
                {
                    Name = "messageAddedByUser",
                    Arguments = new QueryArguments(
                        new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "id" }
                    ),
                    Type = typeof(MessageType),
                    Resolver = new FuncFieldResolver<Message>(ResolveMessage),
                    Subscriber = new EventStreamResolver<Message>(SubscribeById)
                });

                AddField(new EventStreamFieldType
                {
                    Name = "messageAddedAsync",
                    Type = typeof(MessageType),
                    Resolver = new FuncFieldResolver<Message>(ResolveMessage),
                    AsyncSubscriber = new AsyncEventStreamResolver<Message>(SubscribeAsync)
                });

                AddField(new EventStreamFieldType
                {
                    Name = "messageAddedByUserAsync",
                    Arguments = new QueryArguments(
                        new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "id" }
                    ),
                    Type = typeof(MessageType),
                    Resolver = new FuncFieldResolver<Message>(ResolveMessage),
                    AsyncSubscriber = new AsyncEventStreamResolver<Message>(SubscribeByIdAsync)
                });

                AddField(new EventStreamFieldType
                {
                    Name = "messageGetAll",
                    Type = typeof(ListGraphType<MessageType>),
                    Resolver = new FuncFieldResolver<List<Message>>(context => context.Source as List<Message>),
                    Subscriber = new EventStreamResolver<List<Message>>(context => _chat.MessagesGetAll())
                });

                AddField(new EventStreamFieldType
                {
                    Name = "newMessageContent",
                    Type = typeof(StringGraphType),
                    Resolver = new FuncFieldResolver<string>(context => context.Source as string),
                    Subscriber = new EventStreamResolver<string>(context => Subscribe(context).Select(message => message.Content))
                });
            }

            private IObservable<Message> SubscribeById(ResolveEventStreamContext context)
            {
                var id = context.GetArgument<string>("id");

                var messages = _chat.Messages();

                return messages.Where(message => message.From.Id == id);
            }

            private async Task<IObservable<Message>> SubscribeByIdAsync(ResolveEventStreamContext context)
            {
                var id = context.GetArgument<string>("id");

                var messages = await _chat.MessagesAsync();
                return messages.Where(message => message.From.Id == id);
            }

            private Message ResolveMessage(ResolveFieldContext context)
            {
                var message = context.Source as Message;

                return message;
            }

            private IObservable<Message> Subscribe(ResolveEventStreamContext context)
            {
                return _chat.Messages();
            }

            private Task<IObservable<Message>> SubscribeAsync(ResolveEventStreamContext context)
            {
                return _chat.MessagesAsync();
            }
        }

        private class ChatMutation : ObjectGraphType<object>
        {
            public ChatMutation(IChat chat)
            {
                Field<MessageType>("addMessage",
                    arguments: new QueryArguments(
                        new QueryArgument<MessageInputType> { Name = "message" }
                    ),
                    resolve: context =>
                    {
                        var receivedMessage = context.GetArgument<ReceivedMessage>("message");
                        var message = chat.AddMessage(receivedMessage);
                        return message;
                    });
            }
        }

        private class ChatQuery : ObjectGraphType
        {
            public ChatQuery(IChat chat)
            {
                Field<ListGraphType<MessageType>>("messages", resolve: context => chat.AllMessages.Take(100));
            }
        }

        private class MessageType : ObjectGraphType<Message>
        {
            public MessageType()
            {
                Field(o => o.Content);
                Field(o => o.SentAt);
                Field(o => o.From, false, typeof(MessageFromType)).Resolve(ResolveFrom);
            }

            private MessageFrom ResolveFrom(ResolveFieldContext<Message> context)
            {
                var message = context.Source;
                return message.From;
            }
        }

        private class MessageInputType : InputObjectGraphType
        {
            public MessageInputType()
            {
                Field<StringGraphType>("fromId");
                Field<StringGraphType>("content");
                Field<DateGraphType>("sentAt");
            }
        }

        private class MessageFromType : ObjectGraphType<MessageFrom>
        {
            public MessageFromType()
            {
                Field(o => o.Id);
                Field(o => o.DisplayName);
            }
        }
    }
}
