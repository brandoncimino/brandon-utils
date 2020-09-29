using System;
using System.Linq;

using Newtonsoft.Json;

using UnityEngine;
using UnityEngine.Events;

namespace Packages.BrandonUtils.Runtime.GameOptions {
    public abstract class GameOption {
        [JsonProperty]
        public object InitialValue { get; }

        [JsonProperty]
        private object _value;
        [JsonIgnore]
        public object Value {
            get => _value;
            set {
                var changed = _value != value;

                _value = value;

                if (changed) {
                    ValueChangedEvent.Invoke();
                }
            }
        }

        [JsonProperty]
        public abstract Type ValueType { get; }

        [JsonProperty]
        public string Description { get; }

        [JsonProperty]
        public string DisplayName { get; }

        [JsonIgnore]
        public string DisplayValue => ValueDisplayFunction(this).Stylize(ValueDisplayStyle);

        [JsonIgnore]
        public string DisplayLabel => LabelDisplayFunction(this);

        [JsonIgnore]
        public readonly Func<GameOption, string> LabelDisplayFunction;

        [JsonIgnore]
        public readonly Func<GameOption, string> ValueDisplayFunction;

        [JsonIgnore]
        private const string Separators = "?:";

        [JsonIgnore]
        private const char Separator_Default = ':';

        [JsonProperty]
        public FontStyle ValueDisplayStyle = FontStyle.Bold;

        [JsonIgnore]
        public readonly UnityEvent ValueChangedEvent = new UnityEvent();

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

        [JsonIgnore]
        public string ValueAsString => ValueType == typeof(string) ? (string) Value : throw RequestedIncorrectValueType(typeof(string));

        [JsonIgnore]
        public int ValueAsInt => ValueType == typeof(int) ? (int) Value : throw RequestedIncorrectValueType(typeof(int));

        [JsonIgnore]
        public bool ValueAsBool => ValueType == typeof(bool) ? (bool) Value : throw RequestedIncorrectValueType(typeof(bool));

        private InvalidCastException RequestedIncorrectValueType(Type requestedType) {
            return new InvalidCastException($"The {nameof(GameOption)} {DisplayName} has the value {Value} of type {ValueType.Name}, but was requested as a {requestedType.Name}!");
        }

        #endregion
    }
}