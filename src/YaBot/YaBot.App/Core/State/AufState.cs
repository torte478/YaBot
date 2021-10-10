namespace YaBot.App.Core.State
{
    using YaBot.App.Core.Outputs;
    using YaBot.App.Extensions;
    using YaBot.Core.IO;

    public sealed class AufState : BaseState
    {
        private readonly IWords response;

        public AufState(IWords keys, IOutputFactory<string> output, IWords response)
            : base(keys, output)
        {
            this.response = response;
        }

        public override string Name => "Auf";

        protected override string InnerProcess(IInput input)
        {
            return response.ToRandom();
        }
    }
}