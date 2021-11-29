using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

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
                [typeof(IPrettifiable)]  = typeof(IPrettifiable)
            }
        );

        /// <summary>
        /// Converts <see cref="Type"/>s into ones that are easier for <see cref="Prettification"/> to handle.
        /// </summary>
        /// <param name="type">the original <see cref="Type"/></param>
        /// <param name="settings"></param>
        /// <param name="indent"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        [NotNull]
        [Pure]
        internal static Type SimplifyType([NotNull] Type type, [CanBeNull] PrettificationSettings settings, int indent = 0) {
            settings ??= Prettification.DefaultPrettificationSettings;

            settings.TraceWriter.Verbose(() => $"🤏 Simplifying Type: {type}", indent);
            indent++;

            Type simplified = type;

            // check for some exact, immediate types we've identified
            if (SimplestTypes.ContainsKey(type)) {
                simplified = SimplestTypes[type];
                settings.TraceWriter.Verbose(() => $"🔬 {type.Name} => {simplified.Name}", indent);
                return simplified;
            }

            // treat all Enums the same
            // TODO: If we wind up with an enum that we want special prettification for (which seems likely), then we will have to modify this logic.
            if (type.IsEnum && type != typeof(Enum)) {
                simplified = typeof(Enum);
                settings.TraceWriter.Verbose(() => $"🔬 {type.Name}.{nameof(type.IsEnum)} == {true} => {simplified.Name}", indent);
                return simplified;
            }

            if (type.GetInterface(nameof(IPrettifiable)) != null) {
                simplified = typeof(IPrettifiable);
                settings.TraceWriter.Verbose(() => $"🔬 {type.Name} implements {typeof(IPrettifiable)} => {simplified.Name}", indent);
                return simplified;
            }

            if (type.IsConstructedGenericType) {
                simplified = type.GetGenericTypeDefinition();
                settings.TraceWriter.Verbose(() => $"🔬 {type.Name}.{nameof(type.IsConstructedGenericType)} == {true} => {simplified.Name}", indent);
                return SimplifyType(simplified, settings, indent);
            }

            settings.TraceWriter.Verbose(() => $"🦠 Could not simplify {type.Name} past {simplified.Name}", indent);
            return type;
        }
    }
}