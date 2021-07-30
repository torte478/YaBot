﻿namespace YaBot.App.Extensions
{
    using Core;

    public static class AnswerExtensions
    {
        public static Answer ToAnswer(this string text)
        {
            return new() {Text = text};
        }
    }
}