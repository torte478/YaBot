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
        private readonly Func<Func<IWords, string>> createReceiver;

        private readonly Dictionary<long, Func<IWords, string>> chats = new();

        public Bot(Func<string, IWords> parse, Func<Func<IWords, string>> createReceiver)
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
                    
                await update.Message.Text
                    ._(parse)
                    ._(chats[id])
                    .If(
                        _ => !_.Equals(string.Empty),
                        _ => client.SendTextMessageAsync(update.Message.Chat, _, cancellationToken: cancellation),
                        _ => Task.CompletedTask);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}