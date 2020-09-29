using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace Packages.BrandonUtils.Runtime.GameOptions {
    public class GameOptionRenderManager : MonoBehaviour {
        public  GameObject          SliderPrefab;
        public  GameObject          TogglePrefab;
        private List<RectTransform> OptionPanels  = new List<RectTransform>();
        public  float               OptionPadding = 1;

        public RectTransform Render(GameOption gameOption, Transform parent = null) {
            parent = parent == null ? transform : parent;
            switch (gameOption) {
                case SliderOption sliderOption:
                    var sliderObject = Instantiate(SliderPrefab, parent);
                    sliderObject.GetComponent<SliderOptionRenderer>().GameOption = sliderOption;
                    return sliderObject.GetComponent<RectTransform>();

                case ToggleOption toggleOption:
                    var toggleObject = Instantiate(TogglePrefab, parent);
                    toggleObject.GetComponent<ToggleOptionRenderer>().GameOption = toggleOption;
                    return toggleObject.GetComponent<RectTransform>();

                default:
                    throw new InvalidOperationException("WHAT DID YOU DOOOOOOOO");
            }
        }

        public List<RectTransform> Render(IEnumerable<GameOption> gameOptions, Transform parent = null) {
            return gameOptions.Select(it => Render(it, parent)).ToList();
        }

        public List<RectTransform> Render(IGameMode gameOptions, Transform parent = null) {
            return Render(gameOptions.GameOptions, parent);
        }
    }
}