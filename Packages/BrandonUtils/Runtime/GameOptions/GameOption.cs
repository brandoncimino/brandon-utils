﻿using System;
using System.Linq;

using Newtonsoft.Json;

using UnityEngine;
using UnityEngine.Events;

namespace Packages.BrandonUtils.Runtime.GameOptions {
    public abstract class GameOption {
        /// <summary>
        /// The initial value for this <see cref="GameOption"/>.
        /// TODO: Does this mean it is the "Default" value? If so, should we rename it to "Default"? If not, should we add a "DefaultValue" field?
        /// </summary>
        [JsonProperty]
        public object InitialValue { get; }

        /// <summary>
        /// The serialized backing field for <see cref="Value"/>.
        /// </summary>
        [JsonProperty]
        private object _value;

        /// <summary>
        /// The current value of the <see cref="GameOption"/>.
        /// </summary>
        /// <remarks>
        /// Any value sent here should be constrained to <see cref="ValueType"/>, though that is not explicitly enforced as of 9/29/2020.
        /// </remarks>
        [JsonIgnore]
        public object Value {
            get => _value;
            set {
                var changed = _value != value;

                _value = value;
                //TODO: Attempt to enforce some amount of type-safety-ness by converting value to the correct type. At the very least, this should keep you from having to explicitly cast a float to an int before setting it as a Value.
                //_value = Convert.ChangeType(value,ValueType);

                if (changed) {
                    ValueChangedEvent.Invoke();
                }
            }
        }

        /// <summary>
        /// The expected <see cref="Type"/> of <see cref="Value"/>.
        /// </summary>
        /// <remarks>
        /// This <b>must</b> be defined by the inheritor - i.e. <see cref="int"/> in <see cref="SliderOption"/>.
        /// </remarks>
        [JsonProperty]
        public abstract Type ValueType { get; }

        /// <summary>
        /// A <b>verbose</b> description of what this <see cref="GameOption"/> does.
        /// </summary>
        /// TODO: Add an "Example" field for options?
        [JsonProperty]
        public string Description { get; }

        /// <summary>
        /// A user-friendly name for this <see cref="GameOption"/>.
        /// </summary>
        [JsonProperty]
        public string DisplayName { get; }

        /// <summary>
        /// A user-friendly representation of <see cref="Value"/>.
        /// </summary>
        /// <remarks>
        /// This can be "overridden" by individual instances of <see cref="GameOption"/>s by assigning a new method to <see cref="ValueDisplayFunction"/>.
        /// </remarks>
        [JsonIgnore]
        public string DisplayValue => ValueDisplayFunction(this).Stylize(ValueDisplayStyle);

        /// <summary>
        /// A user-friendly combination of <see cref="DisplayName"/> and <see cref="DisplayValue"/>.
        /// </summary>
        /// <remarks>
        /// This can be "overridden" by individual instances of <see cref="GameOption"/>s by assigning a new method to <see cref="LabelDisplayFunction"/>.
        /// </remarks>
        [JsonIgnore]
        public string DisplayLabel => LabelDisplayFunction(this);

        /// <summary>
        /// The method that renders the <see cref="DisplayLabel"/>.
        /// </summary>
        [JsonIgnore]
        public readonly Func<GameOption, string> LabelDisplayFunction;

        /// <summary>
        /// The method that renders the <see cref="DisplayValue"/>.
        /// </summary>
        [JsonIgnore]
        public readonly Func<GameOption, string> ValueDisplayFunction;

        /// <summary>
        /// Strings that can be considered as valid separators between the <see cref="DisplayName"/> and <see cref="DisplayValue"/> when building the <see cref="DisplayLabel"/>.
        /// </summary>
        [JsonIgnore]
        private const string Separators = "?:";

        /// <summary>
        /// The default separator used when building the <see cref="DisplayLabel"/>.
        /// </summary>
        [JsonIgnore]
        private const char Separator_Default = ':';

        /// <summary>
        /// The <see cref="FontStyle"/> to use for the <see cref="DisplayValue"/>, if any.
        /// </summary>
        [JsonProperty]
        public FontStyle ValueDisplayStyle = FontStyle.Bold;

        /// <summary>
        /// The <see cref="UnityEvent"/> triggered whenever <see cref="Value"/> <b>changes</b>.
        /// </summary>
        /// <remarks>
        /// This is <b>only</b> called when <see cref="set_Value"/> is called with a <b>different value</b>!
        /// </remarks>
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