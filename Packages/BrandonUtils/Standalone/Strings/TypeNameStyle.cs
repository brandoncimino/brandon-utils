using System;

using BrandonUtils.Standalone.Reflection;
using BrandonUtils.Standalone.Strings.Json;
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
            settings = Prettification.ResolveSettings(settings);

            var style = type?.IsEnum == true ? settings.EnumLabelStyle : settings.TypeLabelStyle;

            settings?.TraceWriter.Verbose(() => $"Using style: {style} (type: {settings.TypeLabelStyle}/{settings.TypeLabelStyle.Value}, enum: {settings.EnumLabelStyle}/{settings.EnumLabelStyle.Value})");

            if (type == null || style == TypeNameStyle.None) {
                return "";
            }

            var str = type.PrettifyType(settings);

            if (type.IsArray || type.IsEnum || type.IsEnumerable()) {
                return str;
            }

            return $"[{str}]";
        }
    }
}