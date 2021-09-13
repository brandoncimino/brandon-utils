using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using BrandonUtils.Standalone.Collections;

using JetBrains.Annotations;

namespace BrandonUtils.Standalone.Exceptions {
    [Obsolete("Please use " + nameof(EnumNotInSetException<DayOfWeek>) + " instead")]
    public class EnumNotInSubsetException<T> : EnumNotInSetException<T> where T : Enum {
        public EnumNotInSubsetException(ICollection<T> subset, IEnumerable<T> values = null, string message = null, Exception innerException = null) : base(subset, values, message, innerException) { }
        public EnumNotInSubsetException(ICollection<T> subset, T              invalidValue,  string message = null, Exception innerException = null) : base(subset, invalidValue, message, innerException) { }
    }

    [PublicAPI]
    public class EnumNotInSetException<T> : InvalidEnumArgumentException where T : Enum {
        public override string Message { get; }

        /// <inheritdoc cref="InvalidEnumArgumentException"/>
        /// <summary>
        /// Constructs a new <see cref="EnumNotInSetException{T}"/>, listing information about the invalid <paramref name="values"/> and the <paramref name="subset"/>.
        /// </summary>
        /// <param name="values">A collection of <see cref="T"/> values, where <b>at least one</b> (but not necessarily all) is <b>not</b> in <paramref name="subset"/>.
        /// <br/>
        /// Only unique, invalid items from <paramref name="values"/> will be included in the logging message.
        /// </param>
        /// <param name="subset">The set of valid <see cref="T"/> values.</param>
        /// <param name="message">A user-provided message, which will be <b>prepended</b> to the built-in message.</param>
        /// <param name="innerException">The <see cref="Exception"/> that caused this, if any.</param>
        public EnumNotInSetException(
            ICollection<T> subset,
            IEnumerable<T> values         = null,
            string         message        = null,
            Exception      innerException = null
        ) : base(message, innerException) {
            var badValues = (values ?? Enumerable.Empty<T>()).Where(value => !subset.Contains(value)).ToArray();

            var badValueString = badValues.Length == 0 ? null : $"Invalid values: [{badValues.JoinString(", ")}]";

            var subsetString = $"Allowed values: [{subset.JoinString(", ")}]";

            var lines = new[] {
                message,
                badValueString,
                subsetString
            };

            Message = lines.Where(ln => ln != null).JoinString("\n");
        }

        public EnumNotInSetException(
            ICollection<T> subset,
            T              invalidValue,
            string         message        = null,
            Exception      innerException = null
        ) : this(subset, Enumerable.Repeat(invalidValue, 1), message, innerException) { }
    }
}