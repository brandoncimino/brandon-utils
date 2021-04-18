using System;
using System.Collections.Generic;
using System.Linq;

using BrandonUtils.Collections;
using BrandonUtils.Enums;
using BrandonUtils.Logging;

using TMPro;

using UnityEngine;

using static UnityEngine.RectTransform.Edge;

namespace BrandonUtils.UI {
    // ReSharper disable once InconsistentNaming
    public class TMP_Word {
        public TMP_Text TextMesh;

        public List<TMP_CharacterInfo> Word { get; }

        public TMP_Word(TMP_Text textMesh, IEnumerable<TMP_CharacterInfo> wordCharacters) {
            this.TextMesh = textMesh;
            this.Word     = wordCharacters.ToList();
        }

        public float GetEdgePosition_AsFloat(RectTransform.Edge edge, bool extendToLine = false) {
            var edgeLocal  = GetEdgePosition_AsFloat_Local(edge);
            var parentAxis = TextMesh.rectTransform.GetAxisPosition_AsFloat(edge.Axis());
            return parentAxis + edgeLocal;
        }

        public float GetEdgePosition_AsFloat_Local(RectTransform.Edge edge) {
            switch (edge) {
                case Left:
                    return Word.First().bottomLeft.x;
                case Right:
                    return Word.Last().bottomRight.x;
                case Top:
                    return Word.Max(letter => letter.topLeft.y);
                case Bottom:
                    return Word.Min(letter => letter.bottomLeft.y);
                default:
                    throw EnumUtils.InvalidEnumArgumentException(nameof(edge), edge);
            }
        }

        public override string ToString() {
            return Word.Select(it => it.character).JoinString();
        }

        /// <summary>
        /// Returns the local position of <paramref name="word"/>'s <paramref name="textAnchor"/>, e.g. <see cref="TextAnchor.MiddleLeft"/>.
        /// </summary>
        /// <param name="textAnchor"></param>
        /// <returns></returns>
        public Vector2 AnchorPosition(TextAnchor textAnchor) {
            var leftPos = GetEdgePosition_AsFloat(Left);
            var width   = Width;
            var x       = leftPos + (textAnchor.Anchor().x * width);

            var bottomPos = GetEdgePosition_AsFloat(Bottom);
            var height    = Height;
            var y         = bottomPos + (textAnchor.Anchor().y * height);

            return new Vector2(x, y);
        }

        /// <summary>
        /// Returns the width of the <see cref="TMP_Word"/>, measured as the distance between the <see cref="RectTransform.Edge.Right"/> and <see cref="RectTransform.Edge.Left"/> (via <see cref="GetEdgePosition_AsFloat"/>)
        /// </summary>
        public float Width => GetEdgePosition_AsFloat(Right) - GetEdgePosition_AsFloat(Left);

        /// <summary>
        /// Returns the height of the <see cref="TMP_Word"/>, measured as the distance between the <see cref="RectTransform.Edge.Top"/> and <see cref="RectTransform.Edge.Bottom"/> (via <see cref="GetEdgePosition_AsFloat"/>)
        /// </summary>
        public float Height => GetEdgePosition_AsFloat(Top) - GetEdgePosition_AsFloat(Bottom);

        /// <summary>
        /// Returns the <see cref="AnchorPosition"/> converted to a <b>proportion</b> (<see cref="Mathf.Clamp01"/>ed) of <paramref name="parent"/>'s <see cref="RectTransform.sizeDelta"/>, for use as an <see cref="RectTransform.anchorMin"/> or <see cref="RectTransform.anchorMax"/>.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="textAnchor"></param>
        /// <returns></returns>
        public Vector2 WordAnchor(RectTransform parent, TextAnchor textAnchor) {
            var anchorPos = AnchorPosition(textAnchor);
            var sizeDelta = parent.sizeDelta;
            return new Vector2(
                anchorPos.x / sizeDelta.x,
                anchorPos.y / sizeDelta.y
            );
        }

        /// <summary>
        /// Combines <see cref="Width"/> and <see cref="Height"/> into a <see cref="Vector2"/> for use as a <see cref="RectTransform.sizeDelta"/>.
        /// </summary>
        /// <value></value>
        /// <exception cref="NotImplementedException"></exception>
        public Vector2 SizeDelta => new Vector2(Width, Height);

        public float Dimension(RectTransform.Axis dimension) {
            switch (dimension) {
                case RectTransform.Axis.Horizontal:
                    return Width;
                case RectTransform.Axis.Vertical:
                    return Height;
                default:
                    throw EnumUtils.InvalidEnumArgumentException(nameof(dimension), dimension);
            }
        }

        public RectTransform HighlightWord(RectTransform highlight) {
            highlight.sizeDelta = SizeDelta;
            highlight.Align(Top,  GetEdgePosition_AsFloat(Top));
            highlight.Align(Left, GetEdgePosition_AsFloat(Left));

            LogUtils.Log(
                new Dictionary<object, object>() {
                    {"word top", GetEdgePosition_AsFloat(Top)},
                    {"local top", GetEdgePosition_AsFloat_Local(Top)},
                    {"highlight top", highlight.GetEdgePosition_AsFloat(Top)}
                }
            );

            return highlight;
        }

        public void PadChar(int charIndex) {
            var c = Word[charIndex];
        }
    }
}