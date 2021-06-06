using System;

using BrandonUtils.Standalone;
using BrandonUtils.Standalone.Collections;

using NUnit.Framework;

namespace BrandonUtils.Tests.Standalone {
    /// <summary>
    /// Tests for <see cref="Bloop"/>s
    /// </summary>
    public class BloopTests {
        [Test]
        [TestCase(5)]
        public void RepeatRandom(int numberOfPicks) {
            var random     = new Random();
            var picks      = numberOfPicks.Repeat(() => random.Next());
            var pickGroups = picks.Group();
            Assert.That(pickGroups, Has.Count.EqualTo(numberOfPicks));
        }
    }
}
