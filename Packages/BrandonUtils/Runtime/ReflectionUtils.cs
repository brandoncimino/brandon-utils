using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

using JetBrains.Annotations;

using Packages.BrandonUtils.Runtime.Exceptions;

namespace Packages.BrandonUtils.Runtime {
    /// <summary>
    /// Contains utilities for <see cref="System.Reflection"/>.
    /// </summary>
    public static class ReflectionUtils {
        /// <summary>
        /// <see cref="BindingFlags"/> that correspond to all "variables",
        /// which should be all <see cref="PropertyInfo"/>s and <see cref="FieldInfo"/>s
        /// (including <see cref="BindingFlags.NonPublic"/> and <see cref="BindingFlags.Static"/>)
        ///<br/>
        /// TODO: Why does this not include <see cref="BindingFlags.GetProperty"/>, <see cref="BindingFlags.SetProperty"/>, <see cref="BindingFlags.GetField"/> and <see cref="BindingFlags.SetField"/>?
        ///     If I include the property and field flags, will the flags still work if I pass them to <see cref="Type.GetProperties()"/> and <see cref="Type.GetFields()"/>?
        ///     With the current implementation, what is returned when I use the flags with <see cref="Type.GetMembers()"/>?
        /// </summary>
        /// <remarks>
        /// These binding flags will also return <a href="https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/auto-implemented-properties">Auto-Property backing fields</a>.
        /// Some methods, such as <see cref="GetVariables"/>, specifically filter out backing fields.
        /// </remarks>
        public const BindingFlags VariablesBindingFlags =
            BindingFlags.Default |
            BindingFlags.Instance |
            BindingFlags.NonPublic |
            BindingFlags.Public |
            BindingFlags.Static;

        private const string PropertyCaptureGroupName = "property";

        /// <summary>
        /// Returns the <see cref="MethodInfo"/> for this method that <b><i>called <see cref="ThisMethod"/></i></b>.
        /// </summary>
        /// <returns></returns>
        public static MethodInfo ThisMethod() {
            throw new NotImplementedException("TBD - I need to stop getting distracted!");
        }

        /// <summary>
        /// Returns all of the <see cref="VariablesBindingFlags">"variables"</see> from the given <paramref name="type"/>.
        /// </summary>
        /// <remarks>
        /// "Variables" includes both <see cref="Type.GetProperties()"/> and <see cref="Type.GetFields()"/>.
        /// It does <b>not</b> include <see cref="IsBackingField">backing fields</see>.
        /// </remarks>
        /// <param name="type">The <see cref="Type"/> to retrieve the fields and properties of.</param>
        /// <returns>
        /// </returns>
        public static List<MemberInfo> GetVariables(this Type type) {
            // TODO: There's some warning here that I should probably resolve, about co-variant types or something...
            MemberInfo[] properties = type.GetProperties(VariablesBindingFlags);
            MemberInfo[] fields     = type.GetFields(VariablesBindingFlags);
            return properties.Concat(fields).Where(it => !it.IsBackingField()).ToList();
        }

        /// <summary>
        /// Returns the <see cref="FieldInfo"/> or <see cref="PropertyInfo"/> with the the <see cref="MemberInfo.Name"/> <paramref name="variableName"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> that should have a <see cref="FieldInfo"/> or <see cref="PropertyInfo"/> named <paramref name="variableName"/></param>
        /// <param name="variableName">The expected <see cref="FieldInfo"/> or <see cref="PropertyInfo"/> <see cref="MemberInfo.Name"/></param>
        /// <returns>The <see cref="FieldInfo"/> or <see cref="PropertyInfo"/> named <paramref name="variableName"/></returns>
        /// <exception cref="MissingMemberException">If <paramref name="variableName"/> couldn't be retrieved</exception>
        [Pure]
        public static MemberInfo GetVariableInfo(this Type type, string variableName) {
            var prop = type.GetProperty(variableName, VariablesBindingFlags);
            if (!(prop is null)) {
                return prop;
            }

            var field = type.GetField(variableName, VariablesBindingFlags);
            if (!(field is null)) {
                return field;
            }

            throw new MissingMemberException($"The {nameof(type)} {type} did not have a field or property named {variableName}!");
        }

        /// <summary>
        /// <inheritdoc cref="GetVariableInfo"/>
        /// </summary>
        /// <param name="obj">An object to infer <typeparamref name="T"/> from</param>
        /// <param name="variableName"><inheritdoc cref="GetVariableInfo"/></param>
        /// <typeparam name="T">The type to retrieve <paramref name="variableName"/> from</typeparam>
        /// <returns><inheritdoc cref="GetVariableInfo"/></returns>
        [Pure]
        public static MemberInfo GetVariableInfo<T>(T obj, string variableName) {
            return typeof(T).GetVariableInfo(variableName);
        }


        /// <summary>
        /// <inheritdoc cref="GetVariableInfo"/>
        /// </summary>
        /// <param name="variableName"><inheritdoc cref="GetVariableInfo"/></param>
        /// <typeparam name="T">The type to retrieve <paramref name="variableName"/> from</typeparam>
        /// <returns><inheritdoc cref="GetVariableInfo"/></returns>
        [Pure]
        public static MemberInfo GetVariableInfo<T>(string variableName) {
            return typeof(T).GetVariableInfo(variableName);
        }

        /// <summary>
        /// Returns the <b>value</b> (either <see cref="PropertyInfo"/>.<see cref="PropertyInfo.GetValue(object)"/> or <see cref="FieldInfo"/>.<see cref="FieldInfo.GetValue"/>)
        /// of the <see cref="GetVariableInfo"/> named <paramref name="variableName"/>.
        /// </summary>
        /// <param name="obj">The object to retrieve the value from</param>
        /// <param name="variableName">The name of the variable</param>
        /// <typeparam name="T">The <b>return type</b> of <b><paramref name="variableName"/></b></typeparam>
        /// <returns>The value of the <see cref="GetVariableInfo"/></returns>
        /// <exception cref="InvalidCastException">If the retrieved value cannot be cast to <typeparamref name="T"/></exception>
        /// <exception cref="MissingMemberException"><inheritdoc cref="GetVariableInfo"/></exception>
        [Pure]
        public static T GetVariable<T>(object obj, string variableName) {
            var v = obj.GetType().GetVariableInfo(variableName);
            switch (v) {
                case PropertyInfo prop:
                    try {
                        return (T) prop.GetValue(obj);
                    }
                    catch (InvalidCastException e) {
                        throw new InvalidCastException($"A property named {variableName} was found for the {obj.GetType().Name} {obj}, but it couldn't be cast to a {typeof(T).Name}!", e);
                    }
                case FieldInfo field:
                    try {
                        return (T) field.GetValue(obj);
                    }
                    catch (InvalidCastException e) {
                        throw new InvalidCastException($"A field named {variableName} was found for the {obj.GetType().Name} {obj}, but it couldn't be cast to a {typeof(T).Name}!", e);
                    }
            }

            throw new MissingMemberException($"Couldn't find a field or property named {variableName} for the type {obj.GetType().Name}!");
        }

        /// <summary>
        /// <inheritdoc cref="GetVariable{T}"/>
        /// </summary>
        /// <param name="obj"><inheritdoc cref="GetVariable{T}"/></param>
        /// <param name="variableName"><inheritdoc cref="GetVariable{T}"/></param>
        /// <returns><inheritdoc cref="GetVariable{T}"/></returns>
        [Pure]
        public static object GetVariable(object obj, string variableName) {
            return GetVariable<object>(obj, variableName);
        }

        public static void SetVariable<T>(object obj, string variableName, T value) {
            var v = obj.GetType().GetVariableInfo(variableName);
            switch (v) {
                case PropertyInfo prop:
                    prop.SetValue(obj, value);
                    return;
                case FieldInfo field:
                    field.SetValue(obj, value);
                    return;
                default:
                    throw new BrandonException($"Couldn't find a field or property named {variableName} for the type {obj.GetType()}!");
            }
        }

        [Pure]
        public static bool IsEmpty(object thing) {
            if (null == thing) {
                return true;
            }

            switch (thing) {
                case string str:
                    return string.IsNullOrWhiteSpace(str);
                case IEnumerable<object> enumerable:
                    return !enumerable.Any();
                default:
                    return false;
            }
        }

        [Pure]
        public static bool IsNotEmpty(object thing) {
            return !IsEmpty(thing);
        }

        #region Backing Fields

        /// <remarks>
        /// <paramref name="propertyNamePattern"/> will be stored in a capture group named <see cref="PropertyCaptureGroupName"/>.
        /// </remarks>
        /// <param name="propertyNamePattern">A pattern to match the auto-property. Defaults to <c>.*</c></param>
        /// <returns>A <see cref="Regex"/> pattern that will match <a href="https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/auto-implemented-properties">Auto-Property backing names</a>.</returns>
        [Pure]
        private static string GetBackingFieldNamePattern(
            [RegexPattern]
            string propertyNamePattern = ".*"
        ) {
            return $"<(?<{PropertyCaptureGroupName}>{propertyNamePattern})>k__BackingField";
        }

        [Pure]
        private static string BackingFieldName(
            this MemberInfo memberInfo
        ) {
            return memberInfo.MemberType != MemberTypes.Property ? null : $"<{memberInfo.Name}>k__BackingField";
        }

        /// <param name="memberInfo"></param>
        /// <returns><c>true</c> if <paramref name="memberInfo"/> is an <a href="https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/auto-implemented-properties">Auto-Property Backing Field</a></returns>
        [Pure]
        public static bool IsBackingField(this MemberInfo memberInfo) {
            return memberInfo.MemberType == MemberTypes.Field && Regex.IsMatch(memberInfo.Name, GetBackingFieldNamePattern());
        }

        [Pure]
        public static string GetBackedPropertyName([NotNull] this MemberInfo memberInfo) {
            return Regex.Match(memberInfo.Name, GetBackingFieldNamePattern()).Groups[PropertyCaptureGroupName].Value;
        }

        [Pure]
        public static FieldInfo BackingField([NotNull] this MemberInfo memberInfo) {
            if (memberInfo.DeclaringType == null) {
                throw new BrandonException($"The provided {nameof(MemberInfo)}, {memberInfo.Name}, does not have a {nameof(MemberInfo.DeclaringType)}!");
            }

            return memberInfo.MemberType != MemberTypes.Property
                       ? null
                       : memberInfo.DeclaringType.GetField(
                           memberInfo.BackingFieldName(),
                           BindingFlags.Instance |
                           BindingFlags.Static |
                           BindingFlags.NonPublic
                       );
        }

        #endregion
    }
}