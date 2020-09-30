using System;

using Newtonsoft.Json;

namespace Packages.BrandonUtils.Runtime.GameOptions {
    /// <summary>
    /// A <see cref="GameOption"/> that allows for a range of <b>contiguous integer values</b>.
    /// </summary>
    /// <remarks>
    /// Though Unity's UI Sliders store their values as floats by default, <see cref="SliderOption"/> is restricted to <see cref="int"/>s as they are much more convenient for sliders.
    /// </remarks>
    public class SliderOption : GameOption {
        [JsonProperty]
        public readonly int Min;
        [JsonProperty]
        public readonly int Max;

        [JsonProperty]
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