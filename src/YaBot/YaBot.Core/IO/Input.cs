namespace YaBot.Core.IO
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Telegram.Bot;
    using Telegram.Bot.Types;

    public sealed class Input : IInput
    {
        public DateTime Date { get; init; }
        public long Chat { get; init;}
        
        public string Text { get; set;}
        public byte[] Image { get; set;}
        
        public bool IsImage => Image != null;
    }
}