namespace YaBot.Core
{
    using System;
    using System.Collections.Generic;
    using YaBot.Core.IO;

    public sealed class Bot
    {
        private readonly Func<Func<IInput, IOutput>> createReceiver;
        private readonly DateTime begin;

        private readonly Dictionary<long, Func<IInput, IOutput>> chats = new();

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
                if (!chats.ContainsKey(input.Chat))
                    chats.Add(input.Chat, createReceiver());

                return chats[input.Chat](input);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}