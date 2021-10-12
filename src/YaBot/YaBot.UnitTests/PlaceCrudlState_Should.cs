namespace YaBot.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FakeItEasy;
    using NUnit.Framework;
    using YaBot.App.Core.State.Impl;
    using YaBot;
    using YaBot.App.Core;
    using YaBot.App.Core.Database;
    using YaBot.App.Core.State;
    using YaBot.Extensions;
    using YaBot.IO;
    using YaBot.Tests.Fake;

    [TestFixture]
    internal sealed class PlaceCrudlState_Should
    {
        [Test]
        public void SetHeaderToOwnLine_OnListOperation()
        {
            var state = CreateForListTest();

            var title = state
                ._(GetTextOutput, A.Fake<IInput>())
                .Split(Environment.NewLine)
                .First();

            Assert.That(title.Contains("2..3 из 3"), Is.True);
        }

        [Test]
        public void PaginateNames_OnListOperation()
        {
            var place = A.Fake<Place>();
            A.CallTo(() => place.Name).Returns("EXPECTED");

            var places = A.Fake<ICrudl<int, Place>>();
            A.CallTo(() => places.All())
                .Returns(place._(ToQuery));

            var state = CreateForListTest(places);

            var actual = state
                ._(GetTextOutput, A.Fake<IInput>())
                .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
                .Last();

            Assert.That(actual.Contains("EXPECTED"), Is.True);
        }

        [Test]
        public void IncrementIndices_OnListOperation()
        {
            var place = A.Fake<Place>();
            A.CallTo(() => place.Name).Returns("EXPECTED");

            var places = A.Fake<ICrudl<int, Place>>();
            A.CallTo(() => places.All())
                .Returns(place._(ToQuery));

            var state = CreateForListTest(places);

            var actual = state
                ._(GetTextOutput, A.Fake<IInput>())
                .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
                .Last();

            Assert.That(actual, Is.EqualTo("43: EXPECTED"));
        }

        [Test]
        public void IncrementIndicesAtHeader_OnListOperation()
        {
            var state = CreateForListTest();

            var actual = state
                ._(GetTextOutput, A.Fake<IInput>())
                .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
                .First();

            Assert.That(actual.Contains("1"), Is.False);
            Assert.That(actual.Contains("2"), Is.True);
            Assert.That(actual.Contains("3"), Is.True);
        }

        [Test]
        public void NotChangeState_AfterListOperation()
        {
            var keys = A.Fake<IWords>();
            A.CallTo(() => keys.Match(A<string>._, false)).Returns(true);

            IEnumerable<string> text = new[] { "ERROR" };
            var error = A.Fake<IWords>();
            A.CallTo(() => error.GetEnumerator()).Returns(text.GetEnumerator());

            var state = Create(new PlaceCrudlState.Keys
                {
                    List = new PlaceCrudlState.StateKeys { Keys = keys },
                    Error = error
                });

            state.Process(A.Fake<IInput>());
             var (actual, _) = state.Process(A.Fake<IInput>());

            Assert.That(actual.Text.Contains("ERROR"), Is.False);
        }

        [Test]
        public void CloseState_AfterListOperation()
        {
            var state = CreateForListTest();

            state.Process(A.Fake<IInput>());
            var (_, actual) = state.Process(A.Fake<IInput>());

            Assert.That(actual, Is.Null);
        }

        [Test]
        public void UseMetaIndices_OnReadOperation()
        {
            var place = new Place { Id = 42, Name = "EXPECTED" };
            var places = A.Fake<ICrudl<int, Place>>();
            A.CallTo(() => places.All()).Returns(place._(ToQuery));

            var input = A.Fake<IInput>();
            A.CallTo(() => input.Text).Returns("1");

            var state = CreateForReadTest(places);

            state.Process(A.Fake<IInput>());
            var actual = state._(GetTextOutput, input);

            Assert.That(actual, Is.EqualTo("EXPECTED"));
        }

        private static PlaceCrudlState CreateForReadTest(ICrudl<int, Place> places)
        {
            var keys = A.Fake<IWords>();
            A.CallTo(() => keys.Match(A<string>._, false)).Returns(true);

            return Create(
                new PlaceCrudlState.Keys
                {
                    Read = new PlaceCrudlState.StateKeys { Keys = keys }
                },
                places);
        }

        private static PlaceCrudlState CreateForListTest(ICrudl<int, Place> places = null)
        {
            var keys = A.Fake<IWords>();
            A.CallTo(() => keys.Match(A<string>._, false)).Returns(true);

            return Create(
                new PlaceCrudlState.Keys
                {
                    List = new PlaceCrudlState.StateKeys { Keys = keys }
                },
                places);
        }

        private static PlaceCrudlState Create(PlaceCrudlState.Keys keys, ICrudl<int, Place> places = null)
        {
            if (places == null)
            {
                places = A.Fake<ICrudl<int, Place>>();
                A.CallTo(() => places.All())
                    .Returns(Enumerable.Empty<Place>().AsQueryable());
            }

            return new(
                keys,
                places,
                new FakeOutputFactory(),
                (items, _) => new Pagination<string>
                {
                    Start = 1,
                    Finish = 2,
                    Paginated = true,
                    Total = 3,
                    Items = items.AsEnumerable().Select(x => (42, x))
                },
                _ => _
            );
        }

        private static string GetTextOutput(IState state, IInput input)
        {
            var (output, _) = state.Process(input);
            return output.Text;
        }

        private static IQueryable<Place> ToQuery(Place place)
        {
            return new[] { place }.AsQueryable();
        }
    }
}