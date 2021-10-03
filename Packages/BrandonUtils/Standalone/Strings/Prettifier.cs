using System;

using BrandonUtils.Standalone.Collections;
using BrandonUtils.Standalone.Reflection;
using BrandonUtils.Standalone.Strings.Prettifiers;

using JetBrains.Annotations;

namespace BrandonUtils.Standalone.Strings {
    public class Prettifier<T> : IPrettifier<T>, IPrimaryKeyed<Type> {
        private Func<T, PrettificationSettings, string> PrettificationFunction { get; }

        public Type PrettifierType { get; } = typeof(T).IsGenericTypeOrDefinition() ? typeof(T).GetGenericTypeDefinition() : typeof(T);

        public Type PrimaryKey => PrettifierType;

        public Prettifier(Func<T, string> prettifierFunc) {
            PrettificationFunction = (it, settings) => prettifierFunc.Invoke(it);
        }

        public Prettifier(Func<T, PrettificationSettings, string> prettifierFunc) {
            PrettificationFunction = prettifierFunc;
        }

        public string Prettify([CanBeNull] T cinderella, PrettificationSettings settings = default) {
            return cinderella == null ? "" : PrettificationFunction.Invoke(cinderella, settings);
        }

        public string PrettifySafely(T cinderella, PrettificationSettings settings = default) {
            try {
                return Prettify(cinderella, settings);
            }
            catch {
                return Convert.ToString(cinderella);
            }
        }

        private T TrySlipper([NotNull] object cinderella) {
            if (cinderella is T princess) {
                return princess;
            }

            throw new InvalidCastException($"Couldn't prettify [{cinderella.GetType().PrettifyType()}]{cinderella} because it wasn't the right type, {PrettifierType.PrettifyType()}!");
        }

        public string Prettify(object cinderella, PrettificationSettings settings = default) {
            if (PrettifierType.IsGenericTypeOrDefinition()) {
                return PrettifyGeneric(cinderella, settings);
            }

            return cinderella == null ? "" : Prettify(TrySlipper(cinderella), settings);
        }

        private string PrettifyGeneric(object cinderella, PrettificationSettings settings = default) {
            if (!(cinderella.GetType().IsGenericType || cinderella.GetType().IsGenericTypeDefinition)) {
                throw new ArgumentException($"Can't use generic prettification for {cinderella.GetType()} because it isn't a generic type!");
            }

            var cinderellaTypes = cinderella.GetType().GetGenericArguments();
            var madeGeneric     = PrettificationFunction.Method.GetGenericMethodDefinition().MakeGenericMethod(cinderellaTypes);
            var result          = madeGeneric.Invoke(this, new object[] { cinderella, settings });
            return Convert.ToString(result);
        }

        public string PrettifySafely(object cinderella, PrettificationSettings settings = default) {
            try {
                return Prettify(cinderella, settings);
            }
            catch (Exception e) {
                var str = $"Error during prettification of [{cinderella}]:\n{e})";
                Console.WriteLine(str);
                return Convert.ToString(cinderella);
            }
        }
    }
}