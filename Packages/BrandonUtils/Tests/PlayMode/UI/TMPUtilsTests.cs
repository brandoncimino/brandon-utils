using System;
using System.Linq;

using NUnit.Framework;

using Packages.BrandonUtils.Runtime.Strings;
using Packages.BrandonUtils.Runtime.UI;

using TMPro;

using UnityEngine;

using Object = UnityEngine.Object;

namespace Packages.BrandonUtils.Tests.EditMode.UI {
    public class TMPUtilsTests {
        private const string CANVAS_PATH = "TestMeshProCanvas";

        private string Prose = "This is the prose that I'm going to be putting into the TextMeshPro that I'm going to check for words and stuff.";

        public static TMP_Text InstantiateTextMeshPro(string content) {
            var canvasResource = Resources.Load<GameObject>(CANVAS_PATH);
            var canvasObject   = Object.Instantiate(canvasResource);
            var textMesh       = canvasObject.GetComponentInChildren<TMP_Text>();
            textMesh.text = content;
            return textMesh;
        }

        [Test]
        public void TestFirstSubstring(
            [Values("yolo")]
            string strToFind,
            [Values(10)]
            int repetitions,
            [Values] StringComparison stringComparison
        ) {
            var strContent = strToFind.Repeat(repetitions);
            var textMesh   = InstantiateTextMeshPro(strContent);
            var foundStr   = textMesh.FirstSubstring(strToFind, stringComparison);

            Assert.That(foundStr, Is.EqualTo(textMesh.textInfo.characterInfo.ToList().GetRange(0, strToFind.Length)));
        }
    }
}