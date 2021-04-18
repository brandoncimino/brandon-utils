using BrandonUtils.Collections;

using UnityEditor;

using UnityEngine;

namespace BrandonUtils.Editor {
    [CustomPropertyDrawer(typeof(Pair<,>))]
    public class PairDrawer : PropertyDrawer {
        // Draw the property inside the given rect
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property.
            EditorGUI.BeginProperty(position, label, property);

            // Draw label
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            //Don't make child fields be indented
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            // Calculate rects
            const int padding   = 5;
            const int margin    = 10;
            var       width     = ((position.width - margin) + padding) / 2;
            var       keyRect   = new Rect(position.x,             position.y, width, position.height);
            var       valueRect = new Rect(keyRect.xMax + padding, position.y, width, position.height);

            // Draw fields - pass GUIContent.none to each so they are drawn without labels
            EditorGUI.PropertyField(keyRect,   property.FindPropertyRelative(nameof(Pair<object, object>.X)), GUIContent.none);
            EditorGUI.PropertyField(valueRect, property.FindPropertyRelative(nameof(Pair<object, object>.Y)), GUIContent.none);

            // Set indent back to what it was
            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }
    }
}