namespace YaBot.App.Core.State
{
    using System.Collections.Immutable;
    using System.Linq;
    using Extensions;
    using Telegram.Bot.Types;

    internal sealed class StartState : IState
    {
        private readonly IWords names;
        private readonly ImmutableArray<IState> next;

        public StartState(IWords names, ImmutableArray<IState> next)
        {
            this.names = names;
            this.next = next;
        }

        public bool IsInput(Input input)
        {
            return true;
        }

        public (Output, IState) Process(Input input)
        {
            if (!names.Match(input.Message))
                return (string.Empty.ToOutput(), this);

            return next
                   .FirstOrDefault(_ => _.IsInput(input))
                   ?._(_ => _.Process(input))
               ?? 
               ("Звали меня?".ToOutput(), this); // TODO
        }

        public IState Reset()
        {
            return this;
        }
    }
}