namespace YaBot.App.Core.State
{
    using System;
    using Extensions;
    using Telegram.Bot.Types;

    public sealed class States
    {
        private readonly IState start;
        private readonly IWords stoppers;
        private readonly Action<string> log;

        private IState current;

        public States(IState start, IWords stoppers, Action<string> log)
        {
            this.start = start;
            this.stoppers = stoppers;
            this.log = log;

            current = start;
        }

        public Output Process(Input input)
        {
            var reset = current != start && stoppers.Match(input.Message); 
            if (reset)
            {
                current.Reset();
                current = start;
                return "Ауф!".ToOutput(); // TODO
            }
            
            var (answer, next) = current.Process(input);

            next ??= start;
            
            if (current != next)
                log($"{current} => {next}");
            
            current = next;
            
            return answer;
        }
    }
}