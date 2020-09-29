using UnityEngine;
using UnityEngine.UI;

namespace Packages.BrandonUtils.Runtime.GameOptions {
    public abstract class GameOptionRenderer<TOption> : MonoBehaviour where TOption : GameOption {
        public TOption GameOption;
        public Text    Label;

        protected virtual void Start() {
            GameOption.ValueChangedEvent.AddListener(UpdateDisplay_Privately);
            UpdateDisplay_Privately();
        }

        protected virtual void Update() { }

        private void UpdateDisplay_Privately() {
            Label.text = GameOption.DisplayLabel;
            UpdateDisplay();
        }

        protected abstract void UpdateDisplay();
    }
}