namespace YaBot.Core.IO.Format
{
    using System.Collections.Generic;
    using Telegram.Bot.Types;

    public interface IToken
    {
        string Serialize(MessageEntity entity, string text);
        IEnumerable<(string, MessageEntity)> Deserialize(string text);
    }
}