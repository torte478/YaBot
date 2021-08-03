namespace YaBot.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using App.Core.Database;
    using App.Core.State;
    using App.Extensions;
    using NUnit.Framework;

    [TestFixture]
    internal sealed class GetRandomPlaceState_Should
    {
        [Test]
        public void EnumerateAllPlaces_AfterCallLengthTime()
        {
            var state = new GetRandomPlaceState(
                null,
                () => Enumerable.Range(1, 3).Select(_ => new Place {Name = _.ToString()}));
            var actual = new List<string>();

            for (var i = 0; i < 3; ++i)
                state
                    .Process(null)
                    ._(_ => actual.Add(_.Item1.Text));
            
            Assert.That(actual, Is.EquivalentTo(new[] { "1", "2", "3" }));
        }

        [Test]
        public void ReturnItSelf_WhenPlaceHasNotPhoto()
        {
            var state = new GetRandomPlaceState(
                null,
                () => new[] { new Place() });

            var (_, actual) = state.Process(null);

            Assert.That(actual, Is.EqualTo(state));
        }
        
        [Test]
        public void ReturnItSelf_WhenPlaceHasPhoto()
        {
            var state = new GetRandomPlaceState(
                null,
                () => new[] { new Place { Image = Array.Empty<byte>() } });

            var (_, actual) = state.Process(null);

            Assert.That(actual, Is.EqualTo(state));
        }
    }
}