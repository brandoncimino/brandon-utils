using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using BrandonUtils.Standalone.Collections;
using BrandonUtils.Standalone.Strings.Prettifiers;

using JetBrains.Annotations;

namespace BrandonUtils.Standalone.Strings {
    public interface IPrettifierDatabase {
        [CanBeNull] IPrettifier Find([NotNull] Type type);

        void                    Register([NotNull]   IPrettifier prettifier);
        [CanBeNull] IPrettifier Deregister([NotNull] Type        type);

        [CanBeNull] IPrettifier Find([NotNull] Func<IPrettifier, bool> predicate);
    }

    public class PrettifierDatabase : IPrettifierDatabase {
        /// <summary>
        /// All of the <see cref="IPrettifier"/>s in this <see cref="PrettifierDatabase"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="IDictionary{TKey,TValue}"/> is used to make sure that retrieval via indexer is efficient.
        /// </remarks>
        private readonly IDictionary<Type, IPrettifier> Prettifiers;

        public PrettifierDatabase(IEnumerable<IPrettifier> prettifiers) {
            Prettifiers = new PrimaryKeyedList<Type, IPrettifier>(prettifiers).ToDictionary();
        }

        public PrettifierDatabase(params IPrettifier[] prettifiers) : this(prettifiers.AsEnumerable()) { }

        public void Register(IPrettifier prettifier) {
            if (prettifier == null) {
                throw new ArgumentNullException(nameof(prettifier));
            }

            Prettifiers.Add(prettifier.PrimaryKey, prettifier);
        }

        public IPrettifier Deregister(Type type) {
            if (type == null) {
                throw new ArgumentNullException(nameof(type));
            }

            return Prettifiers.Grab(type);
        }

        public IPrettifier Find(Type type) {
            Prettifiers.TryGetValue(type, out IPrettifier result);
            return result;
        }

        public IPrettifier Find(Func<IPrettifier, bool> predicate) {
            return Prettifiers.Where(it => predicate(it.Value))
                              .Select(it => it.Value)
                              .FirstOrDefault();
        }

        [NotNull]
        public static PrettifierDatabase GetDefaultPrettifiers() {
            return new PrettifierDatabase(
                new Prettifier<string>(Convert.ToString),
                new Prettifier<IPrettifiable>(PrettifyPrettifiable),
                new Prettifier<Enum>(InnerPretty.PrettifyEnum),
                new Prettifier<Type>(InnerPretty.PrettifyType),
                new Prettifier<IDictionary>(InnerPretty.PrettifyDictionary2),
                new Prettifier<KeyedList<object, object>>(InnerPretty.PrettifyKeyedList),
                new Prettifier<(object, object)>(InnerPretty.Tuple2),
                new Prettifier<(object, object, object)>(InnerPretty.Tuple3),
                new Prettifier<(object, object, object, object)>(InnerPretty.Tuple4),
                new Prettifier<(object, object, object, object, object)>(InnerPretty.Tuple5),
                new Prettifier<(object, object, object, object, object, object)>(InnerPretty.Tuple6),
                new Prettifier<(object, object, object, object, object, object, object)>(InnerPretty.Tuple7),
                new Prettifier<(object, object, object, object, object, object, object, object)>(InnerPretty.Tuple8Plus),
                new Prettifier<IEnumerable>(InnerPretty.PrettifyEnumerable),
                new Prettifier<MethodInfo>(InnerPretty.PrettifyMethodInfo),
                new Prettifier<ParameterInfo>(InnerPretty.PrettifyParameterInfo),
                new Prettifier<MemberInfo>(InnerPretty.PrettifyMemberInfo),
                new Prettifier<Delegate>(InnerPretty.PrettifyDelegate)
            );
        }

        internal static string PrettifyPrettifiable([NotNull] IPrettifiable prettifiable, [NotNull] PrettificationSettings settings) => prettifiable.Prettify(settings);
        internal static string PrettifyToString([NotNull]     object        obj,          [NotNull] PrettificationSettings settings) => obj.ToString();

        #region Special High-Priority Prettifiers

        internal static readonly IPrettifier PrettifiablePrettifier = new Prettifier<IPrettifiable>(PrettifyPrettifiable);
        internal static readonly IPrettifier ToStringPrettifier     = new Prettifier<object>(PrettifyToString);

        #endregion
    }
}