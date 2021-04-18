using System.Reflection;

using BrandonUtils.Logging;
using BrandonUtils.Standalone.Attributes;

using UnityEditor;

using UnityEngine;

namespace BrandonUtils.Editor {
    /// <summary>
    /// A <see cref="CustomEditor"/> for rendering special <see cref="BrandonUtils"/> component extensions, such as <see cref="EditorInvocationButtonAttribute"/>.
    /// </summary>
    [CustomEditor(typeof(MonoBehaviour), true)]
    public class MonoBehaviourEditor : UnityEditor.Editor {
        public override void OnInspectorGUI() {
            //Render the default MonoBehaviour GUI
            //Because this is placed at the _beginning_ of OnInspectorGUI, all of the custom GUI rendered by this method will be located _below all_ of the default GUI (as opposed to, say, in the order in which they are written in the code, the way that fields are displayed)
            base.OnInspectorGUI();

            //Go through all of the methods in the target type
            foreach (var method in target.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)) {
                //Check to see if the method has the EditorInvocationButtonAttribute
                var buttonAttribute = method.GetCustomAttribute<EditorInvocationButtonAttribute>();
                if (buttonAttribute != null) {
                    //Perform the extra validations of EditorInvocationButtonAttribute, throwing an error if we fail
                    buttonAttribute.ValidateMethodInfo(method);

                    //in one motion, both render the button _and_ check whether or not it was clicked
                    if (EditorInvocationButton(method)) {
                        LogUtils.Log($"<b>Invoke {method}</b>, declared by {method.DeclaringType}");
                        method.Invoke(target, new object[] { });
                    }
                }
            }
        }

        /// <summary>
        /// This <i>both</i> <b>renders</b> the <see cref="EditorInvocationButtonAttribute"/> <i>and</i> returns <b>whether or not it was clicked</b>!
        /// </summary>
        /// <param name="methodInfo"></param>
        /// <returns></returns>
        private static bool EditorInvocationButton(MethodInfo methodInfo) {
            return GUILayout.Button($"Invoke: <b>{methodInfo.Name}()</b>", new GUIStyle("Button") {richText = true, alignment = TextAnchor.MiddleLeft});
        }
    }
}