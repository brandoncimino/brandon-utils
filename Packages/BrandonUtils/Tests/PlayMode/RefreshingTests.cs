using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

using BrandonUtils.Logging;
using BrandonUtils.Refreshing;
using BrandonUtils.Standalone;
using BrandonUtils.Standalone.Chronic;
using BrandonUtils.Standalone.Refreshing;

using NUnit.Framework;

using UnityEngine;
using UnityEngine.TestTools;

using Random = UnityEngine.Random;

namespace BrandonUtils.Tests.PlayMode {
    public class RefreshingTests {
        private static class Validations {
            public static void ForcedRefresh<T>(Refreshing<T, int> refreshing) where T : IEquatable<T> {
                var initialCount = refreshing.RefreshCount;
                refreshing.Refresh();
                Assert.That(refreshing, Has.Property(nameof(refreshing.RefreshCount)).EqualTo(initialCount + 1));
            }
        }

        [UnityTest]
        public IEnumerator PerFameTest() {
            var firstFrame  = Time.frameCount;
            var countedFunc = new CountedFunc<int>(() => Time.frameCount);
            var perFrame    = Per.Frame(countedFunc.Invoke);

            Assert.That(perFrame.Peek(), Is.EqualTo(default(int)), $"Before any retrievals of {nameof(perFrame.Value)}, the value should be default");

            for (int f = 0; f < 10; f++) {
                // LogUtils.Log($"frame: {f}, {Time.frameCount}");
                Assert.That(countedFunc, Has.Property(nameof(countedFunc.InvocationCount)).EqualTo(f));
                Assert.That(perFrame,    Has.Property(nameof(perFrame.Freshness)).EqualTo(Freshness.Stale));
                Assert.That(perFrame,    Has.Property(nameof(perFrame.RefreshCount)).EqualTo(f));

                for (int intraframe = 0; intraframe < 10; intraframe++) {
                    // LogUtils.Log($"{nameof(frame)}: {frame}.{intraframe}");
                    Assert.That(perFrame,    Has.Property(nameof(perFrame.Value)).EqualTo(f + firstFrame));
                    Assert.That(perFrame,    Has.Property(nameof(perFrame.RefreshCount)).EqualTo(f + 1));
                    Assert.That(perFrame,    Has.Property(nameof(perFrame.Freshness)).EqualTo(Freshness.Fresh));
                    Assert.That(countedFunc, Has.Property(nameof(countedFunc.InvocationCount)).EqualTo(f + 1));
                }

                yield return null;
            }
        }

        [Test]
        public void ForcedRefreshTest() {
            var countedFunc = new CountedFunc<int>(() => Time.frameCount);
            var perFrame    = Per.Frame(countedFunc.Invoke);
            for (int intraframe = 0; intraframe < 10; intraframe++) {
                Assert.That(countedFunc, Has.Property(nameof(countedFunc.InvocationCount)).EqualTo(intraframe));
                Validations.ForcedRefresh(perFrame);
                Assert.That(countedFunc, Has.Property(nameof(countedFunc.InvocationCount)).EqualTo(intraframe + 1));
            }
        }

        [Test]
        public void Efficiency() {
            int          iterations = 1;
            Func<double> func       = () => Random.value;
            var          sw         = Stopwatch.StartNew();
            sw.Start();
            for (int i = 0; i < iterations; i++) {
                func.Invoke();
            }

            sw.Stop();

            var sw2      = Stopwatch.StartNew();
            var perFrame = Per.Frame(func);
            sw2.Start();
            for (int i = 0; i < iterations; i++) {
                var a = perFrame.Value;
            }

            sw2.Stop();

            var swAvg  = sw.Elapsed.TotalMilliseconds / iterations;
            var sw2Avg = sw2.Elapsed.TotalMilliseconds / iterations;
            LogUtils.Log($"sw: {sw.Elapsed}, avg: {swAvg}");
            LogUtils.Log($"sw2: {sw2.Elapsed}, avg: {sw2Avg}");
            LogUtils.Log($"cost: {sw2.Elapsed - sw.Elapsed}");
            LogUtils.Log($"ratio: {sw2Avg / swAvg}");
        }

        [Test]
        public void Efficiency2() {
            int laps = 1;

            Compare(
                "{ b = DateTime.Now }",
                () => {
                    var b = DateTime.Now;
                },
                laps
            );
            Compare("Random.Range", () => Random.Range(0, laps), laps);
            Compare(
                "{ a = Random.Range }",
                () => {
                    var a = Random.Range(0, laps);
                },
                laps
            );
            Compare("sleep 1ms", () => Thread.Sleep(1), laps);
        }

        public static void Compare(string nickname, Action action, int laps) {
            LogUtils.Log(nickname);
            var simpleDuration = TimeTrial.Attack(action, laps);

            var perFrame = Per.Frame(
                () => {
                    try {
                        action.Invoke();
                        return true;
                    }
                    catch {
                        return false;
                    }
                }
            );

            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            var perFrameDuration = TimeTrial.Attack(() => perFrame.Get(), laps);

            LogUtils.Log(
                new Dictionary<object, object>() {
                    { nameof(simpleDuration), $"{nameof(simpleDuration.Average)}: {simpleDuration.Average.TotalMilliseconds}, {nameof(simpleDuration.Shortest)}: {simpleDuration.Shortest.TotalMilliseconds}, {nameof(simpleDuration.Longest)}: {simpleDuration.Longest.TotalMilliseconds}" },
                    { nameof(perFrameDuration), $"{nameof(perFrameDuration.Average)}: {perFrameDuration.Average.TotalMilliseconds}, {nameof(perFrameDuration.Shortest)}: {perFrameDuration.Shortest.TotalMilliseconds}, {nameof(perFrameDuration.Longest)}: {perFrameDuration.Longest.TotalMilliseconds}" },
                    { "Ratio", perFrameDuration.Average.Divide(simpleDuration.Average) }
                }
            );
        }
    }
}