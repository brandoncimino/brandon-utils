using JetBrains.Annotations;

using Newtonsoft.Json;

namespace BrandonUtils.Standalone {
    public interface IJsonCloneable { }

    public static class JsonCloneableExtensions {
        [CanBeNull]
        [ContractAnnotation("original:null => null")]
        [ContractAnnotation("original:notnull => notnull")]
        public static T JsonClone<T>([CanBeNull] this T original, [CanBeNull] JsonSerializerSettings settings = default) where T : IJsonCloneable {
            var json = JsonConvert.SerializeObject(original, settings!);
            return JsonConvert.DeserializeObject<T>(json, settings);
        }
    }
}