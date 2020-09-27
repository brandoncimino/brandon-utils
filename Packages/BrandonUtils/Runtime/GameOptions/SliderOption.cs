namespace Packages.BrandonUtils.Runtime.GameOptions {
    public class SliderOption : GameOption<int> {
        public readonly int Min;
        public readonly int Max;

        public SliderOption(string displayName, string description, int min, int max, int initialValue) : base(
            displayName,
            description,
            initialValue
        ) {
            Min = min;
            Max = max;
        }
    }
}