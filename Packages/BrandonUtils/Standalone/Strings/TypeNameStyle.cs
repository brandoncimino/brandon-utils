using System;

using BrandonUtils.Standalone.Reflection;
using BrandonUtils.Standalone.Strings.Prettifiers;

using JetBrains.Annotations;

namespace BrandonUtils.Standalone.Strings {
    public enum TypeNameStyle {
        None,
        Full,
        Short
    }

    public static class TypeNameStyleExtensions {
        [NotNull]
        public static string GetTypeLabel([CanBeNull] this Type type, [CanBeNull] PrettificationSettings settings) {
            settings ??= Prettification.DefaultPrettificationSettings;

            if (type == null || settings.TypeLabelStyle == TypeNameStyle.None) {
                return "";
            }

            var str = type.PrettifyType(settings);

            if (type.IsArray || type.IsEnumerable()) {
                return str;
            }

            return $"[{str}]";
        }

        [NotNull]
        public static string WithTypeLabel([CanBeNull] string thing, [CanBeNull] Type type, [CanBeNull] PrettificationSettings settings) {
            return $"{GetTypeLabel(type, settings)}{thing.OrNullPlaceholder(settings)}";
        }
    }
}