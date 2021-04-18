using System.Collections.Generic;
using System.Collections.ObjectModel;

using BrandonUtils.Standalone.Collections;
using BrandonUtils.Standalone.Enums;

using UnityEngine;

using static UnityEngine.KeyCode;

namespace BrandonUtils.Input {
    public static class KeyboardUtils {
        public static readonly EnumSubset<KeyCode> ArrowKeys = new EnumSubset<KeyCode>(
            new[] {
                UpArrow,
                LeftArrow,
                DownArrow,
                RightArrow
            },
            true
        );

        public static readonly EnumSubset<KeyCode> WASDKeys = new EnumSubset<KeyCode>(
            new[] {
                W,
                A,
                S,
                D
            },
            true
        );

        public static readonly EnumSubset<KeyCode> DirectionalKeys = new EnumSubset<KeyCode>(
            new[] {
                ArrowKeys,
                WASDKeys
            },
            true
        );

        public static readonly ReadOnlyDictionary<KeyCode, KeyCode> ArrowToWASD = new ReadOnlyDictionary<KeyCode, KeyCode>(
            new Dictionary<KeyCode, KeyCode> {
                {UpArrow, W},
                {DownArrow, S},
                {LeftArrow, A},
                {RightArrow, D}
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
            ArrowKeys.Validate(arrowKey);

            return ArrowToWASD[arrowKey];
        }

        public static KeyCode ToArrowKey(this KeyCode wasd) {
            WASDKeys.Validate(wasd);

            return WASDToArrow[wasd];
        }
    }
}