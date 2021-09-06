using System;
using System.Reflection;

using JetBrains.Annotations;

namespace BrandonUtils.Standalone.Attributes {
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
    [PublicAPI]
    public class BackingFieldForAttribute : BrandonAttribute {
        [NotNull] public string BackedPropertyName { get; }

        [CanBeNull]
        public PropertyInfo BackedProperty { get; }

        public BackingFieldForAttribute([NotNull] string backedPropertyName) {
            BackedPropertyName = backedPropertyName;
        }
    }
}