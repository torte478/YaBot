namespace YaBot.App.Core.State
{
    internal sealed class FindPlace : IState
    {
        public (string, IState) Process(IWords words)
        {
            return ("TODO", this);
        }

        public IState Reset()
        {
            return this; // TODO
        }

        public override string ToString()
        {
            return "FindPlace";
        }
    }
}