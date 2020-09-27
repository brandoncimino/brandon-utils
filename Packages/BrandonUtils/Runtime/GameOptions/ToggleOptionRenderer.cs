using UnityEngine.UIElements;

namespace Packages.BrandonUtils.Runtime.GameOptions {
    public class ToggleOptionRenderer : GameOptionRenderer<ToggleOption, bool> {
        public Toggle Toggle;

        protected override void UpdateDisplay() {
            Toggle.value = GameOption.Value;
        }
    }
}