using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using BrandonUtils.Strings;

using UnityEngine;
using UnityEngine.UI;

namespace BrandonUtils.Logging {
    [RequireComponent(typeof(Text))]
    public class LogUtils : MonoBehaviour {
        [Flags]
        public enum Locations {
            None,
            Console,
            Unity,
            UI,
            All = Console | Unity | UI
        }

        public const int LineLimit = 100;

        public static List<string> lines = new List<string>();

        public static string[] LimitedLines => lines
                                               .GetRange(
                                                   Math.Max(0, lines.Count - LineLimit),
                                                   Math.Min(lines.Count, LineLimit)
                                               )
                                               .ToArray();

        public static Locations locations = Locations.All;
        private       Text      _text;

        private void Awake() {
            _text = GetComponent<Text>();
            Log($"Started at: {DateTime.Now} with locations {locations}");
        }

        public static void Log(IDictionary<object, object> stuffToLog, string separator = "|") {
            var col1 = stuffToLog.Keys.Select(it => it.ToString()).ToList();
            var col2 = stuffToLog.Values.Select(it => it.ToString()).ToList();

            var w1 = col1.Max(it => it.Length);
            var w2 = col2.Max(it => it.Length);

            var rows = new string[stuffToLog.Count];

            for (int row = 0; row < stuffToLog.Count; row++) {
                var cell1 = col1[row].PadRight(w1);
                var cell2 = col2[row].PadRight(w2);
                rows[row] = ($"{cell1}{separator}{cell2}");
            }

            Log(rows);
        }

        public static void Log(params object[] stuffToLog) {
            Log(null, stuffToLog);
        }

        public static void Log(Color? color, params object[] stuffToLog) {
            foreach (var thing in stuffToLog) {
                lines.Add(thing.ToString().Colorize(color));
            }

            var joinedLines = string.Join("\n", stuffToLog);

            if (locations.HasFlag(Locations.Console)) {
                Console.WriteLine(joinedLines);
            }

            if (locations.HasFlag(Locations.Unity)) {
                Debug.Log(joinedLines);
            }
        }

        public void Update() {
            _text.enabled = locations.HasFlag(Locations.UI);

            //TODO: Is it necessary to check if _text.text needs to be changed before setting it (for efficiency), or is Unity smart enough to not do anything in that case?
            string newText = string.Join("\n", LimitedLines);
            if (_text.text != newText) {
                _text.text = newText;
            }
        }

        public static void Test(Expression<Func<bool>> condition, params object[] stuffToLog) {
            var result           = condition.Compile().Invoke();
            var conditionMessage = $"[{(result ? "PASS" : "FAIL")}] {condition}";
            stuffToLog = stuffToLog.Prepend(conditionMessage).ToArray();
            Log(result ? Color.green : Color.red, stuffToLog);
        }
    }
}