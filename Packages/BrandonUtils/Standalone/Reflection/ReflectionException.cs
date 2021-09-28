﻿using System;
using System.Reflection;

using BrandonUtils.Standalone.Strings;

using JetBrains.Annotations;

namespace BrandonUtils.Standalone.Reflection {
    public static class ReflectionException {
        private static readonly PrettificationSettings PrettificationSettings = TypeNameStyle.Full;

        internal static ArgumentException NotAFieldException([NotNull] MemberInfo notField, Exception innerException = null) {
            // TODO: Put the DeclaringType.Name pattern into a prettifier
            return new ArgumentException($"{notField.Prettify(PrettificationSettings)} isn't a {MemberTypes.Field}!", innerException);
        }

        internal static ArgumentException NotAPropertyException(MemberInfo notProperty, Exception innerException = null) {
            return new ArgumentException($"{notProperty.Prettify(PrettificationSettings)} isn't a {MemberTypes.Property}!", innerException);
        }

        internal static ArgumentException NotAVariableException(MemberInfo notVariable, Exception innerException = null) {
            return new ArgumentException($"{nameof(MemberInfo)} {notVariable.DeclaringType}.{notVariable.Name} isn't a 'Variable' (either a property or a non-backing-field)!", innerException);
        }

        internal static MissingMemberException VariableNotFoundException([NotNull] Type type, [NotNull] string variableName) {
            return new MissingMemberException($"The {nameof(type)} {type} did not have a field or property named {variableName}!");
        }

        internal static NullReferenceException NoOwningTypeException([NotNull] MemberInfo memberInfo) {
            return new NullReferenceException($"Somehow, {memberInfo.Prettify()} doesn't have a {nameof(MemberInfo.ReflectedType)} OR {nameof(MemberInfo.DeclaringType)}...!");
        }
    }
}