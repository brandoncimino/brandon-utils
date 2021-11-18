using System;
using System.Collections;
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
            var enumerable = (IEnumerable)value;
            enumerable = enumerable.Cast<object>().ToArray();
            serializer.Serialize(writer, enumerable);
        }

        [NotNull]
        public override object ReadJson([NotNull] JsonReader reader, [NotNull] Type objectType, [CanBeNull] object existingValue, [NotNull] JsonSerializer serializer) {
            var elementType   = objectType.GetGenericArguments()[0];
            var arType        = elementType.MakeArrayType();
            var asIList       = serializer.Deserialize(reader, arType) as IList ?? throw new NullReferenceException($"Got a null value when deserializing {objectType.Prettify()} to the intermediate type {arType.Prettify()} as {nameof(IList)}");
            var singleElement = asIList[0];
            return Activator.CreateInstance(objectType, singleElement);
        }

        public override bool CanConvert(Type objectType) {
            return objectType.IsGenericTypeOrDefinition()
                   && typeof(Optional<>).IsAssignableFrom(objectType.GetGenericTypeDefinition());
        }
    }
}