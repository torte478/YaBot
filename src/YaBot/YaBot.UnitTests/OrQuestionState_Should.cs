namespace YaBot.Tests
{
    using FakeItEasy;
    using NUnit.Framework;
    using YaBot.App.Core;
    using YaBot.App.Core.Outputs;
    using YaBot.App.Core.State;
    using YaBot.Core.IO;

    [TestFixture]
    internal sealed class OrQuestionState_Should
    {
        [Test]
        [TestCase("bot a или b?", true)]
        [TestCase("a или b?", false)]
        [TestCase("aилиb?", false)]
        [TestCase("aилиb ?", false)]
        [TestCase("bot  или ?", false)]
        [TestCase("bot aaa или bbbb?", true)]
        public void CorrectMathInput_OnAnyMessage(string message, bool expected)
        {
            var state = new OrQuestionState(
                A.Fake<IWords>(),
                A.Fake<IOutputFactory<string>>(),
                () => 0);

            var input = A.Fake<IInput>();
            A.CallTo(() => input.Text).Returns(message);

            var actual = state.IsInput(input);

            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}