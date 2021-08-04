namespace YaBot.App.Core.State
{
    using Extensions;

    internal sealed class StartCreatePlaceState : IState
    {
        private readonly IWords keys;
        private readonly IWords intro;
        private readonly IState next;

        public StartCreatePlaceState(IWords keys, IWords intro, IState next)
        {
            this.keys = keys;
            this.intro = intro;
            this.next = next;
        }

        public bool IsInput(Input input)
        {
            return keys.Match(input.Message);
        }

        public (Output, IState) Process(Input input)
        {
            return (
                intro.ToRandom().ToOutput(),
                next);
        }

        public IState Reset()
        {
            return this;
        }
    }
}