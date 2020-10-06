using Packages.BrandonUtils.Runtime.Enums;

using UnityEngine;

namespace Packages.BrandonUtils.Runtime.UI {
    public static class TextAnchorUtils {
        public static Vector2 Anchor(this TextAnchor textAnchor) {
            switch (textAnchor) {
                case TextAnchor.UpperLeft:
                    return new Vector2(0, 1);
                case TextAnchor.UpperCenter:
                    return new Vector2(0.1f, 1);
                case TextAnchor.UpperRight:
                    return new Vector2(1, 1);
                case TextAnchor.MiddleLeft:
                    return new Vector2(0, 0.5f);
                case TextAnchor.MiddleCenter:
                    return new Vector2(0.5f, 0.5f);
                case TextAnchor.MiddleRight:
                    return new Vector2(1, 0.5f);
                case TextAnchor.LowerLeft:
                    return new Vector2(0, 0);
                case TextAnchor.LowerCenter:
                    return new Vector2(0.5f, 0);
                case TextAnchor.LowerRight:
                    return new Vector2(1, 0);
                default:
                    throw EnumUtils.InvalidEnumArgumentException(nameof(textAnchor), textAnchor);
            }
        }
    }
}