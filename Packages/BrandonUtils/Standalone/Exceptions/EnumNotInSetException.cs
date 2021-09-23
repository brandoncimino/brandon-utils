﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using BrandonUtils.Standalone.Collections;
using BrandonUtils.Standalone.Strings;
using BrandonUtils.Standalone.Strings.Prettifiers;

using JetBrains.Annotations;

namespace BrandonUtils.Standalone.Exceptions {
    [Obsolete("Please use " + nameof(EnumNotInSetException<DayOfWeek>) + " instead")]
    public class EnumNotInSubsetException<T> : EnumNotInSetException<T> where T : Enum {
        public EnumNotInSubsetException(ICollection<T> superset, IEnumerable<T> expectedValues = null, string messagePrefix = null, Exception innerException = null) : base(superset, expectedValues, messagePrefix, innerException) { }
        public EnumNotInSubsetException(ICollection<T> superset, T              invalidValue,          string messagePrefix = null, Exception innerException = null) : base(superset, invalidValue, messagePrefix, innerException) { }
    }

    [PublicAPI]
    public class EnumNotInSetException<T> : InvalidEnumArgumentException where T : Enum {
        private         string _baseMessage;
        public override string Message       => List.Of(MessagePrefix, "", _baseMessage).NonNull().JoinLines();
        public virtual  string MessagePrefix { get; }

        /// <inheritdoc cref="InvalidEnumArgumentException"/>
        /// <summary>
        /// Constructs a new <see cref="EnumNotInSetException{T}"/>, listing information about the invalid <paramref name="expectedValues"/> and the <paramref name="superset"/>.
        /// </summary>
        /// <param name="expectedValues">A collection of <see cref="T"/> values, where <b>at least one</b> (but not necessarily all) is <b>not</b> in <paramref name="superset"/>.
        /// <br/>
        /// Only unique, invalid items from <paramref name="expectedValues"/> will be included in the logging message.
        /// </param>
        /// <param name="superset">The set of valid <see cref="T"/> values.</param>
        /// <param name="messagePrefix">A user-provided message, which will be <b>prepended</b> to the built-in message.</param>
        /// <param name="innerException">The <see cref="Exception"/> that caused this, if any.</param>
        public EnumNotInSetException(
            [NotNull] IEnumerable<T> superset,
            [CanBeNull]
            IEnumerable<T> expectedValues,
            string    messagePrefix  = null,
            Exception innerException = null
        ) : base(messagePrefix, innerException) {
            _baseMessage  = BuildMessage(superset, expectedValues);
            MessagePrefix = messagePrefix;
        }

        public EnumNotInSetException(
            [NotNull] IEnumerable<T> superset,
            T                        invalidValue,
            string                   messagePrefix  = null,
            Exception                innerException = null
        ) : this(superset, Enumerable.Repeat(invalidValue, 1), messagePrefix, innerException) { }

        private string BuildMessage([NotNull] IEnumerable<T> superset, [CanBeNull] IEnumerable<T> valuesThatShouldBeThere) {
            PrettificationSettings prettySettings = new PrettificationSettings() {
                Flags = PrettificationFlags.IncludeTypeLabels
            };

            var badValues = valuesThatShouldBeThere?.Except(superset);

            return new Dictionary<object, object>() {
                [superset.GetType().PrettifyType(prettySettings)] = this,
                [nameof(valuesThatShouldBeThere)]                 = valuesThatShouldBeThere,
                ["Disallowed values"]                             = badValues
            }.Prettify(prettySettings);
        }
    }
}