using System.Linq;

using Packages.BrandonUtils.Runtime.Collections;
using Packages.BrandonUtils.Runtime.GameOptions;

namespace DefaultNamespace {
    public class DebugGameMode : IGameMode {
        public const string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        public readonly SliderOption Slider_1 = new SliderOption(
            "Slider Option #1",
            "First of my name!",
            5,
            10,
            6
        );

        public readonly SliderOption Slider_2 = new SliderOption(
            "Number. 2",
            "Slightly less evil",
            -3,
            152,
            -1
        );

        public readonly ToggleOption Toggle_1 = new ToggleOption(
            "YOLO?",
            "Swag",
            true
        );

        public readonly ToggleOption Toggle_2 = new ToggleOption(
            "Do you like me?",
            "Y/N",
            false
        );

        public readonly SliderOption LetterPool = new SliderOption(
            "Letter Pool",
            "This is the letter pool",
            5,
            Alphabet.Length,
            8,
            option => Alphabet.Substring(0, option.ValueAsInt)
        );

        public GameOption[] GameOptions => new GameOption[] {
            Slider_1,
            Slider_2,
            Toggle_1,
            Toggle_2,
            LetterPool,
        };

        public override string ToString() {
            return GameOptions.Select(option => option.DisplayLabel).JoinString("\n");
        }
    }
}