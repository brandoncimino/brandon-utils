using System;
using System.Linq;

using BrandonUtils.Strings;

using Newtonsoft.Json;

using UnityEngine;
using UnityEngine.Events;

namespace BrandonUtils.GameOptions {
    /// <summary>
    /// A configurable, renderable, serializable setting.
    /// </summary>
    [Serializable]
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
        /// This can be "overridden" by individual instances of <see cref="GameOption"/>s by assigning a new method to <see cref="DisplayValue_RenderFunction"/>.
        /// </remarks>
        [JsonIgnore]
        public string DisplayValue => DisplayValue_RenderFunction(this).Stylize(DisplayValue_Style);

        /// <summary>
        /// A user-friendly combination of <see cref="DisplayName"/> and <see cref="DisplayValue"/>.
        /// </summary>
        /// <remarks>
        /// This can be "overridden" by individual instances of <see cref="GameOption"/>s by assigning a new method to <see cref="DisplayLabel_RenderFunction"/>.
        /// </remarks>
        [JsonIgnore]
        public string DisplayLabel => DisplayLabel_RenderFunction(this);

        /// <summary>
        /// The method that renders the <see cref="DisplayLabel"/>.
        /// </summary>
        [JsonIgnore]
        public readonly Func<GameOption, string> DisplayLabel_RenderFunction;

        /// <summary>
        /// The method that renders the <see cref="DisplayValue"/>.
        /// </summary>
        [JsonIgnore]
        public readonly Func<GameOption, string> DisplayValue_RenderFunction;

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
        public FontStyle DisplayValue_Style = FontStyle.Bold;

        /// <summary>
        /// The <see cref="UnityEvent"/> triggered whenever <see cref="Value"/> <b>changes</b>.
        /// </summary>
        /// <remarks>
        /// This is <b>only</b> called when <see cref="set_Value"/> is called with a <b>different value</b>!
        /// </remarks>
        [JsonIgnore]
        public readonly UnityEvent ValueChangedEvent = new UnityEvent();

        protected GameOption(
            string                   displayName,
            string                   description,
            object                   initialValue,
            Func<GameOption, string> valueDisplayFunction,
            Func<GameOption, string> labelDisplayFunction
        ) {
            DisplayName                 = displayName;
            InitialValue                = initialValue;
            Value                       = initialValue;
            Description                 = description;
            DisplayValue_RenderFunction = valueDisplayFunction ?? DisplayValue_RenderFunction_Default;
            DisplayLabel_RenderFunction = labelDisplayFunction ?? DisplayLabel_RenderFunction_Default;
        }

        /// <summary>
        /// The default <see cref="DisplayLabel_RenderFunction"/> for <see cref="DisplayLabel"/>.
        /// </summary>
        /// <remarks>
        /// Joins <see cref="DisplayName"/> and <see cref="DisplayValue"/> together.
        /// <p/>
        /// <see cref="Separator_Default"/> placed between them, unless <see cref="DisplayName"/> ends with one of the <see cref="Separators"/>.
        /// </remarks>
        /// <example>
        /// When <see cref="DisplayName"/> does <b>not</b> end with any of the <see cref="Separators"/>:
        /// <br/>
        /// <see cref="DisplayName"/> = "Size" -> "Size: 5"
        /// <p/>
        /// When <see cref="DisplayName"/> <b>does</b> end with one of the <see cref="Separators"/>:
        /// <br/>
        /// <see cref="DisplayName"/> = "DTF?" -> "DTF? True"
        /// </example>
        /// <param name="gameOption"></param>
        /// <returns></returns>
        private static string DisplayLabel_RenderFunction_Default(GameOption gameOption) {
            var separator = Separators.Contains(gameOption.DisplayName.Last()) ? (char?)null : Separator_Default;
            return $"{gameOption.DisplayName}{separator} {gameOption.DisplayValue}";
        }

        /// <summary>
        /// The default <see cref="DisplayValue_RenderFunction"/> for <see cref="DisplayValue"/>.
        /// </summary>
        /// <remarks>
        /// Does <b>not</b> apply <see cref="DisplayValue_Style"/>.
        /// </remarks>
        /// <param name="gameOption"></param>
        /// <returns></returns>
        private static string DisplayValue_RenderFunction_Default(GameOption gameOption) {
            return gameOption.Value.ToString();
        }

        /// <summary>
        /// Returns <see cref="Value"/> as <typeparamref name="T"/> <b>if and only if</b> <see cref="ValueType"/> is <typeparamref name="T"/>.
        /// </summary>
        /// <remarks>
        /// This will <b>not</b> perform <b>any</b> coercion - not even from <see cref="long"/> to <see cref="int"/>!
        /// </remarks>
        /// <typeparam name="T">The expected <see cref="ValueType"/>.</typeparam>
        /// <returns></returns>
        public T ValueAs<T>() {
            Type requestedType = typeof(T);
            return ValueType == requestedType ? (T)Value : throw new InvalidCastException($"The {nameof(GameOption)} {DisplayName} has the value {Value} of type {ValueType.Name}, but was requested as a {requestedType.Name}!");
        }
    }
}