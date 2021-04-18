using UnityEngine.UI;

namespace BrandonUtils.GameOptions {
    public class SliderOptionRenderer : GameOptionRenderer<SliderOption> {
        public Slider Slider;
        public Text   MinLabel;
        public Text   MaxLabel;

        protected override void UpdateDisplay() {
            Slider.minValue     = GameOption.Min;
            Slider.maxValue     = GameOption.Max;
            Slider.wholeNumbers = true;
            Slider.value        = GameOption.ValueAs<int>();
            MinLabel.text       = GameOption.Min.ToString();
            MaxLabel.text       = GameOption.Max.ToString();
        }

        public void OnSliderValueChanged(float value) {
            GameOption.Value = (int) value;
        }
    }
}