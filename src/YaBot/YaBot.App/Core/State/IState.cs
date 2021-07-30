namespace YaBot.App.Core.State
{
    internal interface IState
    {
        (string, IState) Process(IWords words);
        IState Reset();
    }
}