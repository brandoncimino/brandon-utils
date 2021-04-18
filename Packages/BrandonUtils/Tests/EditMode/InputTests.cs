using BrandonUtils.Input;
using BrandonUtils.Standalone.Exceptions;

using NUnit.Framework;

using UnityEngine;

using static UnityEngine.KeyCode;

namespace BrandonUtils.Tests.EditMode {
    public class InputTests {
        private static KeyCode[] NonDirectionalKeys = {
            Comma,
            U,
            KeyCode.Space,
            Tilde,
            Ampersand,
            LeftAlt,
            F4,
        };

        private static KeyCode[] WASD      = {W, A, S, D};
        private static KeyCode[] ArrowKeys = {UpArrow, DownArrow, LeftArrow, RightArrow};

        [Test]
        public void InvalidArrowKeyToWASD(
            [ValueSource(nameof(NonDirectionalKeys))] [ValueSource(nameof(WASD))]
            KeyCode nonArrowKey
        ) {
            Assume.That(ArrowKeys, Does.Not.Contains(nonArrowKey));
            Assert.Throws<EnumNotInSubsetException<KeyCode>>(() => nonArrowKey.ToWASD());
        }

        [Test]
        public void InvalidWASDToArrowKey(
            [ValueSource(nameof(NonDirectionalKeys))] [ValueSource(nameof(ArrowKeys))]
            KeyCode nonWASDKey
        ) {
            Assume.That(WASD, Does.Not.Contains(nonWASDKey));
            Assert.Throws<EnumNotInSubsetException<KeyCode>>(() => nonWASDKey.ToArrowKey());
        }

        [TestCase(W, UpArrow)]
        [TestCase(A, LeftArrow)]
        [TestCase(S, DownArrow)]
        [TestCase(D, RightArrow)]
        public void ValidWASDToArrowKey(
            KeyCode wasd,
            KeyCode arrow
        ) {
            Assume.That(WASD,      Contains.Item(wasd));
            Assume.That(ArrowKeys, Contains.Item(arrow));
            Assert.That(wasd.ToArrowKey(), Is.EqualTo(arrow));
        }

        [TestCase(UpArrow,    W)]
        [TestCase(LeftArrow,  A)]
        [TestCase(DownArrow,  S)]
        [TestCase(RightArrow, D)]
        public void ValidArrowToWASD(KeyCode arrow, KeyCode wasd) {
            Assume.That(ArrowKeys, Contains.Item(arrow));
            Assume.That(WASD,      Contains.Item(wasd));
            Assert.That(arrow.ToWASD(), Is.EqualTo(wasd));
        }
    }
}