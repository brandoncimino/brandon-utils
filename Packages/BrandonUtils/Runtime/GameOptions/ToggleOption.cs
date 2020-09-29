using System;

using Newtonsoft.Json;

namespace Packages.BrandonUtils.Runtime.GameOptions {
    public class ToggleOption : GameOption {
        [JsonProperty]
        public override Type ValueType { get; } = typeof(bool);

        public ToggleOption(
            string displayName,
            string description,
            bool initialValue,
            Func<GameOption, string> valueDisplayFunction = null,
            Func<GameOption, string> labelDisplayFunction = null
        ) : base(
            displayName,
            description,
            initialValue,
            valueDisplayFunction,
            labelDisplayFunction
        ) { }

        protected override string RenderDisplayLabel_Default(GameOption gameOption) {
            return DisplayName;
        }
    }
}