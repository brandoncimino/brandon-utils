using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

using BrandonUtils.Standalone.Attributes;
using BrandonUtils.Standalone.Collections;
using BrandonUtils.Standalone.Exceptions;
using BrandonUtils.Standalone.Strings;
using BrandonUtils.Standalone.Strings.Prettifiers;

using JetBrains.Annotations;

namespace BrandonUtils.Standalone.Reflection {
    /// <summary>
    /// Contains utilities for <see cref="System.Reflection"/>.
    ///
    /// TODO: Split this class up. For example, the <see cref="Type"/> extensions should be in their own class
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
            BindingFlags.Default   |
            BindingFlags.Instance  |
            BindingFlags.NonPublic |
            BindingFlags.Public    |
            BindingFlags.Static;

        public const BindingFlags ConstructorBindingFlags =
            BindingFlags.Instance |
            BindingFlags.Public   |
            BindingFlags.NonPublic;

        public const BindingFlags AutoPropertyBackingFieldBindingFlags =
            BindingFlags.Instance |
            BindingFlags.Static   |
            BindingFlags.NonPublic;

        private const string PropertyCaptureGroupName = "property";

        #region Variables

        /// <summary>
        /// Returns all of the <see cref="IsVariable"/> <see cref="MemberInfo"/>s from the given <paramref name="type"/>.
        /// </summary>
        /// <remarks>
        /// "Variables" includes both <see cref="Type.GetProperties()"/> and <see cref="Type.GetFields()"/>.
        /// It does <b>not</b> include <see cref="IsAutoPropertyBackingField">backing fields</see>.
        /// </remarks>
        /// <param name="type">The <see cref="Type"/> to retrieve the fields and properties of.</param>
        /// <returns>
        /// </returns>
        [NotNull]
        [ItemNotNull]
        public static IEnumerable<MemberInfo> GetVariables([NotNull] this Type type) {
            var properties = type.GetProperties(VariablesBindingFlags).Cast<MemberInfo>();
            var fields     = type.GetFields(VariablesBindingFlags).Cast<MemberInfo>();
            return properties.Union(fields).Where(IsVariable);
        }

        /// <summary>
        /// Returns the <see cref="FieldInfo"/> or <see cref="PropertyInfo"/> with the the <see cref="MemberInfo.Name"/> <paramref name="variableName"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> that should have a <see cref="FieldInfo"/> or <see cref="PropertyInfo"/> named <paramref name="variableName"/></param>
        /// <param name="variableName">The expected <see cref="FieldInfo"/> or <see cref="PropertyInfo"/> <see cref="MemberInfo.Name"/></param>
        /// <returns>The <see cref="FieldInfo"/> or <see cref="PropertyInfo"/> named <paramref name="variableName"/></returns>
        /// <exception cref="MissingMemberException">If <paramref name="variableName"/> couldn't be retrieved</exception>
        [Pure]
        [NotNull]
        public static MemberInfo GetVariableInfo(this Type type, string variableName) {
            var prop = type.GetProperty(variableName, VariablesBindingFlags);

            if (prop != null) {
                return prop;
            }

            var field = type.GetField(variableName, VariablesBindingFlags);
            return field ?? throw ReflectionException.VariableNotFoundException(type, variableName);
        }

        public static T ResetAllVariables<T>(this T objectWithVariables) where T : class {
            throw new NotImplementedException();

            var variables         = objectWithVariables.GetType().GetVariables();
            var settableVariables = variables.Where(IsSettable);
            foreach (var vInfo in settableVariables) {
                SetVariableValue(objectWithVariables, vInfo.Name, vInfo);
            }

            return objectWithVariables;
        }

        /// <param name="fieldOrProperty">either a <see cref="FieldInfo"/> or <see cref="PropertyInfo"/></param>
        /// <returns>true if the <paramref name="fieldOrProperty"/> <see cref="IsSettable(FieldInfo)"/> / <see cref="IsSettable(PropertyInfo)"/></returns>
        public static bool IsSettable(this MemberInfo fieldOrProperty) {
            return fieldOrProperty switch {
                PropertyInfo p => IsSettable(p),
                FieldInfo f    => IsSettable(f),
                _              => false
            };
        }

        /// <param name="propertyInfo">a <see cref="PropertyInfo"/></param>
        /// <returns><see cref="PropertyInfo.CanWrite"/></returns>
        private static bool IsSettable(this PropertyInfo propertyInfo) {
            return propertyInfo.CanWrite;
        }

        /// TODO: Should this have some way of actually checking for the <c>readonly</c> keyword...?
        private static bool IsSettable(this FieldInfo fieldInfo) {
            return !IsAutoPropertyBackingField(fieldInfo);
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
        /// Checks if a <see cref="MemberInfo"/> is a "variable", and therefore appropriate for use with "variable"-referencing methods like <see cref="GetVariableValue{T}"/>.
        /// </summary>
        /// <remarks>
        /// "Variables" includes both <see cref="Type.GetProperties()"/> and <see cref="Type.GetFields()"/>.
        /// It does <b>not</b> include <see cref="IsAutoPropertyBackingField">backing fields</see>.
        /// </remarks>
        /// <param name="memberInfo">a <see cref="MemberInfo"/></param>
        /// <returns>true if the <see cref="MemberInfo"/> is a variable</returns>
        [Pure]
        public static bool IsVariable(this MemberInfo memberInfo) {
            return memberInfo switch {
                PropertyInfo _ => true,
                FieldInfo f    => !IsAutoPropertyBackingField(f),
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
            var variableInfo = obj.GetType().GetVariableInfo(variableName);
            var value = variableInfo switch {
                PropertyInfo p => p.GetValue(obj),
                FieldInfo f    => f.GetValue(obj),
                _              => throw ReflectionException.NotAVariableException(variableInfo)
            };

            try {
                return (T)value;
            }
            catch (InvalidCastException e) {
                throw e.ModifyMessage($"A member for {variableInfo.Prettify(TypeNameStyle.Full)} was found on the [{obj.GetType().Name}]'{obj}', but it couldn't be cast to a {typeof(T).PrettifyType(default)}!");
            }
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

        public static void SetVariableValue<T>(object obj, string variableName, T newValue) {
            var v = obj.GetType().GetVariableInfo(variableName);
            switch (v) {
                case PropertyInfo prop:
                    prop.SetValue(obj, newValue);
                    return;
                case FieldInfo field:
                    field.SetValue(obj, newValue);
                    return;
                default:
                    throw new MemberAccessException($"Unable to set the value of {v.Prettify()}!");
            }
        }

        #endregion

        #region Backing Fields

        #region Operations on Fields

        [NotNull]
        [ItemNotNull]
        [Pure]
        [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter")]
        private static IEnumerable<string> Get_BackingFieldFor_Names([NotNull] FieldInfo fieldInfo) {
            return fieldInfo.GetCustomAttributes<BackingFieldForAttribute>().Select(it => it.BackedPropertyName);
        }

        [Pure]
        [NotNull]
        [ItemNotNull]
        private static IEnumerable<PropertyInfo> Get_BackingFieldFor_Properties([NotNull] FieldInfo fieldInfo, Type owningType = default) {
            owningType ??= GetOwningType(fieldInfo);
            var backedPropertyNames = Get_BackingFieldFor_Names(fieldInfo);
            return owningType.GetProperties()
                             .Where(it => backedPropertyNames.Contains(it.Name));
        }

        /// <param name="memberInfo"></param>
        /// <returns><c>true</c> if <paramref name="memberInfo"/> is an <a href="https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/auto-implemented-properties">Auto-Property Backing Field</a></returns>
        [Pure]
        internal static bool IsAutoPropertyBackingField([NotNull] this FieldInfo memberInfo) {
            return Regex.IsMatch(memberInfo.Name, GetAutoPropertyBackingFieldNamePattern());
        }

        [Pure]
        [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter")]
        private static bool UsedIn_BackedBy([NotNull] FieldInfo fieldInfo) {
            // TODO: pass to other method
            var owningType = fieldInfo.ReflectedType ?? fieldInfo.DeclaringType ?? throw ReflectionException.NoOwningTypeException(fieldInfo);
            return owningType.GetProperties(VariablesBindingFlags)
                             .Select(Get_BackedBy_BackingFieldName)
                             .Any(it => it == fieldInfo.Name);
        }

        [Pure]
        private static bool IsAnnotatedBackingField([NotNull] FieldInfo fieldInfo) {
            return Get_BackingFieldFor_Names(fieldInfo).IsNotEmpty() || UsedIn_BackedBy(fieldInfo);
        }

        [PublicAPI]
        [Pure]
        public static bool IsBackingField([NotNull] this MemberInfo fieldInfo) {
            return (fieldInfo as FieldInfo)?.IsBackingField() ?? throw ReflectionException.NotAFieldException(fieldInfo);
        }

        [PublicAPI]
        [Pure]
        public static bool IsBackingField([NotNull] this FieldInfo fieldInfo) {
            return IsAutoPropertyBackingField(fieldInfo) || IsAnnotatedBackingField(fieldInfo);
        }

        [CanBeNull]
        [Pure]
        [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter")]
        internal static string Get_BackedAutoProperty_Name([NotNull] FieldInfo fieldInfo) {
            var matches = Regex.Match(fieldInfo.Name, GetAutoPropertyBackingFieldNamePattern());
            return matches.Success ? matches.Groups[PropertyCaptureGroupName].Value : null;
        }

        [CanBeNull]
        [Pure]
        private static PropertyInfo Get_BackedAutoProperty([NotNull] FieldInfo fieldInfo, Type owningType = default) {
            owningType ??= GetOwningType(fieldInfo);
            var autoPropertyName = Get_BackedAutoProperty_Name(fieldInfo);
            return autoPropertyName == null ? null : owningType.GetProperty(autoPropertyName);
        }

        [NotNull]
        [ItemNotNull]
        [Pure]
        [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter")]
        private static IEnumerable<PropertyInfo> Get_PropertiesAnnotatedWith_BackedBy([NotNull] FieldInfo fieldInfo, Type owningType = default) {
            owningType ??= GetOwningType(fieldInfo);
            return owningType.GetProperties(VariablesBindingFlags)
                             .Where(it => Get_BackedBy_BackingFieldName(it) == fieldInfo.Name);
        }

        [NotNull]
        [ItemNotNull]
        [PublicAPI]
        public static IEnumerable<PropertyInfo> BackedProperties([NotNull] this FieldInfo fieldInfo) {
            var owningType = GetOwningType(fieldInfo);

            var propertiesFromFieldAnnotations = Get_BackingFieldFor_Properties(fieldInfo, owningType);
            var propertiesReferencingThisField = Get_PropertiesAnnotatedWith_BackedBy(fieldInfo, owningType);
            var autoProperty                   = Get_BackedAutoProperty(fieldInfo, owningType);

            return propertiesFromFieldAnnotations
                   .Union(propertiesReferencingThisField)
                   .Append(autoProperty)
                   .NonNull();
        }

        #endregion

        #region Operations on Properties

        [CanBeNull]
        [Pure]
        [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter")]
        private static string Get_BackedBy_BackingFieldName([NotNull] PropertyInfo propertyInfo) {
            return propertyInfo.GetCustomAttribute<BackedByAttribute>()?.BackingFieldName;
        }

        [NotNull]
        [ItemNotNull]
        [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter")]
        private static IEnumerable<FieldInfo> Get_FieldsAnnotatedWith_BackingFieldFor([NotNull] PropertyInfo propertyInfo, Type owningType = default) {
            owningType ??= GetOwningType(propertyInfo);
            return owningType.GetFields(VariablesBindingFlags)
                             .Where(it => Get_BackingFieldFor_Names(it).Contains(propertyInfo.Name));
        }

        [CanBeNull]
        private static FieldInfo Get_BackedBy_Field([NotNull] PropertyInfo propertyInfo, Type owningType = default) {
            owningType ??= GetOwningType(propertyInfo);
            var backingFieldName = Get_BackedBy_BackingFieldName(propertyInfo);
            return backingFieldName == null ? null : owningType.GetField(backingFieldName, VariablesBindingFlags);
        }

        [Pure]
        [NotNull]
        [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter")]
        private static string GetAutoPropertyBackingFieldName([NotNull] PropertyInfo propertyInfo) {
            return $"<{propertyInfo.Name}>k__BackingField";
        }

        [Pure]
        [CanBeNull]
        [PublicAPI]
        public static FieldInfo BackingField([NotNull] this MemberInfo memberInfo) {
            return (memberInfo as PropertyInfo)?.BackingField() ?? throw ReflectionException.NotAPropertyException(memberInfo);
        }

        /// <summary>
        /// Returns the "backing field" for a <see cref="PropertyInfo"/>.
        ///
        /// A "backing field" can be:
        /// <ul>
        /// <li>an <a href="https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/auto-implemented-properties">Auto-Property backing field</a></li>
        /// <li><paramref name="propertyInfo"/>'s [<see cref="BackedByAttribute.BackingFieldName"/>].<see cref="BackedByAttribute"/></li>
        /// <li>any <see cref="BackingFieldForAttribute"/> whose [<see cref="BackingFieldForAttribute.BackedProperty"/>].<see cref="BackingFieldForAttribute"/> matches <paramref name="propertyInfo"/></li>
        /// </ul>
        /// TODO: Improve this so that the <see cref="PropertyInfo"/> can reference <see cref="FieldInfo"/>s, which can then "chain" down into a <see cref="FieldInfo"/>
        /// </summary>
        /// <param name="propertyInfo">this <see cref="FieldInfo"/></param>
        /// <returns>the <see cref="FieldInfo"/> that backs <paramref name="propertyInfo"/>, if found; otherwise, null</returns>
        [CanBeNull]
        [PublicAPI]
        [Pure]
        public static FieldInfo BackingField([NotNull] this PropertyInfo propertyInfo) {
            var owningType = GetOwningType(propertyInfo);

            return GetAutoPropertyBackingField(propertyInfo, owningType) ??
                   Get_BackedBy_Field(propertyInfo, owningType) ??
                   Get_FieldsAnnotatedWith_BackingFieldFor(propertyInfo, owningType).Single();
        }

        #endregion

        /// <remarks>
        /// <paramref name="propertyNamePattern"/> will be stored in a capture group named <see cref="PropertyCaptureGroupName"/>.
        /// </remarks>
        /// <param name="propertyNamePattern">A pattern to match the auto-property. Defaults to <c>.*</c></param>
        /// <returns>A <see cref="Regex"/> pattern that will match <a href="https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/auto-implemented-properties">Auto-Property backing names</a>.</returns>
        [Pure]
        [NotNull]
        private static string GetAutoPropertyBackingFieldNamePattern(
            [RegexPattern] [NotNull]
            string propertyNamePattern = ".*"
        ) {
            return $"<(?<{PropertyCaptureGroupName}>{propertyNamePattern})>k__BackingField";
        }

        [CanBeNull]
        [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter")]
        private static FieldInfo GetAutoPropertyBackingField([NotNull] PropertyInfo propertyInfo, Type owningType = default) {
            owningType ??= GetOwningType(propertyInfo);
            return owningType.GetField(GetAutoPropertyBackingFieldName(propertyInfo), AutoPropertyBackingFieldBindingFlags);
        }

        #endregion

        private static Type GetOwningType(MemberInfo memberInfo) {
            return memberInfo.ReflectedType ?? memberInfo.DeclaringType ?? throw ReflectionException.NoOwningTypeException(memberInfo);
        }

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
                return (TOut)constructor.Invoke(parametersValues);
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

        #region Generics

        /// <param name="type">a <see cref="Type"/> that might be generic</param>
        /// <returns><see cref="Type.IsGenericType"/> || <see cref="Type.IsGenericTypeDefinition"/></returns>
        [ContractAnnotation("null => false")]
        public static bool IsGenericTypeOrDefinition([CanBeNull] this Type type) {
            return type != null && (type.IsGenericType || type.IsGenericTypeDefinition);
        }

        [ContractAnnotation("null => false")]
        public static bool IsEnumerable([CanBeNull] this Type type) {
            if (type == null) {
                return false;
            }

            return type.IsGenericTypeOrDefinition() && type.GetGenericTypeDefinition() == typeof(IEnumerable<>)
                   || type.IsArray
                   || type.FindInterfaces()
                          .Where(it => it.IsGenericTypeOrDefinition())
                          .Select(it => it.GetGenericTypeDefinition())
                          .Contains(typeof(IEnumerable<>));
        }

        private static readonly Type[] TupleTypes = {
            typeof(ValueTuple),
            typeof(ValueTuple<>),
            typeof(ValueTuple<,>),
            typeof(ValueTuple<,,>),
            typeof(ValueTuple<,,,>),
            typeof(ValueTuple<,,,,>),
            typeof(ValueTuple<,,,,,>),
            typeof(ValueTuple<,,,,,,>),
            typeof(ValueTuple<,,,,,,,>),
            // NOTE: typeof(Tuple) is a static utility class
            typeof(Tuple<>),
            typeof(Tuple<,>),
            typeof(Tuple<,,>),
            typeof(Tuple<,,,>),
            typeof(Tuple<,,,,>),
            typeof(Tuple<,,,,,>),
            typeof(Tuple<,,,,,,>),
            typeof(Tuple<,,,,,,,>),
        };

        /// <remarks>
        /// This is only necessary in .NET Standard 2.0, because in .NET Standard 2.1, an <a href="https://docs.microsoft.com/en-us/dotnet/api/System.Runtime.CompilerServices.ITuple?view=netframework-4.7.1">ITuple</a> interface is available.
        /// </remarks>
        /// <param name="type">a <see cref="Type"/></param>
        /// <returns>true if the given <see cref="Type"/> is one of the <see cref="Tuple{T}"/> or <see cref="ValueTuple{T1}"/> types</returns>
        public static bool IsTupleType([NotNull] this Type type) {
            return type.IsGenericTypeOrDefinition() && TupleTypes.Any(it => type.GetGenericTypeDefinition().IsAssignableFrom(it));
        }

        #endregion

        #region Type Ancestry

        [NotNull]
        internal static Type CommonType([CanBeNull, ItemCanBeNull, InstantHandle] IEnumerable<Type> types) {
            if (types == null) {
                return typeof(object);
            }

            types = types.ToList();
            var baseClass = CommonBaseClass(types);
            if (baseClass != typeof(object)) {
                return baseClass;
            }

            var commonInterfaces = CommonInterfaces(types);
            return commonInterfaces.FirstOrDefault() ?? typeof(object);
        }

        internal static Type CommonBaseClass(IEnumerable<Type> types) {
            Type mostCommonType = default;

            foreach (var t in types) {
                mostCommonType = CommonBaseClass(mostCommonType, t);
                if (mostCommonType == typeof(object)) {
                    return mostCommonType;
                }
            }

            return mostCommonType ?? typeof(object);
        }

        [CanBeNull]
        public static Type CommonBaseClass([CanBeNull] Type a, [CanBeNull] Type b) {
            if (a == null || b == null) {
                return a ?? b;
            }

            while (true) {
                if (a == b) {
                    return a;
                }

                if (a.IsAssignableFrom(b)) {
                    return a;
                }

                if (b.IsAssignableFrom(a)) {
                    return b;
                }

                if (a.BaseType == null) {
                    return typeof(object);
                }

                a = a.BaseType;
            }
        }

        // [CanBeNull]
        // internal static Type CommonInterface([CanBeNull] Type a, [CanBeNull] Type b) {
        //     var overlap = CommonInterfaces(a, b).ToList();
        //     return overlap.FirstOrDefault();
        // }

        [NotNull]
        [ItemNotNull]
        internal static IEnumerable<Type> CommonInterfaces([CanBeNull] Type a, [CanBeNull] Type b) {
            var aInts = a.GetAllInterfaces();
            var bInts = b.GetAllInterfaces();
            return aInts.Intersect(bInts);
        }

        [Pure, NotNull]
        internal static IEnumerable<Type> CommonInterfaces([NotNull, ItemCanBeNull] IEnumerable<Type> types) {
            return types.Select(it => it.GetAllInterfaces()).Intersection();
        }


        public static IEnumerable<Type> GetAllInterfaces([CanBeNull] this Type type) {
            return _GetAllInterfaces(type);
        }

        [Pure]
        [NotNull, ItemNotNull]
        public static Type[] FindInterfaces([NotNull] this Type type) {
            if (type == null) {
                throw new ArgumentNullException(nameof(type));
            }

            return type.FindInterfaces((type1, criteria) => true, default);
        }

        private static IEnumerable<Type> _GetAllInterfaces([CanBeNull] this Type type, [CanBeNull] IEnumerable<Type> soFar = default) {
            var ints = (type?.GetInterfaces()).NonNull().ToList();
            soFar = soFar.NonNull();
            if (type?.IsInterface == true) {
                soFar = soFar.Union(type);
            }

            return (soFar ?? new List<Type>())
                   .Union(ints)
                   .Concat(ints.SelectMany(ti => _GetAllInterfaces(ti, soFar)))
                   .Distinct();
        }

        private static IEnumerable<Type> Ancestors(this Type type) {
            var ancestors = new List<Type>();

            while (type != null) {
                ancestors.Add(type);
                type = type.BaseType;
            }

            return ancestors;
        }

        /// <summary>
        /// An idiomatic inverse of <see cref="Type.IsAssignableFrom"/> because I always get confused by that.
        /// </summary>
        /// <param name="valueType"></param>
        /// <param name="variableType"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        [PublicAPI]
        [Pure]
        [ContractAnnotation("valueType:null => stop")]
        [ContractAnnotation("variableType:null => stop")]
        public static bool CanBeAssignedTo([NotNull] this Type valueType, [NotNull] Type variableType) {
            if (valueType == null) {
                throw new ArgumentNullException(nameof(valueType));
            }

            if (variableType == null) {
                throw new ArgumentNullException(nameof(variableType));
            }

            return variableType.IsAssignableFrom(valueType);
        }

        /// <summary>
        /// An idiomatic alias for <see cref="Type.IsAssignableFrom"/> because I always get confused by that.
        /// </summary>
        /// <param name="variableType"></param>
        /// <param name="valueType"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        [PublicAPI]
        [Pure]
        [ContractAnnotation("variableType:null => stop")]
        [ContractAnnotation("valueType:null => stop")]
        public static bool CanHoldValueOf([NotNull] this Type variableType, [NotNull] Type valueType) {
            if (variableType == null) {
                throw new ArgumentNullException(nameof(variableType));
            }

            if (valueType == null) {
                throw new ArgumentNullException(nameof(valueType));
            }

            return variableType.IsAssignableFrom(valueType);
        }

        #endregion

        #region Type Keywords

        private static readonly Dictionary<Type, string> TypeKeywords = new Dictionary<Type, string>() {
            [typeof(int)]     = "int",
            [typeof(uint)]    = "uint",
            [typeof(short)]   = "short",
            [typeof(ushort)]  = "ushort",
            [typeof(long)]    = "long",
            [typeof(ulong)]   = "ulong",
            [typeof(double)]  = "double",
            [typeof(float)]   = "float",
            [typeof(bool)]    = "bool",
            [typeof(byte)]    = "byte",
            [typeof(decimal)] = "decimal",
            [typeof(sbyte)]   = "sbyte",
            [typeof(char)]    = "char",
            [typeof(object)]  = "object",
            [typeof(string)]  = "string"
        };

        [Pure]
        [NotNull]
        [ContractAnnotation("null => stop")]
        public static string NameOrKeyword([NotNull] this Type type) {
            if (type == null) {
                throw new ArgumentNullException(nameof(type));
            }

            return TypeKeywords.GetOrDefault(type, () => type.Name) ?? throw new InvalidOperationException();
        }

        #endregion
    }
}