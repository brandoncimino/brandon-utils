﻿using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;

using BrandonUtils.Standalone.Reflection;
using BrandonUtils.Standalone.Strings;

using JetBrains.Annotations;

using Newtonsoft.Json;

namespace BrandonUtils.Standalone.Optional {
    /// <summary>
    /// A custom <see cref="JsonConverter"/> for <see cref="Optional{T}"/>.
    /// </summary>
    public class OptionalJsonConverter : JsonConverter {
        public override void WriteJson([NotNull] JsonWriter writer, object value, [NotNull] JsonSerializer serializer) {
            serializer.TraceWriter?.Trace(TraceLevel.Info, $"[{GetType().Prettify()}] serializing {value}", default);
            var enumerable = (IEnumerable)value;
            /*
             * We create a new JsonSerializer here so that it doesn't get upset about self-referencing loops when we try to serialize
             * this optional as an IEnumerable.
             *
             * Somehow, even though the new serializer doesn't contain the settings from the old one,
             * the writer contains the original's settings as well, and those seem to be used...
             *
             * The call to .Cast<object>().ToList() is necessary because that's what _actually_ prevents this from being an infinite loop,
             * because it returns a NEW list (for some reason, just .Cast<object>() wasn't enough)
             */
            JsonSerializer.Create().Serialize(writer, enumerable.Cast<object>().ToList());
        }

        [NotNull]
        public override object ReadJson([NotNull] JsonReader reader, [NotNull] Type objectType, [CanBeNull] object existingValue, [NotNull] JsonSerializer serializer) {
            var elementType = objectType.GetGenericArguments()[0];
            var arType      = elementType.MakeArrayType();
            var asIList     = serializer.Deserialize(reader, arType) as IList ?? throw new NullReferenceException($"Got a null value when deserializing {objectType.Prettify()} to the intermediate type {arType.Prettify()} as {nameof(IList)}");
            return asIList.Count == 0 ? Activator.CreateInstance(objectType) : Activator.CreateInstance(objectType, asIList[0]);
        }

        public override bool CanConvert(Type objectType) {
            return objectType.IsGenericTypeOrDefinition()
                   && typeof(Optional<>).IsAssignableFrom(objectType.GetGenericTypeDefinition());
        }
    }
}