namespace YaBot.App.Extensions
{
    using System;
    using System.Linq;
    using Core;

    public static class WordsExtensions
    {
        private static readonly Random random = new(DateTime.Now.Millisecond);

        public static string ToRandom(this IWords words)
        {
            var variants = words.ToList();

            return variants.Count > 0
                ? variants[random.Next(variants.Count)]
                : string.Empty;
        }

        public static string ToError(this IWords words, string message)
        {
            return $"{words.ToRandom()} {message}";
        }
    }
}