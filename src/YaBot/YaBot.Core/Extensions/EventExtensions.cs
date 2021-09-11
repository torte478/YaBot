namespace YaBot.Core.Extensions
{
    using System;

    public static class EventExtensions
    {
        public static void Raise(this Action action)
        {
            var handler = action;
            handler?.Invoke();
        }
        
        public static void Raise<T>(this Action<T> action, T x)
        {
            var handler = action;
            handler?.Invoke(x);
        }
        
        public static void Raise<T1, T2>(this Action<T1, T2> action, T1 x1, T2 x2)
        {
            var handler = action;
            handler?.Invoke(x1, x2);
        }
    }
}