﻿using System;
using System.Collections.Generic;
using System.Linq;

using BrandonUtils.Standalone.Collections;
using BrandonUtils.Standalone.Reflection;

using JetBrains.Annotations;

namespace BrandonUtils.Standalone.Optional {
    /**
     * <inheritdoc cref="IFailable"/>
     */
    public class Failable : IFailable {
        private const    string                    SuccessIcon = "✅";
        private const    string                    FailIcon    = "❌";
        private readonly Exception                 _excuse;
        public           Exception                 Excuse                => _excuse ?? throw FailableException.DidNotFailException(this);
        public           bool                      Failed                => _excuse != null;
        public           IReadOnlyCollection<Type> IgnoredExceptionTypes { get; }
        public           Optional<Exception>       IgnoredException      { get; }

        public Failable([NotNull, InstantHandle] Action failableAction, [NotNull, ItemNotNull] params Type[] ignoredExceptionTypes) : this(failableAction, ignoredExceptionTypes.AsEnumerable()) { }

        public Failable([NotNull, InstantHandle] Action failableAction, [NotNull, ItemNotNull] IEnumerable<Type> ignoredExceptionTypes) {
            IgnoredExceptionTypes = ignoredExceptionTypes.Must(ReflectionUtils.IsExceptionType).ToArray();

            if (failableAction == null) {
                throw new ArgumentNullException(nameof(failableAction), $"Unable to attempt a {nameof(Failable)} because the {nameof(failableAction)} was null!");
            }

            try {
                failableAction.Invoke();
                Console.WriteLine("Failable -> ✅");
                _excuse          = default;
                IgnoredException = default;
            }
            catch (Exception e) when (e.IsInstanceOf(IgnoredExceptionTypes)) {
                // Handling an ignored exception
                Console.WriteLine("Failable -> 🔇");
                _excuse          = default;
                IgnoredException = e;
            }
            catch (Exception e) {
                // Handling a non-ignored exception
                Console.WriteLine("Failable -> 🧨");
                _excuse          = e;
                IgnoredException = default;
            }
        }

        [NotNull]
        public override string ToString() {
            return $"{(Failed ? $"{FailIcon} [{Excuse}]" : SuccessIcon)}";
        }
    }
}