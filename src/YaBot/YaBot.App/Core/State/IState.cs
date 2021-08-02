namespace YaBot.App.Core.State
{
    using Telegram.Bot.Types;

    public interface IState
    {
        bool IsInput(Message message);
        (Answer, IState) Process(Message message);
        IState Reset();
    }
}