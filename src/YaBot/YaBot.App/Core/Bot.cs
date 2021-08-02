namespace YaBot.App.Core
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Extensions;
    using Telegram.Bot;
    using Telegram.Bot.Types;

    public sealed class Bot
    {
        private readonly Func<Func<Message, Answer>> createReceiver;
        private readonly DateTime begin;

        private readonly Dictionary<long, Func<Message, Answer>> chats = new();

        public Bot(Func<Func<Message, Answer>> createReceiver, DateTime begin)
        {
            this.createReceiver = createReceiver;
            this.begin = begin;
        }

        public async Task ReceiveAsync(ITelegramBotClient client, Update update, CancellationToken cancellation)
        {
            if (update.Message.Date < begin)
                return;
            
            try
            {
                var id = update.Message.Chat.Id;
                
                if (!chats.ContainsKey(id))
                    chats.Add(id, createReceiver());

                var answer = update.Message
                    ._(chats[id]);
                    
                if (answer.Text._(string.IsNullOrEmpty))
                    await Task.CompletedTask;
                else if (answer.Image != null)
                {
                    await client.SendPhotoAsync(update.Message.Chat, answer.Image, answer.Text,
                        cancellationToken: cancellation);
                    answer.Image.Content.Dispose();
                }
                else
                    await client.SendTextMessageAsync(update.Message.Chat, answer.Text, cancellationToken: cancellation);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}