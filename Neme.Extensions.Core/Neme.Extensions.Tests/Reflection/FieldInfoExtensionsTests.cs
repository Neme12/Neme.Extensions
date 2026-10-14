using System.Reflection;
using Neme.Extensions.Reflection;

namespace Neme.Extensions.Tests.Reflection;

public sealed class FieldInfoExtensionsTests
{
#if NETCOREAPP
    // Tests for CreateDelegate methods (ref-returning, .NET Core only)
    public sealed class CreateDelegate
    {
        // Test class with instance fields (shared across tests)
        private class TestClass
        {
            public int InstanceField = 42;
        }

        // Separate test classes for static field tests to avoid parallel test interference
        private class TestClassForStaticFieldCreatesDelegate
        {
            public static int StaticField = 100;
        }

        private class TestClassForStaticFieldDelegateCanAccess
        {
            public static int StaticField = 100;
        }

        private class TestClassForStaticFieldCreatesStaticDelegate
        {
            public static int StaticField = 100;
        }

        private class TestClassForStaticFieldCreatesTypedDelegate
        {
            public static int StaticField = 100;
        }

        private class TestClassForStaticFieldCreatesTypedDelegateWithTarget
        {
            public static int StaticField = 100;
        }

        // Delegate types that match the ref return signatures
        private delegate ref int StaticFieldDelegate();
        private delegate ref int OpenInstanceFieldDelegate(TestClass instance);

        public sealed class CreateDelegateWithDelegateType
        {
            [Fact]
            public void NullField_ThrowsArgumentNullException()
            {
                FieldInfo field = null!;
                Type delegateType = typeof(StaticFieldDelegate);

                Assert.Throws<ArgumentNullException>("field", () => FieldInfoExtensions.CreateDelegate(field, delegateType));
            }

            [Fact]
            public void NullDelegateType_ThrowsArgumentNullException()
            {
                var field = typeof(TestClass).GetField(nameof(TestClass.InstanceField))!;
                Type delegateType = null!;

                Assert.Throws<ArgumentNullException>("delegateType", () => field.CreateDelegate(delegateType));
            }

            [Fact]
            public void StaticField_CreatesDelegate()
            {
                var field = typeof(TestClassForStaticFieldCreatesDelegate).GetField(nameof(TestClassForStaticFieldCreatesDelegate.StaticField))!;

                var del = (StaticFieldDelegate)field.CreateDelegate(typeof(StaticFieldDelegate));

                Assert.NotNull(del);
                ref int value = ref del();
                Assert.Equal(100, value);
                value = 150;
                Assert.Equal(150, TestClassForStaticFieldCreatesDelegate.StaticField);
            }

            [Fact]
            public void StaticField_DelegateCanAccessField()
            {
                var field = typeof(TestClassForStaticFieldDelegateCanAccess).GetField(nameof(TestClassForStaticFieldDelegateCanAccess.StaticField))!;
                var del = (StaticFieldDelegate)field.CreateDelegate(typeof(StaticFieldDelegate));

                ref int value = ref del();

                Assert.Equal(100, value);
                value = 200;
                Assert.Equal(200, TestClassForStaticFieldDelegateCanAccess.StaticField);
            }

            [Fact]
            public void InstanceField_CreatesOpenInstanceDelegate()
            {
                var field = typeof(TestClass).GetField(nameof(TestClass.InstanceField))!;
                var instance = new TestClass { InstanceField = 42 };

                var del = (OpenInstanceFieldDelegate)field.CreateDelegate(typeof(OpenInstanceFieldDelegate));

                Assert.NotNull(del);
                ref int value = ref del(instance);
                Assert.Equal(42, value);
                value = 84;
                Assert.Equal(84, instance.InstanceField);
            }

            [Fact]
            public void InstanceField_DelegateCanAccessField()
            {
                var field = typeof(TestClass).GetField(nameof(TestClass.InstanceField))!;
                var instance = new TestClass { InstanceField = 123 };
                var del = (OpenInstanceFieldDelegate)field.CreateDelegate(typeof(OpenInstanceFieldDelegate));

                ref int value = ref del(instance);

                Assert.Equal(123, value);
                value = 456;
                Assert.Equal(456, instance.InstanceField);
            }
        }

        public sealed class CreateDelegateWithDelegateTypeAndTarget
        {
            [Fact]
            public void NullField_ThrowsArgumentNullException()
            {
                FieldInfo field = null!;
                Type delegateType = typeof(StaticFieldDelegate);
                object? target = new TestClass();

                Assert.Throws<ArgumentNullException>("field", () => FieldInfoExtensions.CreateDelegate(field, delegateType, target));
            }

            [Fact]
            public void NullDelegateType_ThrowsArgumentNullException()
            {
                var field = typeof(TestClass).GetField(nameof(TestClass.InstanceField))!;
                Type delegateType = null!;
                object? target = new TestClass();

                Assert.Throws<ArgumentNullException>("delegateType", () => field.CreateDelegate(delegateType, target));
            }

            [Fact]
            public void StaticField_CreatesStaticDelegate()
            {
                var field = typeof(TestClassForStaticFieldCreatesStaticDelegate).GetField(nameof(TestClassForStaticFieldCreatesStaticDelegate.StaticField))!;
                object? target = new TestClass();

                var del = (StaticFieldDelegate)field.CreateDelegate(typeof(StaticFieldDelegate), target);

                Assert.NotNull(del);
                ref int value = ref del();
                Assert.Equal(100, value);
                value = 250;
                Assert.Equal(250, TestClassForStaticFieldCreatesStaticDelegate.StaticField);
            }

            [Fact]
            public void InstanceField_NullTarget_CreatesOpenInstanceDelegate()
            {
                var field = typeof(TestClass).GetField(nameof(TestClass.InstanceField))!;
                object? target = null;
                var instance = new TestClass { InstanceField = 55 };

                var del = (OpenInstanceFieldDelegate)field.CreateDelegate(typeof(OpenInstanceFieldDelegate), target);

                Assert.NotNull(del);
                ref int value = ref del(instance);
                Assert.Equal(55, value);
                value = 110;
                Assert.Equal(110, instance.InstanceField);
            }

            [Fact]
            public void InstanceField_NonNullTarget_CreatesClosedInstanceDelegate()
            {
                var field = typeof(TestClass).GetField(nameof(TestClass.InstanceField))!;
                var target = new TestClass { InstanceField = 777 };

                var del = (StaticFieldDelegate)field.CreateDelegate(typeof(StaticFieldDelegate), target);

                Assert.NotNull(del);
                ref int value = ref del();
                Assert.Equal(777, value);
                value = 888;
                Assert.Equal(888, target.InstanceField);
            }
        }

        public sealed class CreateDelegateGenericWithoutTarget
        {
            [Fact]
            public void NullField_ThrowsArgumentNullException()
            {
                FieldInfo field = null!;

                Assert.Throws<ArgumentNullException>("field", () => FieldInfoExtensions.CreateDelegate<StaticFieldDelegate>(field));
            }

            [Fact]
            public void StaticField_CreatesTypedDelegate()
            {
                var field = typeof(TestClassForStaticFieldCreatesTypedDelegate).GetField(nameof(TestClassForStaticFieldCreatesTypedDelegate.StaticField))!;

                var del = field.CreateDelegate<StaticFieldDelegate>();

                Assert.NotNull(del);
                Assert.IsType<StaticFieldDelegate>(del);
                ref int value = ref del();
                Assert.Equal(100, value);
                value = 350;
                Assert.Equal(350, TestClassForStaticFieldCreatesTypedDelegate.StaticField);
            }

            [Fact]
            public void InstanceField_CreatesTypedOpenInstanceDelegate()
            {
                var field = typeof(TestClass).GetField(nameof(TestClass.InstanceField))!;
                var instance = new TestClass { InstanceField = 99 };

                var del = field.CreateDelegate<OpenInstanceFieldDelegate>();

                Assert.NotNull(del);
                Assert.IsType<OpenInstanceFieldDelegate>(del);
                ref int value = ref del(instance);
                Assert.Equal(99, value);
                value = 198;
                Assert.Equal(198, instance.InstanceField);
            }
        }

        public sealed class CreateDelegateGenericWithTarget
        {
            [Fact]
            public void NullField_ThrowsArgumentNullException()
            {
                FieldInfo field = null!;
                object? target = new TestClass();

                Assert.Throws<ArgumentNullException>("field", () => FieldInfoExtensions.CreateDelegate<StaticFieldDelegate>(field, target));
            }

            [Fact]
            public void StaticField_CreatesTypedDelegate()
            {
                var field = typeof(TestClassForStaticFieldCreatesTypedDelegateWithTarget).GetField(nameof(TestClassForStaticFieldCreatesTypedDelegateWithTarget.StaticField))!;
                var target = new TestClass();

                var del = field.CreateDelegate<StaticFieldDelegate>(target);

                Assert.NotNull(del);
                Assert.IsType<StaticFieldDelegate>(del);
                ref int value = ref del();
                Assert.Equal(100, value);
                value = 450;
                Assert.Equal(450, TestClassForStaticFieldCreatesTypedDelegateWithTarget.StaticField);
            }

            [Fact]
            public void InstanceField_NullTarget_CreatesTypedOpenInstanceDelegate()
            {
                var field = typeof(TestClass).GetField(nameof(TestClass.InstanceField))!;
                object? target = null;
                var instance = new TestClass { InstanceField = 66 };

                var del = field.CreateDelegate<OpenInstanceFieldDelegate>(target);

                Assert.NotNull(del);
                Assert.IsType<OpenInstanceFieldDelegate>(del);
                ref int value = ref del(instance);
                Assert.Equal(66, value);
                value = 132;
                Assert.Equal(132, instance.InstanceField);
            }

            [Fact]
            public void InstanceField_NonNullTarget_CreatesTypedClosedInstanceDelegate()
            {
                var field = typeof(TestClass).GetField(nameof(TestClass.InstanceField))!;
                var target = new TestClass { InstanceField = 333 };

                var del = field.CreateDelegate<StaticFieldDelegate>(target);

                Assert.NotNull(del);
                Assert.IsType<StaticFieldDelegate>(del);
                ref int value = ref del();
                Assert.Equal(333, value);
                value = 666;
                Assert.Equal(666, target.InstanceField);
            }
        }
    }
#endif

    // Tests for CreateGetDelegate methods (non-ref getter, all frameworks)
    public sealed class CreateGetDelegate
    {
        // Test class with instance fields (shared across tests)
        private class TestClass
        {
            public int InstanceField = 42;
        }

        // Separate test classes for static field tests to avoid parallel test interference
        private class TestClassForStaticFieldCreatesGetterDelegate
        {
            public static int StaticField = 100;
        }

        private class TestClassForStaticFieldCreatesGetterDelegateWithTarget
        {
            public static int StaticField = 100;
        }

        private class TestClassForStaticFieldCreatesTypedGetterDelegate
        {
            public static int StaticField = 100;
        }

        private class TestClassForStaticFieldCreatesTypedGetterDelegateWithTarget
        {
            public static int StaticField = 100;
        }

        public sealed class CreateGetDelegateWithDelegateType
        {
            [Fact]
            public void NullField_ThrowsArgumentNullException()
            {
                FieldInfo field = null!;
                Type delegateType = typeof(Func<int>);

                Assert.Throws<ArgumentNullException>("field", () => FieldInfoExtensions.CreateGetDelegate(field, delegateType));
            }

            [Fact]
            public void NullDelegateType_ThrowsArgumentNullException()
            {
                var field = typeof(TestClass).GetField(nameof(TestClass.InstanceField))!;
                Type delegateType = null!;

                Assert.Throws<ArgumentNullException>("delegateType", () => field.CreateGetDelegate(delegateType));
            }

            [Fact]
            public void StaticField_CreatesGetterDelegate()
            {
                var field = typeof(TestClassForStaticFieldCreatesGetterDelegate).GetField(nameof(TestClassForStaticFieldCreatesGetterDelegate.StaticField))!;

                var getter = (Func<int>)field.CreateGetDelegate(typeof(Func<int>));

                Assert.NotNull(getter);
                Assert.Equal(100, getter());
                TestClassForStaticFieldCreatesGetterDelegate.StaticField = 200;
                Assert.Equal(200, getter());
            }

            [Fact]
            public void InstanceField_CreatesOpenGetterDelegate()
            {
                var field = typeof(TestClass).GetField(nameof(TestClass.InstanceField))!;
                var instance = new TestClass { InstanceField = 42 };

                var getter = (Func<TestClass, int>)field.CreateGetDelegate(typeof(Func<TestClass, int>));

                Assert.NotNull(getter);
                Assert.Equal(42, getter(instance));
                instance.InstanceField = 84;
                Assert.Equal(84, getter(instance));
            }
        }

        public sealed class CreateGetDelegateWithDelegateTypeAndTarget
        {
            [Fact]
            public void NullField_ThrowsArgumentNullException()
            {
                FieldInfo field = null!;
                Type delegateType = typeof(Func<int>);
                object? target = new TestClass();

                Assert.Throws<ArgumentNullException>("field", () => FieldInfoExtensions.CreateGetDelegate(field, delegateType, target));
            }

            [Fact]
            public void NullDelegateType_ThrowsArgumentNullException()
            {
                var field = typeof(TestClass).GetField(nameof(TestClass.InstanceField))!;
                Type delegateType = null!;
                object? target = new TestClass();

                Assert.Throws<ArgumentNullException>("delegateType", () => field.CreateGetDelegate(delegateType, target));
            }

            [Fact]
            public void StaticField_CreatesGetterDelegate()
            {
                var field = typeof(TestClassForStaticFieldCreatesGetterDelegateWithTarget).GetField(nameof(TestClassForStaticFieldCreatesGetterDelegateWithTarget.StaticField))!;
                object? target = new TestClass();

                var getter = (Func<int>)field.CreateGetDelegate(typeof(Func<int>), target);

                Assert.NotNull(getter);
                Assert.Equal(100, getter());
            }

            [Fact]
            public void InstanceField_NullTarget_CreatesOpenGetterDelegate()
            {
                var field = typeof(TestClass).GetField(nameof(TestClass.InstanceField))!;
                object? target = null;
                var instance = new TestClass { InstanceField = 55 };

                var getter = (Func<TestClass, int>)field.CreateGetDelegate(typeof(Func<TestClass, int>), target);

                Assert.NotNull(getter);
                Assert.Equal(55, getter(instance));
            }

            [Fact]
            public void InstanceField_NonNullTarget_CreatesClosedGetterDelegate()
            {
                var field = typeof(TestClass).GetField(nameof(TestClass.InstanceField))!;
                var target = new TestClass { InstanceField = 777 };

                var getter = (Func<int>)field.CreateGetDelegate(typeof(Func<int>), target);

                Assert.NotNull(getter);
                Assert.Equal(777, getter());
                target.InstanceField = 888;
                Assert.Equal(888, getter());
            }
        }

        public sealed class CreateGetDelegateGenericWithoutTarget
        {
            [Fact]
            public void NullField_ThrowsArgumentNullException()
            {
                FieldInfo field = null!;

                Assert.Throws<ArgumentNullException>("field", () => FieldInfoExtensions.CreateGetDelegate<Func<int>>(field));
            }

            [Fact]
            public void StaticField_CreatesTypedGetterDelegate()
            {
                var field = typeof(TestClassForStaticFieldCreatesTypedGetterDelegate).GetField(nameof(TestClassForStaticFieldCreatesTypedGetterDelegate.StaticField))!;

                var getter = field.CreateGetDelegate<Func<int>>();

                Assert.NotNull(getter);
                Assert.IsType<Func<int>>(getter);
                Assert.Equal(100, getter());
            }

            [Fact]
            public void InstanceField_CreatesTypedOpenGetterDelegate()
            {
                var field = typeof(TestClass).GetField(nameof(TestClass.InstanceField))!;
                var instance = new TestClass { InstanceField = 99 };

                var getter = field.CreateGetDelegate<Func<TestClass, int>>();

                Assert.NotNull(getter);
                Assert.IsType<Func<TestClass, int>>(getter);
                Assert.Equal(99, getter(instance));
            }
        }

        public sealed class CreateGetDelegateGenericWithTarget
        {
            [Fact]
            public void NullField_ThrowsArgumentNullException()
            {
                FieldInfo field = null!;
                object? target = new TestClass();

                Assert.Throws<ArgumentNullException>("field", () => FieldInfoExtensions.CreateGetDelegate<Func<int>>(field, target));
            }

            [Fact]
            public void StaticField_CreatesTypedGetterDelegate()
            {
                var field = typeof(TestClassForStaticFieldCreatesTypedGetterDelegateWithTarget).GetField(nameof(TestClassForStaticFieldCreatesTypedGetterDelegateWithTarget.StaticField))!;
                var target = new TestClass();

                var getter = field.CreateGetDelegate<Func<int>>(target);

                Assert.NotNull(getter);
                Assert.IsType<Func<int>>(getter);
                Assert.Equal(100, getter());
            }

            [Fact]
            public void InstanceField_NullTarget_CreatesTypedOpenGetterDelegate()
            {
                var field = typeof(TestClass).GetField(nameof(TestClass.InstanceField))!;
                object? target = null;
                var instance = new TestClass { InstanceField = 66 };

                var getter = field.CreateGetDelegate<Func<TestClass, int>>(target);

                Assert.NotNull(getter);
                Assert.IsType<Func<TestClass, int>>(getter);
                Assert.Equal(66, getter(instance));
            }

            [Fact]
            public void InstanceField_NonNullTarget_CreatesTypedClosedGetterDelegate()
            {
                var field = typeof(TestClass).GetField(nameof(TestClass.InstanceField))!;
                var target = new TestClass { InstanceField = 333 };

                var getter = field.CreateGetDelegate<Func<int>>(target);

                Assert.NotNull(getter);
                Assert.IsType<Func<int>>(getter);
                Assert.Equal(333, getter());
            }
        }
    }

    // Tests for CreateSetDelegate methods (setter, all frameworks)
    public sealed class CreateSetDelegate
    {
        // Test class with instance fields (shared across tests)
        private class TestClass
        {
            public int InstanceField = 42;
        }

        // Separate test classes for static field tests to avoid parallel test interference
        private class TestClassForStaticFieldCreatesSetterDelegate
        {
            public static int StaticField = 100;
        }

        private class TestClassForStaticFieldCreatesSetterDelegateWithTarget
        {
            public static int StaticField = 100;
        }

        private class TestClassForStaticFieldCreatesTypedSetterDelegate
        {
            public static int StaticField = 100;
        }

        private class TestClassForStaticFieldCreatesTypedSetterDelegateWithTarget
        {
            public static int StaticField = 100;
        }

        public sealed class CreateSetDelegateWithDelegateType
        {
            [Fact]
            public void NullField_ThrowsArgumentNullException()
            {
                FieldInfo field = null!;
                Type delegateType = typeof(Action<int>);

                Assert.Throws<ArgumentNullException>("field", () => FieldInfoExtensions.CreateSetDelegate(field, delegateType));
            }

            [Fact]
            public void NullDelegateType_ThrowsArgumentNullException()
            {
                var field = typeof(TestClass).GetField(nameof(TestClass.InstanceField))!;
                Type delegateType = null!;

                Assert.Throws<ArgumentNullException>("delegateType", () => field.CreateSetDelegate(delegateType));
            }

            [Fact]
            public void StaticField_CreatesSetterDelegate()
            {
                var field = typeof(TestClassForStaticFieldCreatesSetterDelegate).GetField(nameof(TestClassForStaticFieldCreatesSetterDelegate.StaticField))!;

                var setter = (Action<int>)field.CreateSetDelegate(typeof(Action<int>));

                Assert.NotNull(setter);
                setter(100);
                Assert.Equal(100, TestClassForStaticFieldCreatesSetterDelegate.StaticField);
                setter(200);
                Assert.Equal(200, TestClassForStaticFieldCreatesSetterDelegate.StaticField);
            }

            [Fact]
            public void InstanceField_CreatesOpenSetterDelegate()
            {
                var field = typeof(TestClass).GetField(nameof(TestClass.InstanceField))!;
                var instance = new TestClass { InstanceField = 0 };

                var setter = (Action<TestClass, int>)field.CreateSetDelegate(typeof(Action<TestClass, int>));

                Assert.NotNull(setter);
                setter(instance, 42);
                Assert.Equal(42, instance.InstanceField);
                setter(instance, 84);
                Assert.Equal(84, instance.InstanceField);
            }
        }

        public sealed class CreateSetDelegateWithDelegateTypeAndTarget
        {
            [Fact]
            public void NullField_ThrowsArgumentNullException()
            {
                FieldInfo field = null!;
                Type delegateType = typeof(Action<int>);
                object? target = new TestClass();

                Assert.Throws<ArgumentNullException>("field", () => FieldInfoExtensions.CreateSetDelegate(field, delegateType, target));
            }

            [Fact]
            public void NullDelegateType_ThrowsArgumentNullException()
            {
                var field = typeof(TestClass).GetField(nameof(TestClass.InstanceField))!;
                Type delegateType = null!;
                object? target = new TestClass();

                Assert.Throws<ArgumentNullException>("delegateType", () => field.CreateSetDelegate(delegateType, target));
            }

            [Fact]
            public void StaticField_CreatesSetterDelegate()
            {
                var field = typeof(TestClassForStaticFieldCreatesSetterDelegateWithTarget).GetField(nameof(TestClassForStaticFieldCreatesSetterDelegateWithTarget.StaticField))!;
                object? target = new TestClass();

                var setter = (Action<int>)field.CreateSetDelegate(typeof(Action<int>), target);

                Assert.NotNull(setter);
                setter(100);
                Assert.Equal(100, TestClassForStaticFieldCreatesSetterDelegateWithTarget.StaticField);
            }

            [Fact]
            public void InstanceField_NullTarget_CreatesOpenSetterDelegate()
            {
                var field = typeof(TestClass).GetField(nameof(TestClass.InstanceField))!;
                object? target = null;
                var instance = new TestClass { InstanceField = 0 };

                var setter = (Action<TestClass, int>)field.CreateSetDelegate(typeof(Action<TestClass, int>), target);

                Assert.NotNull(setter);
                setter(instance, 55);
                Assert.Equal(55, instance.InstanceField);
            }

            [Fact]
            public void InstanceField_NonNullTarget_CreatesClosedSetterDelegate()
            {
                var field = typeof(TestClass).GetField(nameof(TestClass.InstanceField))!;
                var target = new TestClass { InstanceField = 0 };

                var setter = (Action<int>)field.CreateSetDelegate(typeof(Action<int>), target);

                Assert.NotNull(setter);
                setter(777);
                Assert.Equal(777, target.InstanceField);
                setter(888);
                Assert.Equal(888, target.InstanceField);
            }
        }

        public sealed class CreateSetDelegateGenericWithoutTarget
        {
            [Fact]
            public void NullField_ThrowsArgumentNullException()
            {
                FieldInfo field = null!;

                Assert.Throws<ArgumentNullException>("field", () => FieldInfoExtensions.CreateSetDelegate<Action<int>>(field));
            }

            [Fact]
            public void StaticField_CreatesTypedSetterDelegate()
            {
                var field = typeof(TestClassForStaticFieldCreatesTypedSetterDelegate).GetField(nameof(TestClassForStaticFieldCreatesTypedSetterDelegate.StaticField))!;

                var setter = field.CreateSetDelegate<Action<int>>();

                Assert.NotNull(setter);
                Assert.IsType<Action<int>>(setter);
                setter(100);
                Assert.Equal(100, TestClassForStaticFieldCreatesTypedSetterDelegate.StaticField);
            }

            [Fact]
            public void InstanceField_CreatesTypedOpenSetterDelegate()
            {
                var field = typeof(TestClass).GetField(nameof(TestClass.InstanceField))!;
                var instance = new TestClass { InstanceField = 0 };

                var setter = field.CreateSetDelegate<Action<TestClass, int>>();

                Assert.NotNull(setter);
                Assert.IsType<Action<TestClass, int>>(setter);
                setter(instance, 99);
                Assert.Equal(99, instance.InstanceField);
            }
        }

        public sealed class CreateSetDelegateGenericWithTarget
        {
            [Fact]
            public void NullField_ThrowsArgumentNullException()
            {
                FieldInfo field = null!;
                object? target = new TestClass();

                Assert.Throws<ArgumentNullException>("field", () => FieldInfoExtensions.CreateSetDelegate<Action<int>>(field, target));
            }

            [Fact]
            public void StaticField_CreatesTypedSetterDelegate()
            {
                var field = typeof(TestClassForStaticFieldCreatesTypedSetterDelegateWithTarget).GetField(nameof(TestClassForStaticFieldCreatesTypedSetterDelegateWithTarget.StaticField))!;
                var target = new TestClass();

                var setter = field.CreateSetDelegate<Action<int>>(target);

                Assert.NotNull(setter);
                Assert.IsType<Action<int>>(setter);
                setter(100);
                Assert.Equal(100, TestClassForStaticFieldCreatesTypedSetterDelegateWithTarget.StaticField);
            }

            [Fact]
            public void InstanceField_NullTarget_CreatesTypedOpenSetterDelegate()
            {
                var field = typeof(TestClass).GetField(nameof(TestClass.InstanceField))!;
                object? target = null;
                var instance = new TestClass { InstanceField = 0 };

                var setter = field.CreateSetDelegate<Action<TestClass, int>>(target);

                Assert.NotNull(setter);
                Assert.IsType<Action<TestClass, int>>(setter);
                setter(instance, 66);
                Assert.Equal(66, instance.InstanceField);
            }

            [Fact]
            public void InstanceField_NonNullTarget_CreatesTypedClosedSetterDelegate()
            {
                var field = typeof(TestClass).GetField(nameof(TestClass.InstanceField))!;
                var target = new TestClass { InstanceField = 0 };

                var setter = field.CreateSetDelegate<Action<int>>(target);

                Assert.NotNull(setter);
                Assert.IsType<Action<int>>(setter);
                setter(333);
                Assert.Equal(333, target.InstanceField);
            }
        }
    }
}
