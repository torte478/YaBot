namespace YaBot.IntegrTests
{
    using System;
    using System.Collections.Immutable;
    using System.Linq;
    using NUnit.Framework;
    using Telegram.Bot.Types;
    using Telegram.Bot.Types.Enums;
    using YaBot.Core.IO.Format;

    [TestFixture]
    internal sealed class FormattedText_Should
    {
        private FormattedText formattedText;

        [SetUp]
        public void SetUp()
        {
            formattedText = new FormattedText(new IToken[]
                {
                    new Token(MessageEntityType.Bold, "**"),
                    new LinkToken(MessageEntityType.TextLink, "^^", "|")
                }
                .ToImmutableArray());
        }

        [Test]
        public void CheckTokenAtSameString_OnSerialize()
        {
            var actual = formattedText.Serialize(new Message
            {
                Text = "01234567",
                Entities = new MessageEntity[]
                {
                    new()
                    {
                        Type = MessageEntityType.Bold,
                        Offset = 1,
                        Length = 3
                    },
                    new()
                    {
                        Type = MessageEntityType.TextLink,
                        Offset = 5,
                        Length = 2,
                        Url = "abc"
                    }
                }
            });

            Assert.That(actual, Is.EqualTo("0**123**4^^56|abc^^7"));
        }

        [Test]
        public void WriteTokenAtSameString_OnDeserialize()
        {
            var (_, entities) = formattedText.Deserialize(
                $"Список мест{Environment.NewLine}{Environment.NewLine}" +
                $"1: **123**{Environment.NewLine}" +
                $"2: 0**12** ^^3456|http://www.google.com/^^00{Environment.NewLine}" +
                $"3: слово{Environment.NewLine}" +
                $"4: раз **два** три ^^четыре|http://www.google.com/^^ пять{Environment.NewLine}");

            var bold = entities
                .Where(_ => _.Type == MessageEntityType.Bold)
                .OrderByDescending(_ => _.Offset)
                .First();
            Assert.That(bold.Offset, Is.EqualTo(55));
        }
    }
}