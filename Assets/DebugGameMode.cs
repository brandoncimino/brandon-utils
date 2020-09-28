using Packages.BrandonUtils.Runtime.GameOptions;

namespace DefaultNamespace {
    public class DebugGameMode : IGameMode {
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

        public GameOption[] GameOptions => new GameOption[] {
            Slider_1,
            Slider_2,
            Toggle_1,
            Toggle_2
        };
    }
}