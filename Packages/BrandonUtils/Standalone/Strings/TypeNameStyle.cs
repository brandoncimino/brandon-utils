using System;

using BrandonUtils.Standalone.Strings.Prettifiers;

using JetBrains.Annotations;

namespace BrandonUtils.Standalone.Strings {
    public enum TypeNameStyle {
        None,
        Full,
        Short
    }

    public static class TypeLabelStyleExtensions {
        [NotNull]
        public static string GetTypeLabel([CanBeNull] this Type type, [CanBeNull] PrettificationSettings settings) {
            if (type == null || settings?.TypeLabelStyle == TypeNameStyle.None) {
                return "";
            }

            return $"[{type.PrettifyType(settings)}]";
        }

        public static string WithTypeLabel(string thing, Type type, [CanBeNull] PrettificationSettings settings) {
            return $"{GetTypeLabel(type, settings)}{thing}";
        }
    }
}