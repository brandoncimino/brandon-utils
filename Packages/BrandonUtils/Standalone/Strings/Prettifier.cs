using System;

using BrandonUtils.Standalone.Collections;
using BrandonUtils.Standalone.Reflection;
using BrandonUtils.Standalone.Strings.Prettifiers;

using JetBrains.Annotations;

namespace BrandonUtils.Standalone.Strings {
    public class Prettifier<T> : IPrettifier<T>, IPrimaryKeyed<Type> {
        [NotNull] private Func<T, PrettificationSettings, string> PrettificationFunction { get; }

        public Type PrettifierType { get; } = _getPrettifierType();

        [NotNull] public Type PrimaryKey => PrettifierType;

        #region Constructors

        public Prettifier([NotNull] Func<T, string> prettifierFunc) : this((it, settings) => prettifierFunc.Invoke(it)) { }

        public Prettifier([NotNull] Func<T, PrettificationSettings, string> prettifierFunc) {
            PrettificationFunction = prettifierFunc;
        }

        private static Type _getPrettifierType() {
            if (typeof(T) == typeof(Type)) {
                // ReSharper disable once PossibleMistakenCallToGetType.2
                return typeof(T).GetType();
            }

            return typeof(T).IsGenericTypeOrDefinition() ? typeof(T).GetGenericTypeDefinition() : typeof(T);
        }

        #endregion

        #region Prettifier Implementation

        public string Prettify(T cinderella, PrettificationSettings settings = default) {
            return PrettificationFunction.Invoke(cinderella, settings);
        }

        public string PrettifySafely(T cinderella, PrettificationSettings settings = default) {
            return PrettifySafely((object)cinderella, settings);
        }

        public string Prettify(object cinderella, PrettificationSettings settings = default) {
            settings ??= Prettification.DefaultPrettificationSettings;

            if (settings.VerboseLogging) {
                Console.WriteLine($"⚠ DANGEROUSLY prettifying [{cinderella?.GetType()}]{cinderella}");
            }

            if (cinderella == null) {
                return settings.NullPlaceholder;
            }
            else if (PrettifierType.IsGenericTypeOrDefinition()) {
                return PrettifyGeneric(cinderella, settings);
            }
            else {
                return Prettify(TrySlipper(cinderella), settings);
            }
        }

        public string PrettifySafely(object cinderella, PrettificationSettings settings = default) {
            settings ??= Prettification.DefaultPrettificationSettings;

            if (settings.VerboseLogging) {
                Console.WriteLine($"🦺 SAFELY prettifying [{cinderella?.GetType().Name}]{cinderella}");
            }

            try {
                return Prettify(cinderella, settings);
            }
            catch (Exception e) {
                var str = $"🧨 Error during prettification of [{cinderella?.GetType().Name}]{cinderella}:\n{e})";
                Console.WriteLine(str);
                return Prettification.LastResortPrettifier(cinderella, settings);
            }
        }

        #endregion

        #region Helpers

        [NotNull]
        private T TrySlipper([NotNull] object cinderella) {
            if (cinderella is T princess) {
                return princess;
            }

            throw new InvalidCastException($"Couldn't prettify [{cinderella.GetType().PrettifyType()}]{cinderella} because it wasn't the right type, {PrettifierType.PrettifyType()}!");
        }

        [NotNull]
        private string PrettifyGeneric([CanBeNull] object cinderella, [CanBeNull] PrettificationSettings settings) {
            settings ??= Prettification.DefaultPrettificationSettings;

            if (settings.VerboseLogging) {
                Console.WriteLine($"🕵️ Using generic prettification for [{cinderella?.GetType()}");
            }

            if (cinderella?.GetType().IsGenericTypeOrDefinition() != true) {
                throw new ArgumentException($"Can't use generic prettification for type [{cinderella?.GetType()}] because it isn't a generic type!");
            }

            var cinderellaTypes = cinderella.GetType().GetGenericArguments();
            var madeGeneric     = PrettificationFunction.Method.GetGenericMethodDefinition().MakeGenericMethod(cinderellaTypes);
            var result          = madeGeneric.Invoke(this, new object[] { cinderella, settings });
            return Convert.ToString(result);
        }

        #endregion

        [NotNull]
        public override string ToString() {
            return $"{GetType().Name} for [{PrettifierType.Name}] via [{PrettificationFunction.Method.Name}]";
        }
    }
}