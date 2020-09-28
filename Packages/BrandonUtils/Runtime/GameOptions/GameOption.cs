using System;
using System.Linq;

namespace Packages.BrandonUtils.Runtime.GameOptions {
    public abstract class GameOption {
        public object InitialValue { get; }
        public object Value        { get; set; }

        public abstract Type ValueType { get; }

        public string Description  { get; }
        public string DisplayName  { get; }
        public string DisplayValue => ValueDisplayFunction(this);
        public string DisplayLabel => LabelDisplayFunction(this);

        public readonly Func<GameOption, string> LabelDisplayFunction;
        public readonly Func<GameOption, string> ValueDisplayFunction;

        private const string Separators = "?:";

        private const char Separator_Default = ':';

        protected GameOption(
            string displayName,
            string description,
            object initialValue,
            Func<GameOption, string> valueDisplayFunction,
            Func<GameOption, string> labelDisplayFunction
        ) {
            DisplayName          = displayName;
            InitialValue         = initialValue;
            Value                = initialValue;
            Description          = description;
            ValueDisplayFunction = valueDisplayFunction ?? RenderDisplayValue_Default;
            LabelDisplayFunction = labelDisplayFunction ?? RenderDisplayLabel_Default;
        }

        protected virtual string RenderDisplayLabel_Default(GameOption gameOption) {
            var separator = Separators.Contains(gameOption.DisplayName.Last()) ? (char?) null : Separator_Default;
            return $"{gameOption.DisplayName}{separator} {gameOption.DisplayValue}";
        }

        protected virtual string RenderDisplayValue_Default(GameOption gameOption) {
            return gameOption.Value.ToString();
        }

        #region Specific Type Returns

        public string ValueAsString => ValueType == typeof(string) ? (string) Value : throw RequestedIncorrectValueType(typeof(string));
        public int    ValueAsInt    => ValueType == typeof(int) ? (int) Value : throw RequestedIncorrectValueType(typeof(int));
        public bool   ValueAsBool   => ValueType == typeof(bool) ? (bool) Value : throw RequestedIncorrectValueType(typeof(bool));

        private InvalidCastException RequestedIncorrectValueType(Type requestedType) {
            return new InvalidCastException($"The {nameof(GameOption)} {DisplayName} has the value {Value} of type {ValueType.Name}, but was requested as a {requestedType.Name}!");
        }

        #endregion
    }
}