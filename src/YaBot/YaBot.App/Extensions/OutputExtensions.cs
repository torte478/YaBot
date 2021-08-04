namespace YaBot.App.Extensions
{
    using Core;

    public static class AnswerExtensions
    {
        public static Output ToOutput(this string text)
        {
            return new() {Text = text};
        }

        public static Output ToRandomOutput(this IWords words)
        {
            return words.ToRandom().ToOutput();
        }
    }
}