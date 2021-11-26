using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using BrandonUtils.Standalone.Collections;
using BrandonUtils.Standalone.Strings.Prettifiers;

using JetBrains.Annotations;

namespace BrandonUtils.Standalone.Strings {
    internal static partial class PrettifierDatabase {
        [NotNull]
        internal static KeyedList<Type, IPrettifier> GetDefaultPrettifiers() {
            return new KeyedList<Type, IPrettifier>(it => it.PrettifierType) {
                new Prettifier<string>(Convert.ToString),
                new Prettifier<Type>(InnerPretty.PrettifyType),
                new Prettifier<IDictionary<object, object>>(InnerPretty.PrettifyDictionary),
                new Prettifier<IDictionary>(InnerPretty.PrettifyDictionary),
                new Prettifier<KeyedList<object, object>>(InnerPretty.PrettifyKeyedList),
                new Prettifier<(object, object)>(InnerPretty.Tuple2),
                new Prettifier<(object, object, object)>(InnerPretty.Tuple3),
                new Prettifier<(object, object, object, object)>(InnerPretty.Tuple4),
                new Prettifier<(object, object, object, object, object)>(InnerPretty.Tuple5),
                new Prettifier<(object, object, object, object, object, object)>(InnerPretty.Tuple6),
                new Prettifier<(object, object, object, object, object, object, object)>(InnerPretty.Tuple7),
                new Prettifier<(object, object, object, object, object, object, object, object)>(InnerPretty.Tuple8Plus),
                new Prettifier<IEnumerable<object>>(InnerPretty.PrettifyEnumerableT),
                new Prettifier<IEnumerable>(InnerPretty.PrettifyEnumerable),
                new Prettifier<MethodInfo>(InnerPretty.PrettifyMethodInfo),
                new Prettifier<ParameterInfo>(InnerPretty.PrettifyParameterInfo),
                new Prettifier<MemberInfo>(InnerPretty.PrettifyMemberInfo),
                new Prettifier<Delegate>(InnerPretty.PrettifyDelegate),
            };
        }

        internal static readonly IPrettifier EnumPrettifier = new Prettifier<Enum>(InnerPretty.PrettifyEnum);
    }
}