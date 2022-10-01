﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using BrandonUtils.Standalone.Attributes;
using BrandonUtils.Standalone.Reflection;
using BrandonUtils.Standalone.Strings;
using BrandonUtils.Testing;

using NUnit.Framework;

using Is = BrandonUtils.Testing.Is;

namespace BrandonUtils.Tests.Standalone.Reflection {
    /// <summary>
    /// TODO: These tests need to be finished! It may be possible to organize them in a better way...
    /// </summary>
    [SuppressMessage("ReSharper", "ConvertToAutoProperty")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Local")]
    [SuppressMessage("ReSharper", "ConvertToAutoPropertyWhenPossible")]
    [SuppressMessage("ReSharper", "ConvertToAutoPropertyWithPrivateSetter")]
    public class ReflectionUtilsBackingFieldTests {
        private class WithAnnotatedField {
            #region Instance field + instance property

            #region Public Property

            [BackingFieldFor(nameof(PublicProperty_BackedBy_PublicField))]
            public int PublicField_For_PublicProperty;

            public int PublicProperty_BackedBy_PublicField {
                get => PublicField_For_PublicProperty;
                set => PublicField_For_PublicProperty = value;
            }

            [BackingFieldFor(nameof(PublicProperty_BackedBy_PrivateField))]
            private int PrivateField_For_PublicProperty;

            public int PublicProperty_BackedBy_PrivateField {
                get => PrivateField_For_PublicProperty;
                set => PrivateField_For_PublicProperty = value;
            }

            #endregion

            #region Private Property

            [BackingFieldFor(nameof(PrivateProperty_BackedBy_PublicField))]
            public int PublicField_For_PrivateProperty;
            private int PrivateProperty_BackedBy_PublicField {
                get => PublicField_For_PrivateProperty;
                set => PublicField_For_PrivateProperty = value;
            }

            [BackingFieldFor(nameof(PrivateProperty_BackedBy_PrivateField))]
            private int PrivateField_For_PrivateProperty;
            private int PrivateProperty_BackedBy_PrivateField {
                get => PrivateField_For_PrivateProperty;
                set => PrivateField_For_PrivateProperty = value;
            }

            #endregion

            #endregion

            #region Static field + static property

            [BackingFieldFor(nameof(PublicStaticProperty_BackedBy_PublicStaticField))]
            public static int PublicStaticField;
            public static int PublicStaticProperty_BackedBy_PublicStaticField {
                get => PublicStaticField;
                set => PublicStaticField = value;
            }

            [BackingFieldFor(nameof(PublicStaticProperty_BackedBy_PrivateStaticField))]
            private static int PrivateStaticField;
            public static int PublicStaticProperty_BackedBy_PrivateStaticField {
                get => PrivateStaticField;
                set => PrivateStaticField = value;
            }

            #endregion
        }

        private class WithAnnotatedProperty {
            private int Field;

            [BackedBy(nameof(Field))]
            public double Property {
                get => Field;
                set => Field = Convert.ToInt32(value);
            }
        }

        private class WithAutoProperty {
            public int AutoProperty { get; set; }
        }

        private class WithMultiplePropertiesOnOneField {
            [BackingFieldFor(nameof(Prop_SameType_GetOnly))]
            [BackingFieldFor(nameof(Prop_DifferentType_GetOnly))]
            [BackingFieldFor(nameof(Prop_SameType_GetSet))]
            private int PrivateField_For_MultipleProperties;
            private int UnusedField;

            public int    Prop_SameType_GetOnly      => PrivateField_For_MultipleProperties;
            public double Prop_DifferentType_GetOnly => PrivateField_For_MultipleProperties;
            public int Prop_SameType_GetSet {
                get => PrivateField_For_MultipleProperties;
                set => PrivateField_For_MultipleProperties = value;
            }

            public int UnusedProperty { get; set; }
        }

        private class WithMultiplePropertiesReferencingOneField {
            private int PrivateField_For_MultipleProperties;
            private int UnusedField;

            [BackedBy(nameof(PrivateField_For_MultipleProperties))]
            public int Prop_SameType_GetOnly => PrivateField_For_MultipleProperties;
            [BackedBy(nameof(PrivateField_For_MultipleProperties))]
            public double Prop_DifferentType_GetOnly => PrivateField_For_MultipleProperties;
            [BackedBy(nameof(PrivateField_For_MultipleProperties))]
            public int Prop_SameType_GetSet {
                get => PrivateField_For_MultipleProperties;
                set => PrivateField_For_MultipleProperties = value;
            }

            public int UnusedProperty { get; set; }
        }

        private class WithMixedAnnotationsAndMultipleProperties {
            [BackingFieldFor(nameof(Prop_DifferentType_GetOnly))]
            private int PrivateField_For_MultipleProperties;

            private int UnusedField;

            [BackedBy(nameof(PrivateField_For_MultipleProperties))]
            public int Prop_SameType_GetOnly => PrivateField_For_MultipleProperties;

            public double Prop_DifferentType_GetOnly => PrivateField_For_MultipleProperties;

            [BackedBy(nameof(PrivateField_For_MultipleProperties))]
            public int Prop_SameType_GetSet {
                get => PrivateField_For_MultipleProperties;
                set => PrivateField_For_MultipleProperties = value;
            }

            public int UnusedProperty { get; set; }

            [Test]
            public void TestPropertyBackingFields() {
                TestPropertiesAreBackedByField(this, nameof(PrivateField_For_MultipleProperties));
            }
        }

        private static void TestPropertiesAreBackedByField(object classWithFieldsAndStuff, string expectedBackingFieldName) {
            var allProps = classWithFieldsAndStuff
                           .GetType()
                           .GetProperties(ReflectionUtils.VariablesBindingFlags);
            var unusedProps = allProps
                              .Where(it => it.Name.Matches(@"Unused\w*"))
                              .ToList();
            var props = allProps
                        .Except(unusedProps)
                        .ToList();

            var expectedBackingField = classWithFieldsAndStuff.GetType()
                                                              .GetField(expectedBackingFieldName, ReflectionUtils.VariablesBindingFlags);

            AssertAll.Of(
                () => Assert.That(expectedBackingField, Is.Not.Null, $"{nameof(expectedBackingField)} named {expectedBackingFieldName}"),
                () => Validate.AllHaveBackingField(props, expectedBackingField),
                //TODO: I need to make an equivalent Constraint to AssertJ's "satisfies" and "allSatisfy" methods
                () => Assert.DoesNotThrow(() => unusedProps.ForEach(Validate.IsAutoProperty)),
                () => Assert.That(expectedBackingField?.BackedProperties().ToList(), Is.Not.Null.And.With.Count.EqualTo(props.Count))
            );
        }

        public static object[] TestInputs = {
            new WithAnnotatedField(),
            new WithAnnotatedProperty(),
            new WithAutoProperty(),
            new WithMixedAnnotationsAndMultipleProperties(),
            new WithMultiplePropertiesReferencingOneField()
        };
    }
}