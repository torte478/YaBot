namespace YaBot.App.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Extensions;
    using Telegram.Bot;
    using Telegram.Bot.Types;

    internal sealed class Bot
    {
        private readonly Func<string, IWords> parse;
        private readonly Func<Func<IWords, Answer>> createReceiver;

        private readonly Dictionary<long, Func<IWords, Answer>> chats = new();

        public Bot(Func<string, IWords> parse, Func<Func<IWords, Answer>> createReceiver)
        {
            this.parse = parse;
            this.createReceiver = createReceiver;
        }

        public async Task ReceiveAsync(ITelegramBotClient client, Update update, CancellationToken cancellation)
        {
            try
            {
                var id = update.Message.Chat.Id;
                
                if (!chats.ContainsKey(id))
                    chats.Add(id, createReceiver());

                var answer = update.Message.Text
                    ._(parse)
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