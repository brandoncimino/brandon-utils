using System;
using System.Linq;

using BrandonUtils.Standalone.Collections;
using BrandonUtils.Standalone.Strings;
using BrandonUtils.Testing;
using BrandonUtils.UI;

using NUnit.Framework;

using TMPro;

using UnityEngine;

using Object = UnityEngine.Object;

namespace BrandonUtils.Tests.PlayMode.UI {
    public class TMPUtilsTests {
        #region Resources

        private const string CanvasPath    = "TestMeshProCanvas";
        private const string HighlightPath = "TestImage";

        public static TMP_Text InstantiateTextMeshPro(string content = null) {
            var canvasResource = Resources.Load<GameObject>(CanvasPath);
            var canvasObject   = Object.Instantiate(canvasResource);
            var textMesh       = canvasObject.GetComponentInChildren<TMP_Text>();
            textMesh.ForceMeshUpdate();

            if (content == null) {
                return textMesh;
            }

            var initialContent = textMesh.text;

            textMesh.text = content;

            Assert.That(
                textMesh.VisibleText(),
                Is.EqualTo(initialContent)
            );

            textMesh.ForceMeshUpdate();
            Assert.That(
                textMesh.VisibleText(),
                Is.EqualTo(content)
            );
            return textMesh;
        }

        public static RectTransform InstantiateHighlight() {
            var highlightResource = Resources.Load<GameObject>(HighlightPath);
            return Object.Instantiate(highlightResource).GetComponent<RectTransform>();
        }

        #endregion

        [Test]
        public void TestVisibleText() {
            var    text     = InstantiateTextMeshPro();
            string fullText = text.text;

            string[] words = {
                "one",
                "two-three",
                "yoloas;lkj;qa3lk4m;lk,mah",
                "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa",
                "bbbbbbbbbbbbbbbbbbbb",
                "ccccccccccc",
                "ddddddddddddddd",
                "eeeeeee",
                "fffff",
                "aabbcc",
                "b",
                text.text.Repeat(2)
            };


            foreach (var word in words) {
                // if the new word is longer than the current content...
                if (word.Length >= fullText.Length) {
                    fullText = word;
                }
                else {
                    fullText = word + fullText.Substring(word.Length, fullText.Length - word.Length);
                }

                text.text = word;
                text.ForceMeshUpdate();

                Assert.That(text.VisibleText(), Is.EqualTo(word));

                Assert.That(text.textInfo.characterInfo.Select(it => it.character).JoinString(), Is.EqualTo(fullText));
            }
        }

        public enum Find {
            First,
            Last
        }

        [TestCase(Find.First, "yolo", "swag yolo")]
        [TestCase(Find.First, "sw*g", "aALSKJDF;kljh;k2234h;kjhasd;l22.d2***sw*g.*", StringComparison.CurrentCulture)]
        [TestCase(Find.Last,  "yolo", "yo yo lo yolo loyo yolo swag swag yo")]
        public void TestSubstring(
            Find find,
            string searchFor,
            string searchIn,
            StringComparison stringComparison = StringComparison.Ordinal
        ) {
            var textMesh                  = InstantiateTextMeshPro(searchIn);
            var foundStr                  = find == Find.First ? textMesh.FirstSubstring(searchFor, stringComparison) : textMesh.LastSubstring(searchFor, stringComparison);
            var expectedIndex             = find == Find.First ? searchIn.IndexOf(searchFor, stringComparison) : searchIn.LastIndexOf(searchFor, stringComparison);
            var expectedCharacterInfoList = textMesh.textInfo.VisibleCharacterInfo().ToList().GetRange(expectedIndex, searchFor.Length);
            Assert.That(foundStr.Word,       Is.EqualTo(expectedCharacterInfoList));
            Assert.That(foundStr.ToString(), Is.EqualTo(searchFor));
        }

        [Test]
        public void TestWordEdge() { }

        [TestCase("butts'n'stuff")]
        public void TestHighlight(
            string searchIn
        ) {
            Assume.That(searchIn, Has.Length.GreaterThan(1));

            var textMesh  = InstantiateTextMeshPro(searchIn);
            var highlight = InstantiateHighlight();

            var word = new TMP_Word(textMesh, textMesh.textInfo.characterInfo.Take(searchIn.Length / 2).ToList());

            Assert.That(
                word.ToString(),
                Is.EqualTo(searchIn.Substring(0, searchIn.Length / 2))
            );

            word.HighlightWord(highlight);

            foreach (var e in (RectTransform.Edge[]) Enum.GetValues(typeof(RectTransform.Edge))) {
                float actual = word.GetEdgePosition_AsFloat(e);
                float expect = highlight.GetEdgePosition_AsFloat(e);
                Assert.That(
                    actual,
                    new ApproximationConstraint(expect, TestUtils.ApproximationThreshold)
                );
            }
        }
    }
}
