namespace YaBot.Core
{
    public interface IHasId<out T>
    {
        public T Id { get; }
    }
}