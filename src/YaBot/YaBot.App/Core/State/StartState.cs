namespace YaBot.App.Core.State
{
    using System;
    using System.Collections.Immutable;
    using System.Linq;
    using Extensions;
    using Outputs;

    internal sealed class StartState : IState
    {
        private readonly IWords names;
        private readonly IWords ping;
        private readonly ImmutableArray<IState> next;
        private readonly Func<IWords, IOutput> toOutput;

        public StartState(IWords names, IWords ping, ImmutableArray<IState> next, Func<IWords, IOutput> toOutput)
        {
            this.names = names;
            this.ping = ping;
            this.next = next;
            this.toOutput = toOutput;
        }

        public string Name => "Start";

        public bool IsInput(IInput input)
        {
            return true;
        }

        public (IOutput, IState) Process(IInput input)
        {
            if (!names.Match(input.Text))
                return (null, this);

            return next
                   .FirstOrDefault(_ => _.IsInput(input))
                   ?._(_ => _.Process(input))
               ?? 
               (ping._(toOutput), this);
        }

        public IState Reset()
        {
            return this;
        }
    }
}