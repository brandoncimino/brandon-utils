using System;

namespace Packages.BrandonUtils.Runtime.GameOptions {
    public class ToggleOption : GameOption {
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

        protected override string RenderDisplayValue_Default(GameOption gameOption) => "";
    }
}