using System.Collections.Generic;
using System.Linq;

using DefaultNamespace;

using Packages.BrandonUtils.Runtime.GameOptions;
using Packages.BrandonUtils.Runtime.UI;

using UnityEngine;

public class GameModeDebugManager : GameOptionRenderManager {
    public  RectTransform       OptionHolderPanel;
    private IGameMode           GameMode = new DebugGameMode();
    private List<RectTransform> RenderedOptions;

    public GameObject SimpleImage;

    public List<RectTransform> SimpleImages;

    public int ImagesToCreate;

    // Start is called before the first frame update
    void Start() {
        RenderedOptions = Render(GameMode);
        RenderedOptions.First().Align(RectTransform.Edge.Top, OptionHolderPanel.GetEdgePosition_AsFloat(RectTransform.Edge.Top));
        RectTransformUtils.LineUp(RenderedOptions, RectTransform.Edge.Bottom);
        RenderedOptions.ForEach(it => it.Align(RectTransform.Edge.Left, OptionHolderPanel.GetEdgePosition_AsFloat(RectTransform.Edge.Left)));

        SimpleImages = CreateImages(SimpleImage, ImagesToCreate);
        RectTransformUtils.LineUp(OptionHolderPanel, SimpleImages, RectTransform.Edge.Bottom);
        RenderedOptions.ForEach(it => it.Align(RectTransform.Edge.Left, OptionHolderPanel.GetEdgePosition_AsFloat((RectTransform.Edge.Left))));
    }

    // Update is called once per frame
    void Update() { }

    private List<RectTransform> CreateImages(GameObject image, int count) {
        return Enumerable.Repeat(0, count).Select(it => Instantiate(image, transform)).Select(it => it.GetComponent<RectTransform>()).ToList();
    }
}