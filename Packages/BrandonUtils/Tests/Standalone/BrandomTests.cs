using System;
using System.Collections.Generic;
using System.Linq;

using BrandonUtils.Standalone;
using BrandonUtils.Standalone.Collections;
using BrandonUtils.Standalone.Randomization;
using BrandonUtils.Testing;

using NUnit.Framework;

namespace BrandonUtils.Tests.Standalone {
    public class BrandomTests {
        [Test]
        [TestCase(7, 1000)]
        public void FromList(int numberOfChoices, int numberOfPicks) {
            var ls = Enumerable.Range(0, numberOfChoices).ToList();
            var v2 = new FromList<int>(ls);

            var picks = numberOfPicks.Repeat<int>(() => v2).ToList();

            Assert.That(picks, Is.SupersetOf(ls));

            var groups                = picks.Group();
            var expectedHitsPerChoice = (double) numberOfPicks / numberOfChoices;

            Assert.That(groups, Has.All.Values().Approximately(expectedHitsPerChoice, expectedHitsPerChoice / 2));
        }

        private static Dictionary<string, int> BuildWeighted(params int[] weights) {
            return Enumerable.Range(0, weights.Count()).ToDictionary(it => $"[{it}]", it => weights[it]);
        }

        private static Dictionary<T, double> BuildWeightPortion<T>(Dictionary<T, double> weighted) {
            var totalWeight = weighted.Values.Sum();
            return weighted.ToDictionary(it => it.Key, it => (double) it.Value / totalWeight);
        }

        private static Dictionary<T, double> BuildWeightedPortion<T>(Dictionary<T, int> weighted) {
            var totalWeight = weighted.Values.Sum();
            Console.WriteLine(totalWeight);
            return weighted.ToDictionary(
                it => it.Key,
                it => (double) it.Value / totalWeight
            );
        }

        private static Dictionary<T, float> BuildWeightedPortion<T>(Dictionary<T, float> weighted) {
            var totalWeight = weighted.Values.Sum();
            return weighted.ToDictionary(it => it.Key, it => it.Value / totalWeight);
        }

        [TestCase(1000, new int[] {1, 1, 1, 5, 2, 3, 7, 8})]
        public void FromWeightedList(int numberOfPicks, params int[] weights) {
            var weighted = BuildWeighted(weights);

            var picks = numberOfPicks.Repeat(it => Brandom.FromWeightedList(weighted)).ToList();

            AssertPicks(picks, weighted);
        }

        private static void AssertPicks(List<string> picks, Dictionary<string, int> weighted) {
            Assert.That(picks, Is.SupersetOf(weighted.Keys));

            var groups = picks.Group();
            var expectedHits = BuildWeightedPortion(weighted)
                .ToDictionary(
                    it => it.Key,
                    it => picks.Count * it.Value
                );

            foreach (var g in groups) {
                Assert.That(g, Has.Property(nameof(KeyValuePair<int, int>.Value)).Approximately(expectedHits[g.Key], picks.Count / 10f));
            }
        }

        [Test]
        [TestCase(1000, new[] {1, 2, 3, 4, 15, 3})]
        public void RandomizedFromWeightedList(int numberOfPicks, params int[] weights) {
            var weighted = BuildWeighted(weights);

            var fromWeightedList = Randomized.FromWeightedList(weighted);

            var picks = numberOfPicks.Repeat(fromWeightedList.Get).ToList();

            AssertPicks(picks, weighted);
        }
    }
}
