using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

using BrandonUtils.Standalone.Exceptions;

using JetBrains.Annotations;

namespace BrandonUtils.Standalone {
    /// <summary>
    /// Contains utilities for <see cref="System.Reflection"/>.
    /// </summary>
    public static class ReflectionUtils {
        /// <summary>
        /// Returns the <see cref="MethodInfo"/> for this method that <b><i>called <see cref="ThisMethod"/></i></b>.
        /// </summary>
        /// <returns></returns>
        public static MethodInfo ThisMethod() {
            throw new NotImplementedException("TBD - I need to stop getting distracted!");
        }

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

        public const BindingFlags ConstructorBindingFlags =
            BindingFlags.Instance |
            BindingFlags.Public |
            BindingFlags.NonPublic;

        private const string PropertyCaptureGroupName = "property";

        #region Variables

        private static ArgumentException NotVariableException(MemberInfo memberInfo, Exception innerException = null) {
            return new ArgumentException($"{nameof(MemberInfo)} {memberInfo.DeclaringType}.{memberInfo.Name} isn't a 'Variable' (either a property or a non-backing-field)!", innerException);
        }

        private static void ValidateIsVariable(MemberInfo memberInfo) {
            if (!memberInfo.IsVariable()) {
                throw NotVariableException(memberInfo);
            }
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
            return properties.Union(fields).Where(it => !it.IsBackingField()).ToList();
            ;
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

        public static T ResetAllVariables<T>(this T objectWithVariables) where T : class {
            var variables         = objectWithVariables.GetType().GetVariables();
            var settableVariables = variables.Where(IsSettable);
            foreach (var vInfo in settableVariables) {
                SetVariableValue(objectWithVariables, vInfo.Name, vInfo);
            }

            return objectWithVariables;
        }

        public static bool IsSettable(this MemberInfo fieldOrProperty) {
            return fieldOrProperty switch {
                PropertyInfo p => IsSettable(p),
                FieldInfo f    => IsSettable(f),
                _              => false
            };
        }

        private static bool IsSettable(this PropertyInfo propertyInfo) {
            return propertyInfo.CanWrite;
        }

        private static bool IsSettable(this FieldInfo fieldInfo) {
            return !fieldInfo.IsBackingField();
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

        public static bool IsVariable(this MemberInfo memberInfo) {
            return memberInfo switch {
                PropertyInfo p => true,
                FieldInfo f    => !f.IsBackingField(),
                _              => false
            };
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
        public static T GetVariableValue<T>(object obj, string variableName) {
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
        /// <inheritdoc cref="GetVariableValue{T}"/>
        /// </summary>
        /// <param name="obj"><inheritdoc cref="GetVariableValue{T}"/></param>
        /// <param name="variableName"><inheritdoc cref="GetVariableValue{T}"/></param>
        /// <returns><inheritdoc cref="GetVariableValue{T}"/></returns>
        [Pure]
        public static object GetVariableValue(object obj, string variableName) {
            return GetVariableValue<object>(obj, variableName);
        }

        public static void SetVariableValue<T>(object obj, string variableName, T value) {
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

        #endregion

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
            //TODO: fix this annotation
            //[RegexPattern]
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

        #region Constructors

        /// <summary>
        /// Returns a <see cref="ConstructorInfo"/> with parameter types matching <see cref="parameterTypes"/> if one exists;
        /// otherwise throws a <see cref="MissingMethodException"/>.
        /// </summary>
        /// <remarks>
        /// This is a wrapper around <see cref="Type.GetConstructor(Type[])"/>
        /// that:
        /// <ul>
        ///     <li>Will throw an exception on failure rather than returning null</li>
        /// <li>Accepts <see cref="Type"/>s as <a href="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/params">params</a> (aka <a href="https://docs.oracle.com/javase/8/docs/technotes/guides/language/varargs.html">varargs</a> aka <a href="https://en.wikipedia.org/wiki/Variadic_function">variadic</a> aka arbitrary <a href="https://en.wikipedia.org/wiki/Arity">arity</a>)</li>
        /// </ul>
        ///
        /// </remarks>
        /// <param name="type"></param>
        /// <param name="parameterTypes"></param>
        /// <returns></returns>
        /// <exception cref="MissingMethodException"></exception>
        [Pure]
        public static ConstructorInfo EnsureConstructor(this Type type, params Type[] parameterTypes) {
            return type.GetConstructor(ConstructorBindingFlags, null, parameterTypes, null) ?? throw new MissingMethodException($"Could not retrieve a constructor for {type}");
        }

        private static TOut Construct<TOut>(Type[] parameterTypes, object[] parametersValues) {
            try {
                var constructor = typeof(TOut).EnsureConstructor(parameterTypes);
                return (TOut) constructor.Invoke(parametersValues);
            }
            catch (Exception e) {
                throw e.PrependMessage($"Could not construct an instance of {typeof(TOut).Name} with the parameters {parametersValues}!");
            }
        }

        /// <summary>
        /// Constructs a new instance of <see cref="TOut"/> using a constructor that <b>exactly</b> matches the given <see cref="parameters"/>.
        ///
        /// <p/><b>NOTE:</b><br/>
        /// This must match the desired constructor <b>EXACTLY</b> - that means that the parameter types must be exact, and that optional parameters must be present!
        ///
        /// </summary>
        /// <remarks>
        /// I tried to make this an extension method of <see cref="Type"/>, but couldn't get it to work:
        /// Unfortunately, I don't think it's possible, in C#, to go from a <see cref="Type"/> to a generic type parameter in a way that the
        /// compiler knows about - i.e., in a way equivalent to Java's <![CDATA[Class<T>]]>.
        ///
        /// This means that making this a <see cref="Type"/> extension method is redundant with <typeparamref name="TOut"/>,
        /// other than allowing this to be an extension method and thus giving you a way to find it that is semantically similar
        /// to <see cref="Type.GetConstructor(System.Reflection.BindingFlags,System.Reflection.Binder,System.Reflection.CallingConventions,System.Type[],System.Reflection.ParameterModifier[])">Type.GetConstructor()</see>.
        /// </remarks>
        /// <param name="parameters">the parameters of the constructor to be called</param>
        /// <typeparam name="TOut">the desired type to be constructed</typeparam>
        /// <returns>an instance of <see cref="TOut"/> constructed with <paramref name="parameters"/></returns>
        public static TOut Construct<TOut>(params object[] parameters) {
            return Construct<TOut>(
                parameters.Select(it => it.GetType()).ToArray(),
                parameters
            );
        }

        #endregion
    }
}