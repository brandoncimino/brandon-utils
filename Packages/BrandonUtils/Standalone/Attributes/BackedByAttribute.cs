﻿using System;

using JetBrains.Annotations;

namespace BrandonUtils.Standalone.Attributes {
    [PublicAPI]
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class BackedByAttribute : BrandonAttribute {
        public string BackingFieldName { get; }

        public BackedByAttribute(string backingFieldName) {
            BackingFieldName = backingFieldName;
        }
    }
}