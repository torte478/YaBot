namespace YaBot.Tests.App
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FakeItEasy;
    using NUnit.Framework;
    using YaBot.App.Core;
    using YaBot.App.Core.Database;
    using YaBot.App.Core.State.Impl;
    using YaBot.Extensions;
    using YaBot.IO;
    using YaBot.Tests.Fake;

    [TestFixture]
    internal sealed class GetRandomPlaceState_Should
    {
        private IInput input;

        [SetUp]
        public void SetUp()
        {
            input = A.Fake<IInput>();
            A.CallTo(() => input.Text).Returns(string.Empty);
        }
        
        [Test]
        public void EnumerateAllPlaces_AfterCallLengthTime()
        {
            var state = Create(() => Enumerable
                .Range(1, 3)
                .Select(_ => new Place
                {
                    Name = _.ToString()
                }));
                
            var actual = new List<string>();
            for (var i = 0; i < 3; ++i)
                state
                    .Process(input)
                    ._(_ => actual.Add(_.Item1.Text));
            
            Assert.That(actual, Is.EquivalentTo(new[] { "1", "2", "3" }));
        }

        [Test]
        public void ReturnItself_WhenPlaceHasNotPhoto()
        {
            var state = Create(() => new[] {new Place()});

            var (_, actual) = state.Process(input);

            Assert.That(actual, Is.EqualTo(state));
        }
        
        [Test]
        public void ReturnItself_WhenPlaceHasPhoto()
        {
            var state = Create(() => new[] { new Place { Image = Array.Empty<byte>() } });

            var (_, actual) = state.Process(input);

            Assert.That(actual, Is.EqualTo(state));
        }

        [Test]
        public void ReturnNotEmptyOutput_WhenAnyKeysMatch()
        {
            var state = Create(() => new[] {new Place{ Name = "expected" }});

            var actual = state.Process(input).Item1.Text;
            
            Assert.That(actual, Is.EqualTo("expected"));
        }

        private static GetRandomPlaceState Create(Func<IEnumerable<Place>> getPlaces)
        {
            var keys = A.Fake<IWords>();
            A.CallTo(() => keys.Match(A<string>._, false)).Returns(false);
            var words = A.Fake<IWords>();
            A.CallTo(() => words.Match(A<string>._, false)).Returns(true);
            return new GetRandomPlaceState(
                keys,
                words,
                getPlaces,
                new FakeOutputFactory());
        }
    }
}