namespace YaBot.App.Core
{
    using System.Collections.Generic;

    internal interface IWords : IEnumerable<string>
    {
        bool Match(IWords other);
    }
}