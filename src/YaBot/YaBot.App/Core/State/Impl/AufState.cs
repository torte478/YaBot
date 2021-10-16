namespace YaBot.App.Core.State.Impl
{
    using YaBot.App.Extensions;
    using YaBot.IO;

    public sealed class AufState : BaseState
    {
        private readonly IWords response;
        private readonly bool substring;

        public AufState(IWords keys, IOutputFactory<string> output, IWords response, bool substring = true)
            : base(keys, output)
        {
            this.response = response;
            this.substring = substring;
        }

        public override string Name => "Auf";

        public override bool IsInput(IInput input)
        {
            return Keys.Match(input.Text, substring);
        }

        protected override string InnerProcess(IInput input)
        {
            return response.ToRandom();
        }
    }
}