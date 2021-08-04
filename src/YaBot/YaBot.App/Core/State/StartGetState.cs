namespace YaBot.App.Core.State
{
    using Extensions;

    public sealed class StartGetState : IState
    {
        private readonly IState next;
        private readonly IWords keys;
        private readonly IWords intro;

        public StartGetState(IWords keys, IWords intro, IState next)
        {
            this.next = next;
            this.keys = keys;
            this.intro = intro;
        }

        public bool IsInput(Input input)
        {
            return keys.Match(input.Message);
        }

        public (Output, IState) Process(Input input)
        {
            return (intro.ToRandom().ToOutput(), next);
        }

        public IState Reset()
        {
            return this;
        }
    }
}