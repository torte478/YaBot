namespace YaBot
{
    public interface IHasId<out T>
    {
        public T Id { get; }
    }
}