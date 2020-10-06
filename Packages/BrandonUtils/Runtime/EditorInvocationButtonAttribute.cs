using System;
using System.Reflection;

using Packages.BrandonUtils.Runtime.Exceptions;

namespace Packages.BrandonUtils.Runtime {
    /// <summary>
    /// Creates a button in the Unity editor that will execute the annotated <see cref="AttributeTargets.Method"/>.
    /// </summary>
    /// <remarks>
    /// <li>The actual code that creates the button and invokes the method is located in the <c>BrandonUtils.Editor</c> namespace, in (as of 10/6/2020) <c>MonoBehaviorEditor</c> (which cannot be referenced here, because this is a <see cref="Runtime"/> class).</li>
    /// <li>Works with both public and private methods.</li>
    /// <li>Works with both static and instance methods.</li>
    /// <li>Works with both declared and inherited methods.</li>
    /// <li>Works in both Editor and Play mode.</li>
    /// <li>While it is theoretically possible to pass values to the <c>MonoBehaviorEditor</c> invocation, it is currently unsupported.</li>
    /// <li><see cref="ValidateMethodInfo"/> will throw an exception if the given <c>"MonoBehaviorEditor"</c> is invalid - which in this case would be anything that contains parameters.</li>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Method)]
    public class EditorInvocationButtonAttribute : Attribute {
        public static void ValidateMethodInfo(MethodInfo methodInfo) {
            if (methodInfo.GetParameters().Length != 0) {
                throw new BrandonException($"Methods annotated with {nameof(EditorInvocationButtonAttribute)} must have exactly 0 parameters, but {methodInfo.Name} has {methodInfo.GetParameters().Length}!");
            }
        }

        public void ValidateMethod(MethodInfo methodInfo) {
            ValidateMethodInfo(methodInfo);
        }
    }
}