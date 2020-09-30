﻿using UnityEngine.UI;

namespace Packages.BrandonUtils.Runtime.GameOptions {
    public class ToggleOptionRenderer : GameOptionRenderer<ToggleOption> {
        public Toggle Toggle;

        protected override void UpdateDisplay() {
            Toggle.SetIsOnWithoutNotify(GameOption.ValueAs<bool>());
        }

        public void OnToggleValueChanged(bool value) {
            GameOption.Value = value;
        }
    }
}