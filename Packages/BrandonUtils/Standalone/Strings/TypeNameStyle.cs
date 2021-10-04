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
            settings ??= new PrettificationSettings();

            if (type == null || settings.TypeLabelStyle == TypeNameStyle.None) {
                return "";
            }

            return $"[{type.PrettifyType(settings)}]";
        }

        [NotNull]
        public static string WithTypeLabel([CanBeNull] string thing, [CanBeNull] Type type, [CanBeNull] PrettificationSettings settings) {
            return $"{GetTypeLabel(type, settings)}{thing.OrNullPlaceholder(settings)}";
        }
    }
}