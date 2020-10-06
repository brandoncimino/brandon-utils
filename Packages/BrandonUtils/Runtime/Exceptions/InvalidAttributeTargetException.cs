﻿using System;
using System.Reflection;

namespace Packages.BrandonUtils.Runtime.Exceptions {
    public class InvalidAttributeTargetException<T> : BrandonException {
        private readonly MemberInfo BadTarget;
        public override  string     BaseMessage => $"The custom attribute {typeof(T).Name} was set to an invalid target, {StringUtils.FormatMember(BadTarget)}!";

        public InvalidAttributeTargetException() { }

        public InvalidAttributeTargetException(string message, MemberInfo badTarget, Exception innerException = null) : base(message, innerException) {
            BadTarget = badTarget;
        }
    }
}