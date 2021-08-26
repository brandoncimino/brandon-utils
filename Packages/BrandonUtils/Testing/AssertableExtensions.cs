using System;
using System.Collections.Generic;
using System.Linq;

using BrandonUtils.Standalone.Collections;
using BrandonUtils.Standalone.Exceptions;
using BrandonUtils.Standalone.Strings;

namespace BrandonUtils.Testing {
    internal static class AssertableExtensions {
        private const           int    HorizontalRuleLength = 50;
        private static readonly string HorizontalRule       = "-".Repeat(HorizontalRuleLength);

        internal static IEnumerable<string> FormatAssertable(this IAssertable failure) {
            if (!failure.Failed) {
                return new[] { $"{failure.Nickname}: Passed" };
            }

            return new[] {
                       $"{failure.Nickname} failed!",
                       $"{failure.Excuse}"
                   }
                   .Concat(FilterStackTrace(failure.Excuse))
                   .Bookend(HorizontalRule);
        }

        private static IEnumerable<string> FilterStackTrace(Exception exception) => exception.FilteredStackTrace(
                                                                                                 new StringFilter()
                                                                                                     .Matching(@"^\s*(in|at) NUnit")
                                                                                                     .Matching($@"^\s*(in|at) {typeof(AssertAll)}")
                                                                                             )
                                                                                             .TruncateLines(10);
    }
}