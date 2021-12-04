﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using BrandonUtils.Standalone.Exceptions;
using BrandonUtils.Standalone.Reflection;
using BrandonUtils.Standalone.Strings.Json;

using JetBrains.Annotations;

namespace BrandonUtils.Standalone.Strings {
    /// <summary>
    /// Converts some <see cref="Type"/>s into ones that are easier for <see cref="Prettification"/> to handle.
    /// </summary>
    internal static class PrettificationTypeSimplifier {
        internal class SimplifiedType {
            [NotNull]   public readonly  Type                   Original;
            [NotNull]   public           Type                   Simplified => _simplified.Value;
            [NotNull]   private readonly Lazy<Type>             _simplified;
            [CanBeNull] private readonly PrettificationSettings Settings;

            public SimplifiedType(Type original, [CanBeNull] PrettificationSettings settings = default) {
                Original    = original;
                _simplified = new Lazy<Type>(Simplify);
                Settings    = settings;
            }

            [NotNull]
            private Type Simplify() {
                return SimplifyType(Original, Settings);
            }

            [NotNull]
            public override string ToString() {
                return $"{Original.Name} 🤏 {Simplified.Name}";
            }
        }

        /// <summary>
        /// When we encounter one of these <see cref="IDictionary{TKey,TValue}.Keys"/>, we return the corresponding value and <b>stop simplifying</b>.
        /// </summary>
        private static readonly ReadOnlyDictionary<Type, Type> SimplestTypes = new ReadOnlyDictionary<Type, Type>(
            new Dictionary<Type, Type>() {
                [typeof(IReadOnlyDictionary<,>)] = typeof(IDictionary),
                [typeof(IDictionary<,>)]         = typeof(IDictionary),
                [typeof(Enum)]                   = typeof(Enum),
                [typeof(Type)]                   = typeof(Type),
                // ReSharper disable once PossibleMistakenCallToGetType.2
                [typeof(Type).GetType()] = typeof(Type),
                [typeof(IPrettifiable)]  = typeof(IPrettifiable),
                [typeof(object)]         = typeof(object),
            }
        );

        /// <summary>
        /// Converts <see cref="Type"/>s into ones that are easier for <see cref="Prettification"/> to handle.
        /// </summary>
        /// <param name="type">the original <see cref="Type"/></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        [NotNull]
        [Pure]
        internal static Type SimplifyType([NotNull] Type type, [CanBeNull] PrettificationSettings settings) {
            settings = Prettification.ResolveSettings(settings);
            return SimplifyTypeSwitch(type, settings);
        }

        private static Type SimplifyTypeSwitch([NotNull] Type type, [NotNull] PrettificationSettings settings, int recurCount = 0) {
            if (SimplestTypes.ContainsKey(type)) {
                var simplest = SimplestTypes[type];
                settings.TraceWriter.Verbose(() => $"🦠 Could not simplify {type.Name} past {simplest.Name}!", recurCount + 2);
            }

            var simplified = type switch {
                { IsEnum: true }                                => typeof(Enum),
                { IsPrimitive: true }                           => typeof(object),
                { } when type.Implements(typeof(IPrettifiable)) => typeof(IPrettifiable),
                { IsConstructedGenericType: true }              => type.GetGenericTypeDefinition(),
                _                                               => type,
            };

            if (simplified == type) {
                settings.TraceWriter.Verbose(() => $"🦠 {type.Name} simplified to the same type {simplified.Name}; returning fully simplified {simplified.Name}", recurCount + 2);
                return simplified;
            }
            else if (recurCount > 30) {
                throw new BrandonException($"REACHED RECUR LIMIT: {nameof(simplified)}: {simplified} != {nameof(type)}: {type}!");
            }
            else {
                settings.TraceWriter.Verbose(() => $"🤏 original {type.Name} simplified to {simplified.Name}; recurring...", recurCount + 2);
                return SimplifyTypeSwitch(simplified, settings, ++recurCount);
            }
        }
    }
}