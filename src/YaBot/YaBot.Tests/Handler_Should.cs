﻿namespace YaBot.Tests
{
    using System.Threading;
    using System.Threading.Tasks;
    using App.Core;
    using NUnit.Framework;
    using Telegram.Bot.Types;

    [TestFixture]
    internal sealed class Handler_Should
    {
        [Test]
        public void IgnoreUpdate_WhenTypeIsNotText()
        {
            var called = false;
            var handler = new Handler(
                (_, _, _) => 
                { 
                    called = true;
                    return Task.CompletedTask;
                },
            _ => { });
            var update = new Update { Message = new Message() };

            handler.HandleUpdate(null, update, new CancellationToken()).Wait();

            Assert.That(called, Is.False);
        }
    }
}