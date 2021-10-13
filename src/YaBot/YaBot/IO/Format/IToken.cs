namespace YaBot.IO.Format
{
    using System.Collections.Generic;
    using Telegram.Bot.Types;
    using Telegram.Bot.Types.Enums;

    public interface IToken
    {
        MessageEntityType Type { get; }

        string Serialize(MessageEntity entity, string text);
        IEnumerable<(string, MessageEntity)> Deserialize(string text);
    }
}