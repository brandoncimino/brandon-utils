using BrandonUtils.Standalone;
using BrandonUtils.Vectors;

using NUnit.Framework;

using UnityEngine;

namespace BrandonUtils.Tests.EditMode {
    public class Vector2Tests {
        [Test]
        [TestCase(10)]
        public void SetXTest(int iterations) {
            var vec = new Vector2(-1, -1);

            iterations.For(
                x_set => {
                    var x_prev = vec.x;

                    vec.SetX(x_set);

                    Assert.That(vec.x, Is.EqualTo(x_set));
                    Assert.That(vec.x, Is.Not.EqualTo(x_prev));
                }
            );
        }

        [Test]
        [TestCase(10)]
        public void SetYTest(int iterations) {
            var vec = new Vector2(-1, -1);

            iterations.For(
                y_set => {
                    var y_prev = vec.y;
                    vec.SetY(y_set);

                    Assert.That(vec.y, Is.EqualTo(y_set));
                    Assert.That(vec.y, Is.Not.EqualTo(y_prev));
                }
            );
        }

        [Test]
        [TestCase(1, 2)]
        public void SwapTest(float x_prev, float y_prev) {
            var vec = new Vector2(x_prev, y_prev);
            vec.Swap();

            Assert.That(vec.x, Is.EqualTo(y_prev));
            Assert.That(vec.y, Is.EqualTo(x_prev));
            Assert.That(vec.x, Is.Not.EqualTo(x_prev));
            Assert.That(vec.y, Is.Not.EqualTo(y_prev));
        }

        [Test]
        public void SwapSetChain() {
            var vec         = Vector2.one;
            var vec_initial = vec;
            var vec_set     = new Vector2(vec_initial.x + 1, vec_initial.y + 2);

            vec.SetX(vec_set.x).SetY(vec_set.y).Swap();

            Assert.That(vec.x, Is.EqualTo(vec_set.y));
            Assert.That(vec.y, Is.EqualTo(vec_set.x));
            Assert.That(vec.x, Is.Not.EqualTo(vec_initial.x));
            Assert.That(vec.y, Is.Not.EqualTo(vec_initial.y));
        }

        [Test]
        public void SortTest() {
            var vec = new Vector2(10, 1);
            vec.Sort();

            Assert.That(vec.x, Is.LessThanOrEqualTo(vec.y));
            Assert.That(vec.y, Is.GreaterThanOrEqualTo(vec.x));
        }

        [Test]
        public void SortChain() {
            var vec = new Vector2(10, 1);
            vec.Sort().SetX(vec.y + 1).Sort();

            Assert.That(vec.x, Is.LessThanOrEqualTo(vec.y));
            Assert.That(vec.y, Is.GreaterThanOrEqualTo(vec.x));
        }

        [Test]
        public void SortedDoesNotSort() {
            var unsorted = new Vector2(10, 1);
            var sorted   = unsorted.Sorted();

            Assert.That(unsorted != sorted);
            Assert.That(sorted.x, Is.LessThanOrEqualTo(sorted.y));
            Assert.That(sorted.y, Is.GreaterThanOrEqualTo(sorted.x));
            Assert.That(sorted.x, Is.EqualTo(unsorted.y));
            Assert.That(sorted.y, Is.EqualTo(unsorted.x));
        }

        [Test]
        public void SwappedDoesNotSwap() {
            var unswapped = new Vector2(1, 2);
            var swapped   = unswapped.Swapped();

            Assert.That(swapped.x, Is.EqualTo(unswapped.y));
            Assert.That(swapped.y, Is.EqualTo(unswapped.x));
        }
    }
}
