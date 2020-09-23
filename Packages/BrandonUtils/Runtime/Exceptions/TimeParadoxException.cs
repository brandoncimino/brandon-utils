﻿using System;

namespace Packages.BrandonUtils.Runtime.Exceptions {
    public class TimeParadoxException : BrandonException {
        public override string BaseMessage { get; } = "\t\tWhen am I?";
        public TimeParadoxException() { }
        public TimeParadoxException(string message, Exception innerException = null) : base(message, innerException) { }
    }
}