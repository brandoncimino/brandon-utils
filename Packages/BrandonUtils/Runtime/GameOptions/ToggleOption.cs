namespace Packages.BrandonUtils.Runtime.GameOptions {
    public class ToggleOption : GameOption<bool> {
        public ToggleOption(string displayName, string description, bool initialValue) : base(
            displayName,
            description,
            initialValue
        ) { }
    }
}