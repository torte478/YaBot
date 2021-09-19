namespace YaBot.Tests
{
    using System;
    using System.Linq;
    using App.Core;
    using App.Core.Database;
    using App.Core.State;
    using App.TelegramApi;
    using FakeItEasy;
    using NUnit.Framework;
    using YaBot.Core;
    using YaBot.Core.Extensions;
    using YaBot.Core.IO;

    [TestFixture]
    internal sealed class PlaceCrudlState_Should
    {
        [Test]
        public void SetHeaderToOwnLine_AfterListFormat()
        {
            var places = A.Fake<ICrudl<int, Place>>();
            A.CallTo(() => places.Enumerate()).Returns(
                Enumerable.Range(1, 3).Select(_ => A.Fake<Place>()));
            var state = CreateForListTest(places);

            // TODO : check first content, not count
            var count = state
                ._(GetTextOutput, A.Fake<IInput>())
                .Split(Environment.NewLine)
                .Length;
            
            Assert.That(count, Is.EqualTo(6));
        }

        [Test]
        public void UseMetaIndices_OnReadOperation()
        {
            var place = new Place { Id = 42, Name = "EXPECTED" };
            var places = A.Fake<ICrudl<int, Place>>();
            A.CallTo(() => places.Enumerate()).Returns(new[] { place });

            var input = A.Fake<IInput>();
            A.CallTo(() => input.Text).Returns("0");

            var state = CreateForReadTest(places);

            state.Process(A.Fake<IInput>());
            var actual = state._(GetTextOutput, input);

            Assert.That(actual, Is.EqualTo("EXPECTED"));
        }

        private static PlaceCrudlState CreateForReadTest(ICrudl<int, Place> places, int pagination = 100)
        {
            var keys = A.Fake<IWords>();
            A.CallTo(() => keys.Match(A<string>._)).Returns(true);

            return new PlaceCrudlState(
                new PlaceCrudlState.Keys
                {
                    Read = new PlaceCrudlState.StateKeys { Keys = keys }
                },
                places,
                new OutputFactory(),
                new Page(pagination).Create // TODO : wtf?
            );
        }

        private static PlaceCrudlState CreateForListTest(ICrudl<int, Place> places, int pagination = 100)
        {
            var keys = A.Fake<IWords>();
            A.CallTo(() => keys.Match(A<string>._)).Returns(true);

            return new PlaceCrudlState(
                new PlaceCrudlState.Keys
                {
                    List = new PlaceCrudlState.StateKeys { Keys = keys }
                },
                places,
                new OutputFactory(),
                new Page(pagination).Create // TODO : wtf?
            );
        }

        private static string GetTextOutput(IState state, IInput input)
        {
            var (output, _) = state.Process(input);
            return output.Text;
        }
    }
}