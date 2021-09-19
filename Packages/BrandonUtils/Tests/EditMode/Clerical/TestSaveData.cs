using BrandonUtils.Standalone.Clerical.Saving;

using Newtonsoft.Json;

namespace BrandonUtils.Tests.EditMode.Clerical {
    public class TestSaveData : SaveData {
        [JsonProperty]
        public string Property_Public_Get { get; }

        [JsonProperty]
        public string Field_Public;

        public TestSaveData(string nickname) {
            Property_Public_Get = $"{nickname}_{nameof(Property_Public_Get)}";
            Field_Public        = $"{nickname}_{nameof(Field_Public)}";
        }
    }
}