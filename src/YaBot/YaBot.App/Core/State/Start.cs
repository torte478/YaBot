namespace YaBot.App.Core.State
{
    using Extensions;

    internal sealed class Start : IState
    {
        private readonly IWords names;
        private readonly IWords keys;
        private readonly IState next;

        public Start(IWords names, IWords keys, IState next)
        {
            this.names = names;
            this.keys = keys;
            this.next = next;
        }

        public (Answer, IState) Process(IWords words)
        {
            if (names.Match(words))
            {
                if (keys.Match(words))
                    return (next.Process(words).Item1, next);
                else
                    return ("Звали меня?".ToAnswer(), this);
            }
            else
            {
                return (string.Empty.ToAnswer(), this);
            }
        }

        public IState Reset()
        {
            return this;
        }

        public override string ToString()
        {
            return "Start";
        }
    }
}