using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using BrandonUtils.Standalone.Collections;
using BrandonUtils.Standalone.Enums;

using UnityEngine;

using static UnityEngine.KeyCode;

namespace BrandonUtils.Input {
    public static class KeyboardUtils {
        public static readonly EnumSet<KeyCode> ArrowKeys = EnumSet.Of(
            UpArrow,
            LeftArrow,
            DownArrow,
            RightArrow
        );

        public static readonly EnumSet<KeyCode> WASDKeys = EnumSet.Of(
            W,
            A,
            S,
            D
        );

        public static readonly EnumSet<KeyCode> DirectionalKeys = ArrowKeys.Union(WASDKeys).ToEnumSet();

        public static readonly ReadOnlyDictionary<KeyCode, KeyCode> ArrowToWASD = new ReadOnlyDictionary<KeyCode, KeyCode>(
            new Dictionary<KeyCode, KeyCode> {
                { UpArrow, W },
                { DownArrow, S },
                { LeftArrow, A },
                { RightArrow, D }
            }
        );

        public static readonly ReadOnlyDictionary<KeyCode, KeyCode> WASDToArrow = ArrowToWASD.Inverse();

        public static bool IsArrowKey(this KeyCode keyCode) {
            return ArrowKeys.Contains(keyCode);
        }

        public static bool IsWASD(this KeyCode keyCode) {
            return WASDKeys.Contains(keyCode);
        }

        public static bool IsDirectional(this KeyCode keyCode) {
            return IsArrowKey(keyCode) || IsWASD(keyCode);
        }

        public static KeyCode ToWASD(this KeyCode arrowKey) {
            ArrowKeys.MustContain(arrowKey);

            return ArrowToWASD[arrowKey];
        }

        public static KeyCode ToArrowKey(this KeyCode wasd) {
            WASDKeys.MustContain(wasd);

            return WASDToArrow[wasd];
        }
    }
}