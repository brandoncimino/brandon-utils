using System;

using NUnit.Framework.Constraints;

namespace BrandonUtils.Testing {
    public static class IgnoreAll {
        public static void Of(string heading, params Action[] ignoreActions) {
            Ignorer.WithHeading(heading)
                   .And(ignoreActions)
                   .Execute();
        }

        public static void Of(params Action[] assertions) {
            Of(null, assertions);
        }

        public static void Of<T>(string heading, T actual, params IResolveConstraint[] constraints) {
            Ignorer.Against(actual)
                   .WithHeading(heading)
                   .And(constraints)
                   .Execute();
        }

        public static void Of<T>(T actual, params IResolveConstraint[] constraints) {
            Ignorer.Against(actual)
                   .And(constraints)
                   .Execute();
        }
    }
}