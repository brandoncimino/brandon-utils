using UnityEngine;
using UnityEngine.UI;

namespace BrandonUtils.GameOptions {
    /// <summary>
    /// A <see cref="MonoBehaviour"/> that manages the UI components for a <see cref="GameOption"/>.
    /// </summary>
    /// <remarks>
    /// This <see cref="MonoBehaviour"/> is responsible for keeping the UI and <see cref="GameOption"/> synced up, by updating one whenever the other changes.
    /// </remarks>
    /// <typeparam name="TOption">The type of <see cref="GameOption"/> that this <see cref="GameOptionRenderer{TOption}"/> manages.</typeparam>
    public abstract class GameOptionRenderer<TOption> : MonoBehaviour where TOption : GameOption {
        /// <summary>
        /// The <see cref="GameOption"/> that this <see cref="GameOptionRenderer{TOption}"/> will manage.
        /// </summary>
        public TOption GameOption;
        /// <summary>
        /// The <see cref="Text"/> UI component that will display <see cref="GameOption"/>'s <see cref="GameOptions.GameOption.DisplayLabel"/>.
        /// </summary>
        public Text Label;

        /// <summary>
        /// Subscribe to <see cref="GameOption"/>'s <see cref="GameOptions.GameOption.ValueChangedEvent"/> and make sure the display is accurate via <see cref="UpdateDisplay_Full"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="GameOptions.GameOption.ValueChangedEvent"/> should be invoked whenever <see cref="GameOptions.GameOption.Value"/> changes - whether "directly" or via a UI callback like <see cref="Slider.onValueChanged"/>. Therefore, we must always update the display at that time.
        /// </remarks>
        protected virtual void Start() {
            GameOption.ValueChangedEvent.AddListener(UpdateDisplay_Full);
            UpdateDisplay_Full();
        }

        /// <summary>
        /// Updates <b>all</b> of the relevant UI components for this <see cref="GameOptionRenderer{TOption}"/>.
        /// </summary>
        /// <remarks>
        /// "Updating the display" is split into <b>2 methods</b>:
        /// <li><see cref="UpdateDisplay_Base"/> updates the components defined in the <b>base <see cref="GameOptionRenderer{TOption}"/> class</b>, e.g. <see cref="Label"/>.</li>
        /// <li><see cref="UpdateDisplay"/> updates the components defined in <b>inheritors of <see cref="GameOptionRenderer{TOption}"/></b>, e.g. <see cref="SliderOptionRenderer"/>.<see cref="SliderOptionRenderer.MinLabel"/>.</li>
        /// </remarks>
        private void UpdateDisplay_Full() {
            UpdateDisplay_Base();
            UpdateDisplay();
        }

        /// <summary>
        /// Updates the UI components associated with the base <see cref="GameOptionRenderer{TOption}"/> type, e.g. <see cref="Label"/>.
        /// </summary>
        private void UpdateDisplay_Base() {
            Label.text = GameOption.DisplayLabel;
        }

        protected abstract void UpdateDisplay();
    }
}