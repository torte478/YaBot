namespace YaBot.App.Core
{
    using System.Collections.Generic;

    public interface IWords : IEnumerable<string>
    {
        bool Match(string text, bool substring = false);
    }
}