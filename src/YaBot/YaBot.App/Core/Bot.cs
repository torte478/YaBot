namespace YaBot.App.Core
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Extensions;
    using Telegram.Bot;
    using Telegram.Bot.Types;

    internal sealed class Bot
    {
        private readonly Func<string, IWords> parse;
        private readonly Func<IWords, string> process;

        public Bot(Func<string, IWords> parse, Func<IWords, string> process)
        {
            this.parse = parse;
            this.process = process;
        }

        public async Task ReceiveAsync(ITelegramBotClient client, Update update, CancellationToken cancellation)
        {
            try
            {
                await update.Message.Text
                    ._(parse)
                    ._(process)
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