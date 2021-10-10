namespace YaBot.App.Core.State
{
    using System;
    using System.Collections.Immutable;
    using System.Linq;
    using Outputs;
    using YaBot.Core.Extensions;
    using YaBot.Core.IO;

    public sealed class States
    {
        // TODO : refactor
        private readonly IState start;
        private readonly ImmutableArray<IState> liners;
        private readonly IWords stoppers;
        private readonly IWords reset;
        private readonly IOutputFactory<string, IWords> outputs;
        private readonly Action<string> log;

        private IState current;
        private IState Current
        {
            get => current;
            set
            {
                if (current != value)
                {
                    log($"{current.Name} => {value.Name}");
                    StateChanged.Raise(value.Name);
                }
                current = value;
            }
        }

        public States(
            IState start,
            ImmutableArray<IState> liners,
            IWords stoppers, 
            IWords reset,
            IOutputFactory<string, IWords> outputs,
            Action<string> log)
        {
            this.start = start;
            this.stoppers = stoppers;
            this.reset = reset;
            this.outputs = outputs;
            this.log = log;
            this.liners = liners;

            current = start;
        }

        // TODO : rename
        public event Action<string> StateChanged;

        public IOutput Process(IInput input)
        {
            var stop = Current != start && stoppers.Match(input.Text);
            if (stop)
            {
                Current.Reset();
                Current = start;
                return reset._(outputs.Create);
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