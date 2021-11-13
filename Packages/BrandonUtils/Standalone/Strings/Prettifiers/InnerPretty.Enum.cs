using System;

using JetBrains.Annotations;

namespace BrandonUtils.Standalone.Strings.Prettifiers {
    internal partial class InnerPretty {
        internal static string PrettifyEnum([NotNull] Enum enm, [CanBeNull] PrettificationSettings settings = default) {
            settings ??= Prettification.DefaultPrettificationSettings;
            return settings.TypeLabelStyle.Value switch {
                TypeNameStyle.None => enm.ToString(),
                _                  => $"{enm.GetType().Name}.{enm}"
            };
        }
    }
}