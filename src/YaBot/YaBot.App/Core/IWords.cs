namespace YaBot.App.Core
{
    using System.Collections.Generic;
    using Telegram.Bot.Types;

    public interface IWords : IEnumerable<string>
    {
        bool Match(Message message);
    }
}