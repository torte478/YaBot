namespace YaBot.Tests
{
    using System;
    using System.Linq;
    using App.Core;
    using App.Core.Database;
    using App.Core.State;
    using App.TelegramApi;
    using FakeItEasy;
    using FakeItEasy.Configuration;
    using Microsoft.EntityFrameworkCore.Diagnostics;
    using NUnit.Framework;
    using Telegram.Bot.Types;

    [TestFixture]
    internal sealed class PlaceCrudlState_Should
    {
        [Test]
        public void SetHeaderToOwnLine_AfterListFormat()
        {
            var keys = A.Fake<IWords>();
            A.CallTo(() => keys.Match(A<string>._)).Returns(true);
            var places = A.Fake<ICrudl<int, Place>>();
            A.CallTo(() => places.Enumerate()).Returns(
                Enumerable.Range(1, 3).Select(_ => A.Fake<Place>()));
            
            var state = new PlaceCrudlState(
                new PlaceCrudlState.Keys
                {
                    List = new PlaceCrudlState.StateKeys
                    {
                        Keys = keys
                    }
                },
                places,
                Output.Create,
                Output.Create,
                Output.Create
                );

            var count = state.Process(A.Fake<IInput>())
                .Item1
                .Text
                .Split(Environment.NewLine)
                .Length;
            
            Assert.That(count, Is.EqualTo(5));
        }

        [Test]
        public void UseMetaIndicies_OnReadOperation()
        {
            var keys = A.Fake<IWords>();
            A.CallTo(() => keys.Match(A<string>._)).Returns(true);
            
            var place = new Place { Id = 42, Name = "EXPECTED" };
            var places = A.Fake<ICrudl<int, Place>>();
            A.CallTo(() => places.Enumerate()).Returns(new[] { place });

            var input = A.Fake<IInput>();
            A.CallTo(() => input.Text).Returns("0");
            
            var state = new PlaceCrudlState(
                new PlaceCrudlState.Keys
                {
                    Read = new PlaceCrudlState.StateKeys
                    {
                        Keys = keys
                    }
                },
                places,
                Output.Create,
                Output.Create,
                Output.Create
                );

            state.Process(A.Fake<IInput>());
            var actual = state
                .Process(input)
                .Item1
                .Text;

            Assert.That(actual, Is.EqualTo("EXPECTED"));
        }
    }
}