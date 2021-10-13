namespace YaBot.App.Core.State
{
    using System;
    using System.Collections.Immutable;
    using System.Linq;
    using YaBot.Extensions;
    using YaBot.IO;

    public sealed class States
    {
        private readonly IState start;
        private readonly ImmutableArray<IState> liners;
        private readonly ImmutableArray<IState> resets;
        private readonly ImmutableArray<IState> stoppers;
        private readonly Action<string> log;

        private bool stopped;

        private IState current;
        private IState Current
        {
            get => current;
            set
            {
                if (current != value)
                {
                    log($"{current.Name} => {value.Name}");
                    Changed.Raise(value.Name);
                }
                current = value;
            }
        }

        public States(
            IState start,
            ImmutableArray<IState> liners,
            ImmutableArray<IState> resets,
            ImmutableArray<IState> stoppers,
            Action<string> log)
        {
            this.start = start;
            this.liners = liners;
            this.resets = resets;
            this.stoppers = stoppers;
            this.log = log;

            current = start;
        }

        public event Action<string> Changed;

        public IOutput Process(IInput input)
        {
            if (stopped)
                return null;

            var stop = stoppers.FirstOrDefault(_ => _.IsInput(input));
            if (stop != null)
            {
                stopped = true;
                return stop.Process(input).Item1;
            }

            var reset = resets.FirstOrDefault(_ => _.IsInput(input));
            if (Current != start && reset != null)
            {
                Current.Reset();
                Current = start;
                return reset.Process(input).Item1;
            }

            var liner = liners.FirstOrDefault(_ => _.IsInput(input));
            if (liner != null)
                return liner.Process(input).Item1;
            
            var (answer, next) = Current.Process(input);

            next ??= start;

            Current = next;

            return answer;
        }
    }
}