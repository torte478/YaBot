namespace YaBot.Core.IO.Format
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Text;
    using Telegram.Bot.Types;
    using YaBot.Core.Extensions;

    public sealed class FormattedText
    {
        private readonly ImmutableArray<IToken> formats;

        public FormattedText(ImmutableArray<IToken> formats)
        {
            this.formats = formats;
        }

        public string Serialize(Message message)
        {
            //TODO : to cool swtich
            if (message?.Text?._(string.IsNullOrEmpty) ?? true)
                return null;

            if (message.Entities == null)
                return message.Text;

            var entities = message.Entities
                .Where(_ => formats.Any(f => f.Type == _.Type))
                .OrderBy(_ => _.Offset)
                ._(_ => new Queue<MessageEntity>(_));

            var result = new StringBuilder();
            for (var i = 0; i < message.Text.Length; ++i)
            {
                if (entities.TryPeek(out var entity) && entity.Offset == i)
                {
                    var end = entity.Offset + entity.Length - 1;
                    var format = formats.Single(_ => _.Type == entity.Type);

                    message.Text
                        .Substring(entity.Offset, entity.Length)
                        ._(_ => format.Serialize(entity, _))
                        ._(result.Append);

                    entities.Dequeue();
                    i = end;
                }
                else
                {
                    result.Append(message.Text[i]);
                }
            }

            return result.ToString();
        }

        public (string, MessageEntity[]) Deserialize(string text)
        {
            (string token, MessageEntity entity) seed = (token: text, null);
            var tokens =  formats
                .Aggregate(
                    new[] { seed }.AsEnumerable(),
                    Deserialize);

            var result = new StringBuilder();
            var entities = new List<MessageEntity>();
            foreach (var (token, entity) in tokens)
            {
                if (entity != null)
                {
                    entity.Offset = result.Length;
                    entity.Length = token.Length;
                    entities.Add(entity);
                }

                result.Append(token);
            }

            return (result.ToString(), entities.ToArray());
        }

        private IEnumerable<(string, MessageEntity)> Deserialize(
            IEnumerable<(string token, MessageEntity entity)> acc,
            IToken format)
        {
            return acc
                .SelectMany(_ => _.entity != null
                    ? new[] { _ }
                    : format.Deserialize(_.token));
        }
    }
}