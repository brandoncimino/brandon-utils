using System;
using System.Collections.Generic;
using System.Linq;

using BrandonUtils.Standalone.Exceptions;

namespace BrandonUtils.Standalone.Randomization {
    public static class Brandom {
        public static readonly Random Gen = new Random();

        /// <returns>a random sign (either 1 or -1)</returns>
        public static int Sign() {
            return Gen.Next(2) == 0 ? -1 : 1;
        }

        /// <returns>a random <see cref="bool"/> (true or false)</returns>
        public static bool Bool() {
            return Gen.Next(2) == 0;
        }

        /// <summary>
        /// Returns a random value within <see cref="center"/> +- <see cref="radius"/>, <b>inclusive</b>.
        /// </summary>
        /// <param name="center">the center of the random range</param>
        /// <param name="radius">the size +- <see cref="center"/> to select a value from, <b>inclusively</b></param>
        /// <returns></returns>
        public static double Near(double center, double radius) {
            return center + Gen.NextDouble() * radius * Sign();
        }

        #region Weighted random

        /// <summary>
        /// Returns a <see cref="KeyValuePair{TKey,TValue}.Key"/> from the <paramref name="weightedChoices"/>,
        /// where each choice is weighted by its <see cref="KeyValuePair{TKey,TValue}.Value"/>.
        /// </summary>
        /// <param name="weightedChoices">a dictionary where the <see cref="IDictionary{TKey,TValue}.Keys"/> are the possible choices and the <see cref="IDictionary{TKey,TValue}.Values"/> are their weights.</param>
        /// <param name="generator"></param>
        /// <typeparam name="T">the outcome's <see cref="Type"/>.</typeparam>
        /// <returns>a random <see cref="KeyValuePair{TKey,TValue}.Key"/> from <see cref="weightedChoices"/>.</returns>
        /// <exception cref="BrandonException"></exception>
        public static T FromWeightedList<T>(IDictionary<T, double> weightedChoices, Random generator = default) {
            generator ??= Gen;
            var totalWeight  = weightedChoices.Values.Sum();
            var targetWeight = generator.NextDouble() * totalWeight;

            foreach (var choice in weightedChoices) {
                targetWeight -= choice.Value;
                if (targetWeight <= 0) {
                    return choice.Key;
                }
            }

            throw new BrandonException("Unable to find a random item, somehow...");
        }

        /// <inheritdoc cref="FromWeightedList{T}(System.Collections.Generic.IDictionary{T,double}, Random)"/>
        public static T FromWeightedList<T>(IDictionary<T, int> weightedChoices, Random generator = default) {
            var doubleDic = weightedChoices.ToDictionary(it => it.Key, it => (double)it.Value);
            return FromWeightedList(doubleDic, generator);
        }

        /// <inheritdoc cref="FromWeightedList{T}(System.Collections.Generic.IDictionary{T,double}, Random)"/>
        public static T FromWeightedList<T>(IDictionary<T, float> weightedChoices, Random generator = default) {
            var doubleDic = weightedChoices.ToDictionary(it => it.Key, it => (double)it.Value);
            return FromWeightedList(doubleDic, generator);
        }

        /// <inheritdoc cref="FromWeightedList{T}(System.Collections.Generic.IDictionary{T,double}, Random)"/>
        public static T FromWeightedList<T>(IEnumerable<T> choices, Func<T, double> weightSelector, Random generator = default) {
            var weightedChoices = choices.ToDictionary(
                it => it,
                weightSelector
            );

            return FromWeightedList(weightedChoices, generator);
        }

        #endregion
    }
}