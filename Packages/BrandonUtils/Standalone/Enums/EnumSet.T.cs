using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

using BrandonUtils.Standalone.Exceptions;
using BrandonUtils.Standalone.Strings;
using BrandonUtils.Standalone.Strings.Prettifiers;

using JetBrains.Annotations;

namespace BrandonUtils.Standalone.Enums {
    /// <summary>
    /// A special type of <see cref="HashSet{T}"/> specifically meant for use with <see cref="Enum"/>s.
    /// </summary>
    /// <typeparam name="T">an <see cref="Enum"/> type</typeparam>
    [PublicAPI]
    public class EnumSet<T> : HashSet<T> where T : struct, Enum {
        #region Constructors

        public EnumSet([NotNull] params T[] enumValues) : base(enumValues) { }

        public EnumSet([NotNull] params IEnumerable<T>[] setsToBeUnionized) : base(setsToBeUnionized.SelectMany(it => it)) { }

        #region Inherited Constructors

        public EnumSet() { }
        public EnumSet([NotNull] IEnumerable<T> collection) : base(collection) { }
        protected EnumSet(SerializationInfo     info, StreamingContext context) : base(info, context) { }

        #endregion Inherited Constructors

        #endregion Constructors

        #region Factory Methods

        /// <returns>an <see cref="EnumSet{T}"/> containing all of the <b>unique</b> values of <typeparamref name="T"/></returns>
        /// <seealso cref="EnumSet.OfAllValues{T}"/>
        public static EnumSet<T> OfAllValues() {
            return new EnumSet<T>(BEnum.GetValues<T>());
        }

        #endregion

        #region MustContain

        public void MustContain(params T[] valuesThatShouldBeThere) {
            if (!IsSupersetOf(valuesThatShouldBeThere)) {
                var badValues = valuesThatShouldBeThere.Except(this);
                var prettySettings = new PrettificationSettings() {
                    Flags = PrettificationFlags.IncludeTypeLabels
                };
                var mapStuff = new Dictionary<object, object>() {
                    [GetType().PrettifyType(prettySettings)] = this,
                    [nameof(valuesThatShouldBeThere)]        = valuesThatShouldBeThere,
                    ["Disallowed values"]                    = badValues
                };
                throw new EnumNotInSetException<T>(this, valuesThatShouldBeThere, $"The {GetType().PrettifyType()} didn't contain all of the {nameof(valuesThatShouldBeThere)}!\n{mapStuff.Prettify(prettySettings)}");
            }
        }

        #endregion

        public EnumSet<T> Copy() {
            return new EnumSet<T>(this);
        }
    }
}