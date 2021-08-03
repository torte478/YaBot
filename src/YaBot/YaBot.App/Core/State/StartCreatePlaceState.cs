namespace YaBot.App.Core.State
{
    using Extensions;
    using Telegram.Bot.Types;

    internal sealed class StartCreatePlaceState : IState
    {
        private readonly IWords keys;
        private readonly IState next;

        public StartCreatePlaceState(IWords keys, IState next)
        {
            this.keys = keys;
            this.next = next;
        }

        public bool IsInput(Input input)
        {
            return keys.Match(input.Message);
        }

        public (Output, IState) Process(Input input)
        {
            return (
                "Введите название места. Будет здорово, если вы прикрепите его фотографию".ToOutput(), // TODO
                next);
        }

        public IState Reset()
        {
            return this;
        }
    }
}