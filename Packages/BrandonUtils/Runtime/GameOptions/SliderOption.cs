using System;

namespace Packages.BrandonUtils.Runtime.GameOptions {
    public class SliderOption : GameOption {
        public readonly int Min;
        public readonly int Max;

        public override Type ValueType { get; } = typeof(int);

        public SliderOption(
            string displayName,
            string description,
            int min,
            int max,
            int initialValue,
            Func<GameOption, string> valueDisplayFunction = null,
            Func<GameOption, string> labelDisplayFunction = null
        ) : base(
            displayName,
            description,
            initialValue,
            valueDisplayFunction,
            labelDisplayFunction
        ) {
            Min = min;
            Max = max;
        }
    }
}