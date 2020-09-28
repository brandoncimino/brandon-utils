using UnityEngine.UI;

namespace Packages.BrandonUtils.Runtime.GameOptions {
    public class ToggleOptionRenderer : GameOptionRenderer<ToggleOption> {
        public Toggle Toggle;

        protected override void UpdateDisplay() {
            // Toggle.value = GameOption.ValueAsBool;
        }
    }
}