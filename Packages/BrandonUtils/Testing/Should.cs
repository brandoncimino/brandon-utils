using System.Diagnostics.CodeAnalysis;

using BrandonUtils.Standalone.Enums;

using NUnit.Framework.Constraints;

namespace BrandonUtils.Testing {
    public enum Should {
        Pass,
        Fail
    }

    [SuppressMessage("ReSharper", "AccessToStaticMemberViaDerivedType")]
    public static class ShouldExtensions {
        public static bool Boolean(this Should should) {
            return should switch {
                Should.Pass => true,
                Should.Fail => false,
                _           => throw BEnum.InvalidEnumArgumentException(nameof(should), should)
            };
        }

        public static Constraint Constrain(this Should should) {
            return should switch {
                Should.Pass => Is.True,
                Should.Fail => Is.False,
                _           => throw BEnum.InvalidEnumArgumentException(nameof(should), should)
            };
        }
    }
}