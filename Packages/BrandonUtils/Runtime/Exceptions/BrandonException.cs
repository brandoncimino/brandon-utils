﻿using System;

namespace BrandonUtils.Exceptions {
    public class BrandonException : SystemException {
        public virtual  string BaseMessage   { get; } = "This was probably Brandon's fault. For support, call 203-481-1845.";
        public          string CustomMessage { get; }
        public override string Message       => FormatMessage(CustomMessage, BaseMessage);

        public BrandonException() : base() {
            CustomMessage = null;
        }

        public BrandonException(string message, Exception innerException = null) : base(message, innerException) {
            CustomMessage = message;
        }

        protected static string FormatMessage(string customMessage, string baseMessage) {
            return customMessage == null ? baseMessage : $"{customMessage}\n{baseMessage}";
        }
    }
}