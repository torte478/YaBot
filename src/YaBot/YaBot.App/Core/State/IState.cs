namespace YaBot.App.Core.State
{
    using Telegram.Bot.Types;

    public interface IState
    {
        bool IsInput(Input input);
        (Output, IState) Process(Input input);
        IState Reset();
    }
}