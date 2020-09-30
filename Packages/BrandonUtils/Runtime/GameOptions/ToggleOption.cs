using System;

using Newtonsoft.Json;

namespace Packages.BrandonUtils.Runtime.GameOptions {
    /// <summary>
    /// A boolean <see cref="GameOption"/>.
    /// </summary>
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
            labelDisplayFunction ?? DisplayLabel_RenderFunction_ToggleOverride
        ) { }

        /// <summary>
        /// A method to be used as <see cref="GameOption.DisplayLabel_RenderFunction"/> if the user does not pass a new function to the <see cref="ToggleOption"/> constructor.
        /// </summary>
        /// <param name="gameOption"></param>
        /// <returns></returns>
        private static string DisplayLabel_RenderFunction_ToggleOverride(GameOption gameOption) => gameOption.DisplayName;
    }
}