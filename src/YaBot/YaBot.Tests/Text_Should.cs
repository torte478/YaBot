namespace YaBot.Tests
{
    using System.Linq;
    using App.Core;
    using NUnit.Framework;

    [TestFixture]
    internal sealed class Text_Should
    {
        [Test]
        public void ConvertStringToLowerCase_BeforeParse()
        {
            var text = new Text(Words.Create);

            var actual = text.Parse("TEST");
            
            Assert.That(actual.Single(), Is.EqualTo("test"));
        }
    }
}