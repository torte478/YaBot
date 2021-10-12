namespace YaBot.Tests
{
    using System;
    using NUnit.Framework;
    using YaBot.IO;

    [TestFixture]
    internal sealed class Output_Should
    {
        [Test]
        public void ReturnFalseIsText_WhenItIsImage()
        {
            var output = new Output
            {
                Image = Array.Empty<byte>(),
                Text = "text"
            };

            Assert.That(output.IsText, Is.False);
        }
    }
}