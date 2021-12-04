using BrandonUtils.Standalone.Optional;
using BrandonUtils.Standalone.Strings;
using BrandonUtils.Testing;
using BrandonUtils.Tests.Standalone.Collections;

namespace BrandonUtils.Tests.Standalone {
    public static class Asserters {
        public static IMultipleAsserter CompareFallbacks<T>(Fallback<T> actual, Fallback<T> expected) {
            return Asserter.Against(actual)
                           .And(
                               Is.EqualTo(expected).Using(new FallbackComparer()),
                               typeof(FallbackComparer).Prettify
                           );
        }
    }
}