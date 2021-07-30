namespace YaBot.App.Extensions
{
    using System;

    public static class FuncExtensions
    {
        public static TOut _<TIn, TOut>(this TIn x, Func<TIn, TOut> f)
        {
            return f(x);
        }
        
        public static TOut _<TFirst, TSecond, TOut>(this TFirst x, Func<TFirst, TSecond, TOut> f, TSecond y)
        {
            return f(x, y);
        }
        
        public static TIn _<TIn>(this TIn x, Action<TIn> f)
        {
            f(x);
            return x;
        }
    }
}