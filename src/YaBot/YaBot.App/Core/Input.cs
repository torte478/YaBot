namespace YaBot.App.Core
{
    using Telegram.Bot;
    using Telegram.Bot.Types;

    public class Input // TODO : to sealed (for tests)
    {
        public ITelegramBotClient Client { get; set; }
        
        public Message Message { get; set; }
    }
}