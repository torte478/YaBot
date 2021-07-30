namespace YaBot.App.Core.State
{
    public interface IState
    {
        (Answer, IState) Process(IWords words);
        IState Reset();
    }
}