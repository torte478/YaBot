namespace YaBot.App.Core.State
{
    using Telegram.Bot.Types;

    internal sealed class FinishCreatePlaceState : IState
    {
        public bool IsInput(Message message)
        {
            // TODO : is text with image(?)
            return true;
        }

        public (Answer, IState) Process(Message message)
        {
            throw new System.NotImplementedException();
        }

        public IState Reset()
        {
            return this;
        }
    }
}