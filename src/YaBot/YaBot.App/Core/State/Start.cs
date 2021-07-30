namespace YaBot.App.Core.State
{
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

        public (string, IState) Process(IWords words)
        {
            if (names.Match(words))
            {
                if (keys.Match(words))
                    return (string.Empty, next);
                else
                    return ("Звали меня?", this);
            }
            else
            {
                return (string.Empty, this);
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