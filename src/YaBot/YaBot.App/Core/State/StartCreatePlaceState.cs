namespace YaBot.App.Core.State
{
    using Extensions;
    using Telegram.Bot.Types;

    internal sealed class StartCreatePlaceState : IState
    {
        private readonly IWords input;
        private readonly IState next;

        public StartCreatePlaceState(IWords input, IState next)
        {
            this.input = input;
            this.next = next;
        }

        public bool IsInput(Message message)
        {
            return input.Match(message);
        }

        public (Answer, IState) Process(Message message)
        {
            return (
                "Введите название места. Будет здорово, если вы прикрепите его фотографию".ToAnswer(), // TODO
                next);
        }

        public IState Reset()
        {
            return this;
        }
    }
}