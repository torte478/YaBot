namespace YaBot.App.Core
{
    public interface IHasId<out T>
    {
        public T Id { get; }
    }
}