using UnityEngine.UI;

namespace Packages.BrandonUtils.Runtime.GameOptions {
    public class SliderOptionRenderer : GameOptionRenderer<SliderOption, int> {
        public Slider Slider;

        protected override void UpdateDisplay() {
            Slider.minValue     = GameOption.Min;
            Slider.maxValue     = GameOption.Max;
            Slider.wholeNumbers = true;
            Slider.value        = GameOption.Value;
        }
    }
}