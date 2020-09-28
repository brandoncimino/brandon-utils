using UnityEngine;
using UnityEngine.UI;

namespace Packages.BrandonUtils.Runtime.GameOptions {
    public abstract class GameOptionRenderer<TOption> : MonoBehaviour where TOption : GameOption {
        public TOption GameOption;
        public Text    Label;

        protected virtual void Start() {
            UpdateDisplay();
        }

        protected virtual void Update() {
            UpdateDisplay_Privately();
            UpdateDisplay();
        }

        private void UpdateDisplay_Privately() {
            Label.text = GameOption.DisplayLabel;
        }

        protected abstract void UpdateDisplay();
    }
}