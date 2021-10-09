namespace YaBot.App.Core.State
{
    using YaBot.App.Core.Outputs;
    using YaBot.App.Extensions;
    using YaBot.Core.Extensions;
    using YaBot.Core.IO;

    public sealed class AufState : IState
    {
        private readonly IWords keys;
        private readonly IWords response;
        private readonly IOutputFactory<string> output;

        public AufState(IWords keys, IWords response, IOutputFactory<string> output)
        {
            this.keys = keys;
            this.response = response;
            this.output = output;
        }

        public string Name => "Auf";

        public bool IsInput(IInput input)
        {
            return keys.Match(input.Text, true);
        }

        public (IOutput, IState) Process(IInput input)
        {
            return (response.ToRandom()._(output.Create), null);
        }

        public IState Reset()
        {
            return this;
        }
    }
}