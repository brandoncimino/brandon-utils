using System;

namespace Packages.BrandonUtils.Runtime.GameOptions {
    public abstract class GameOption<T> {
        public T InitialValue;
        public T Value;

        public string Description;
        public string DisplayName  { get; }
        public string DisplayValue => ValueDisplayFunction(this);
        public string DisplayLabel => LabelDisplayFunction(this);

        public readonly Func<GameOption<T>, string> LabelDisplayFunction = RenderDisplayLabel_Default;
        public readonly Func<GameOption<T>, string> ValueDisplayFunction = RenderDisplayValue_Default;

        protected GameOption(string displayName, string description, T initialValue) {
            DisplayName  = displayName;
            InitialValue = initialValue;
            Value        = initialValue;
            Description  = description;
        }

        private static string RenderDisplayLabel_Default(GameOption<T> gameOption) {
            return $"{gameOption.DisplayName}: {gameOption.DisplayValue}";
        }

        private static string RenderDisplayValue_Default(GameOption<T> gameOption) {
            return gameOption.Value.ToString();
        }
    }
}