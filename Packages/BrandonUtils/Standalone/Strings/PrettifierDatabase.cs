﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using BrandonUtils.Standalone.Collections;
using BrandonUtils.Standalone.Strings.Prettifiers;

namespace BrandonUtils.Standalone.Strings {
    internal static partial class PrettifierDatabase {
        internal static KeyedList<Type, IPrettifier> GetDefaultPrettifiers() {
            return new KeyedList<Type, IPrettifier>(it => it.PrettifierType) {
                new Prettifier<string>(Convert.ToString),
                new Prettifier<Type>((type, settings) => InnerPretty.PrettifyType(type, settings)),
                new Prettifier<IDictionary<object, object>>(InnerPretty.PrettifyDictionary),
                new Prettifier<IDictionary>(InnerPretty.PrettifyDictionary),
                new Prettifier<KeyedList<object, object>>(InnerPretty.PrettifyKeyedList),
                new Prettifier<(object, object)>(InnerPretty.Tuple2),
                new Prettifier<(object, object, object)>(InnerPretty.Tuple3),
                new Prettifier<(object, object, object, object)>(InnerPretty.Tuple4),
                new Prettifier<(object, object, object, object, object)>(InnerPretty.Tuple5),
                new Prettifier<(object, object, object, object, object, object)>(InnerPretty.Tuple6),
                new Prettifier<(object, object, object, object, object, object, object)>(InnerPretty.Tuple7),
                new Prettifier<(object, object, object, object, object, object, object, object)>(InnerPretty.Tuple8),
                new Prettifier<(object, object, object, object, object, object, object, object, object)>(InnerPretty.Tuple9),
                new Prettifier<(object, object, object, object, object, object, object, object, object, object)>(InnerPretty.Tuple10),
                new Prettifier<(object, object, object, object, object, object, object, object, object, object, object)>(InnerPretty.Tuple11),
                new Prettifier<(object, object, object, object, object, object, object, object, object, object, object, object)>(InnerPretty.Tuple12),
                new Prettifier<(object, object, object, object, object, object, object, object, object, object, object, object, object)>(InnerPretty.Tuple13),
                new Prettifier<(object, object, object, object, object, object, object, object, object, object, object, object, object, object)>(InnerPretty.Tuple14),
                new Prettifier<(object, object, object, object, object, object, object, object, object, object, object, object, object, object, object)>(InnerPretty.Tuple15),
                new Prettifier<(object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object)>(InnerPretty.Tuple16),
                new Prettifier<(object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object)>(InnerPretty.Tuple17),
                new Prettifier<IEnumerable<object>>(InnerPretty.PrettifyEnumerableT),
                new Prettifier<IEnumerable>(InnerPretty.PrettifyEnumerable),
                new Prettifier<MethodInfo>(InnerPretty.PrettifyMethodInfo),
                new Prettifier<ParameterInfo>(InnerPretty.PrettifyParameterInfo),
                new Prettifier<MemberInfo>(InnerPretty.PrettifyMemberInfo),
            };
        }
    }
}