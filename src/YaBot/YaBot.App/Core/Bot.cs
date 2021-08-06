namespace YaBot.App.Core
{
    using System;
    using System.Collections.Generic;
    using Outputs;

    public sealed class Bot
    {
        private readonly Func<Func<IInput, IOutput>> createReceiver;
        private readonly DateTime begin;

        private readonly Dictionary<long, Func<IInput, IOutput>> process = new();

        public Bot(Func<Func<IInput, IOutput>> createReceiver, DateTime begin)
        {
            this.createReceiver = createReceiver;
            this.begin = begin;
        }

        public IOutput Receive(IInput input)
        {
            if (input.Date < begin)
                return null;
            
            try
            {
                if (!process.ContainsKey(input.Chat))
                    process.Add(input.Chat, createReceiver());

                return process[input.Chat](input);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}