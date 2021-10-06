namespace YaBot.Core.IO.Format
{
    using System;
    using System.Linq;
    using YaBot.Core.Extensions;

    public sealed class Title
    {
        private readonly int length;
        private readonly Func<string, string> format;

        public Title(int length, Func<string, string> format)
        {
            this.length = length;
            this.format = format;
        }

        public string Create(string text)
        {
            return text
                ._(format)
                .Split(new[] { '\r', '\n'}, StringSplitOptions.RemoveEmptyEntries)
                .First()
                ._(_ => _.Length > length
                    ? $"{_[..length]}..."
                    : _);
        }
    }
}