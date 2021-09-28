using System;
using System.Collections.Generic;
using System.Linq;

using BrandonUtils.Standalone.Exceptions;
using BrandonUtils.Standalone.Strings;

using FowlFever.Conjugal.Affixing;

namespace BrandonUtils.Testing {
    internal static class AssertableExtensions {
        private const string PassIcon             = "✅";
        private const string FailIcon             = "❌";
        private const string ExcuseIcon           = "";
        private const int    HorizontalRuleLength = 50;
        internal static readonly PrettificationSettings AssertablePrettificationSettings = new PrettificationSettings() {
            TypeLabelStyle = { Value = TypeNameStyle.Short }
        };
        private static readonly string HorizontalRule = "".FillRight(AssertablePrettificationSettings.LineLengthLimit.Value, "-");

        internal static IEnumerable<string> FormatAssertable(this IAssertable failure, int indent = 0) {
            return FormatAssertable(failure, PassIcon, FailIcon, ExcuseIcon, indent);
        }

        private static IEnumerable<string> FormatAssertable(this IAssertable failure, string passIcon, string failIcon, string excuseIcon, int indent) {
            var header = GetHeader(failure, passIcon, failIcon);
            var excuse = FormatExcuse(failure, excuseIcon);

            var excuseIndentSize = header.BoundMorpheme.Length  + header.Joiner.Length;
            var excusePrefix     = " ".Repeat(excuseIndentSize) + "| ";

            return header
                   .Render()
                   .SplitLines()
                   .Concat(excuse.Prefix(excusePrefix))
                   .Indent(indent);
        }

        private static IEnumerable<string> FormatExcuseMessage(IAssertable failure, string excuseIcon) {
            return failure.Excuse.Message.SplitLines();
            // return failure.Excuse.Message.SplitLines().Prefix("|", " ").IndentWithLabel(excuseIcon);
        }

        private static IEnumerable<string> FormatExcuse(IAssertable failure, string excuseIcon) {
            if (failure.Failed) {
                var message    = FormatExcuseMessage(failure, excuseIcon);
                var stacktrace = failure.Excuse.StackTrace.SplitLines();
                return message.Concat(stacktrace);
            }
            else {
                return Array.Empty<string>();
            }
        }

        private static Affixation GetHeader(IAssertable assertable, string passIcon = PassIcon, string failIcon = FailIcon) {
            var icon = assertable.Failed ? failIcon : passIcon;
            return Affixation.Prefixation(assertable.Nickname, icon, " ");
        }

        private static IEnumerable<string> FormatHeader(IAssertable failure, string passIcon = PassIcon, string failIcon = FailIcon) {
            var icon   = failure.Failed ? failIcon : passIcon;
            var result = failure.Failed ? "failed!" : "passed!";
            var name   = failure.Nickname;
            return new[] {
                $"{icon} {name} {result}"
            };
        }

        private static IEnumerable<string> FilterStackTrace(Exception exception) => exception.FilteredStackTrace(
                                                                                                 new StringFilter()
                                                                                                     .Matching(@"^\s*(in|at) NUnit")
                                                                                                     .Matching($@"^\s*(in|at) {typeof(AssertAll)}")
                                                                                                     .Matching(@"^\s*(in|at) System\.Reflection")
                                                                                                     .Matching(@"^\s*(in|at) System\.RuntimeMethodHandle")
                                                                                             )
                                                                                             .TruncateLines(10);
    }
}