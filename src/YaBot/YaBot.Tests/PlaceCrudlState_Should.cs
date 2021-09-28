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
        public void SetHeaderToOwnLine_OnListOperation()
        {
            var state = CreateForListTest(A.Fake<ICrudl<int, Place>>());

            // TODO : check first content, not count
            var count = state
                ._(GetTextOutput, A.Fake<IInput>())
                .Split(Environment.NewLine)
                .Length;
            
            Assert.That(count, Is.EqualTo(5));
        }

        [Test]
        public void PaginateNames_OnListOperation()
        {
            var place = A.Fake<Place>();
            A.CallTo(() => place.Name).Returns("EXPECTED");

            var places = A.Fake<ICrudl<int, Place>>();
            A.CallTo(() => places.Enumerate())
                .Returns(new[] { place });

            var state = CreateForListTest(places);

            var actual = state
                ._(GetTextOutput, A.Fake<IInput>())
                .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
                .Last();

            Assert.That(actual, Is.EqualTo("EXPECTED"));
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

        private static PlaceCrudlState CreateForReadTest(ICrudl<int, Place> places)
        {
            var keys = A.Fake<IWords>();
            A.CallTo(() => keys.Match(A<string>._)).Returns(true);

            return Create(
                new PlaceCrudlState.Keys
                {
                    Read = new PlaceCrudlState.StateKeys { Keys = keys }
                },
                places);
        }

        private static PlaceCrudlState CreateForListTest(ICrudl<int, Place> places)
        {
            var keys = A.Fake<IWords>();
            A.CallTo(() => keys.Match(A<string>._)).Returns(true);

            return Create(
                new PlaceCrudlState.Keys
                {
                    List = new PlaceCrudlState.StateKeys { Keys = keys }
                },
                places);
        }

        private static PlaceCrudlState Create(PlaceCrudlState.Keys keys, ICrudl<int, Place> places)
        {
            return new(
                keys,
                places,
                new OutputFactory(),
                (items, _) => new Pagination<string>
                {
                    Start = 1,
                    Finish = 2,
                    Paginated = false,
                    Total = 2,
                    Items = items.Select(x => (-1, x))
                }
            );
        }

        private static string GetTextOutput(IState state, IInput input)
        {
            var (output, _) = state.Process(input);
            return output.Text;
        }
    }
}