using System;
using System.Linq;

using NUnit.Framework;

using Packages.BrandonUtils.Runtime.Collections;
using Packages.BrandonUtils.Runtime.Strings;
using Packages.BrandonUtils.Runtime.UI;

using TMPro;

using UnityEngine;

using Object = UnityEngine.Object;

namespace Packages.BrandonUtils.Tests.PlayMode.UI {
    public class TMPUtilsTests {
        private const string CanvasPath = "TestMeshProCanvas";

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
                Assert.That(FullText(text),     Is.EqualTo(fullText));
            }
        }

        private static string FullText(TMP_Text text) {
            return text.textInfo.characterInfo.Select(it => it.character).JoinString();
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
            var textMesh = InstantiateTextMeshPro(searchIn);
            var foundStr = find == Find.First ? textMesh.FirstSubstring(searchFor, stringComparison) : textMesh.LastSubstring(searchFor, stringComparison);

            var expectedIndex = find == Find.First ? searchIn.IndexOf(searchFor, stringComparison) : searchIn.LastIndexOf(searchFor, stringComparison);

            Assert.That(foundStr, Is.EqualTo(textMesh.textInfo.VisibleCharacterInfo().ToList().GetRange(expectedIndex, searchFor.Length)));

            Assert.That(foundStr.Select(it => it.character).JoinString(), Is.EqualTo(searchFor));
        }
    }
}