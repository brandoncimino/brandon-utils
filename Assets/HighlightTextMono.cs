using System.Collections.Generic;
using System.Linq;

using Packages.BrandonUtils.Runtime;
using Packages.BrandonUtils.Runtime.Logging;
using Packages.BrandonUtils.Runtime.UI;

using TMPro;

using UnityEngine;

public class HighlightTextMono : MonoBehaviour {
    public TMP_Text      TextMesh;
    public RectTransform Highlight;
    public RectTransform Img2;

    // Start is called before the first frame update
    void Start() { }

    // Update is called once per frame
    void Update() { }

    private TMP_Word RandomWord() {
        var ln    = Random.Range(1, TextMesh.text.Length);
        var start = Random.Range(0, TextMesh.text.Length - ln);
        LogUtils.Log(
            new Dictionary<object, object>() {
                {nameof(TextMesh.text), TextMesh.text},
                {nameof(TextMesh.text.Length), TextMesh.text.Length},
                {nameof(ln), ln},
                {nameof(start), start}
            }
        );
        // new TMP_Word(TextMesh, TextMesh.textInfo.characterInfo.Take(TextMesh.text.Length / 2));
        var chars = TextMesh.textInfo.characterInfo.ToList().GetRange(start, ln);
        var word  = new TMP_Word(TextMesh, chars);
        LogUtils.Log(Color.green, word);
        return word;
    }

    private TMP_Word FirstWord(int length) {
        var chars = TextMesh.textInfo.characterInfo.Take(length);
        return new TMP_Word(TextMesh, chars);
    }

    [EditorInvocationButton]
    public void HighlightRandom() {
        RandomWord().HighlightWord(Highlight);
    }

    [EditorInvocationButton]
    public void HighlightNew() {
        FirstWord(3).HighlightWord(Highlight);
    }

    public int hCount = 1;

    [EditorInvocationButton]
    public void HighlightCount() {
        FirstWord(hCount).HighlightWord(Highlight);
        hCount++;

        if (hCount > TextMesh.text.Length) {
            hCount = 1;
        }
    }


    [EditorInvocationButton]
    public void AlignLeft() {
        AlignHighlightToWord(RectTransform.Edge.Left);
    }

    [EditorInvocationButton]
    public void AlignRight() {
        AlignHighlightToWord(RectTransform.Edge.Right);
    }

    [EditorInvocationButton]
    public void AlignTop() {
        AlignHighlightToWord(RectTransform.Edge.Top);
    }

    [EditorInvocationButton]
    public void AlignBottom() {
        AlignHighlightToWord(RectTransform.Edge.Bottom);
    }

    private void AlignHighlightToWord(RectTransform.Edge edge) {
        Highlight.Align(edge, RandomWord().GetEdgePosition_AsFloat(edge));

        LogUtils.Log($"{edge} = {RandomWord().GetEdgePosition_AsFloat(edge)}");
    }

    private void AlignHighlightToImg2(RectTransform.Edge edge) {
        Highlight.Align(edge, Img2.GetEdgePosition_AsFloat(edge));
    }

    [EditorInvocationButton]
    public void Img2Left() {
        AlignHighlightToImg2(RectTransform.Edge.Left);
    }

    [EditorInvocationButton]
    public void Img2Right() {
        AlignHighlightToImg2(RectTransform.Edge.Right);
    }

    [EditorInvocationButton]
    public void Img2Top() {
        AlignHighlightToImg2(RectTransform.Edge.Top);
    }

    [EditorInvocationButton]
    public void Img2Bottom() {
        AlignHighlightToImg2(RectTransform.Edge.Bottom);
    }
}