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

        public bool IsInput(Message message)
        {
            return true;
        }

        public (Answer, IState) Process(Message message)
        {
            if (!names.Match(message))
                return (string.Empty.ToAnswer(), this);

            return next
                   .FirstOrDefault(_ => _.IsInput(message))
                   ?._(_ => _.Process(message))
               ?? 
               ("Звали меня?".ToAnswer(), this); // TODO
        }

        public IState Reset()
        {
            return this;
        }
    }
}