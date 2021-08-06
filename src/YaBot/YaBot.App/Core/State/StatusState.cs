namespace YaBot.App.Core.State
{
    using System;
    using System.Text;
    using Extensions;
    using Outputs;

    internal class StatusState : IState
    {
        private readonly string version;
        private readonly IWords keys;
        private readonly IWords success;
        private readonly Func<string, IOutput> toOutput;

        public StatusState(string version, IWords keys, IWords success, Func<string, IOutput> toOutput)
        {
            this.version = version;
            this.keys = keys;
            this.success = success;
            this.toOutput = toOutput;
        }

        public bool IsInput(IInput input)
        {
            return keys.Match(input.Text);
        }

        public (IOutput, IState) Process(IInput input)
        {
            var answer = new StringBuilder()
                .AppendLine(success.ToRandom())
                .Append("Версия: ")
                .AppendLine(version)
                .ToString();

            return (answer._(toOutput), null);
        }

        public IState Reset()
        {
            return this;
        }
    }
}