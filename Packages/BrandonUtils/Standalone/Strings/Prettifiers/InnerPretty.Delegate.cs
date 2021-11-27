using System;
using System.Reflection;

using BrandonUtils.Standalone.Reflection;

using JetBrains.Annotations;

namespace BrandonUtils.Standalone.Strings.Prettifiers {
    internal partial class InnerPretty {
        private const string MethodIcon = "Ⓜ";
        private const string LambdaIcon = "λ";

        [NotNull]
        public static string PrettifyDelegate([NotNull] Delegate del, [NotNull] PrettificationSettings settings) {
            return del.IsCompilerGenerated() ? _Prettify_Lambda(del, settings) : _Prettify_MethodReference(del, settings);
        }

        [NotNull]
        private static string _Prettify_MethodReference([NotNull] Delegate methodReference, [NotNull] PrettificationSettings settings) {
            var methodInfo = PrettifyMethodInfo(methodReference.Method, settings);
            return $"{MethodIcon} {methodInfo}";
        }

        [NotNull]
        private static string _Prettify_Lambda([NotNull] Delegate del, [NotNull] PrettificationSettings settings) {
            var typeStr = del.GetType().Prettify(settings);
            var nameStr = del.GetMethodInfo().Name;
            return $"{LambdaIcon} {typeStr} => {nameStr}";
        }
    }
}