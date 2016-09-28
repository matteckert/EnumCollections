using System;
using NUnit.Framework;

namespace EnumCollections.Tests
{
    [TestFixture]
    public class EnumSetTests
    {
        [TestFixture]
        public class GivenAnEmptyEnum
        {
            private readonly EnumSet<E> _enumSet;

            public enum E
            {
            }

            public GivenAnEmptyEnum()
            {
                _enumSet = EnumSet.Of<E>();
            }

            [Test]
            public void WhenCreated_ShouldBeAnEnumSet()
            {
                Assert.IsInstanceOf<EnumSet<E>>(_enumSet);
            }

            [Test]
            public void WhenCompared_ThenShouldBeEqual()
            {
                CollectionAssert.AreEqual(_enumSet, EnumSet.Of<E>());
            }

            [Test]
            public void WhenComparedToNullSet_ThenShouldThrow()
            {
                Assert.Throws<ArgumentNullException>(() => _enumSet.SetEquals(null));
            }
        }

        [TestFixture]
        public class GivenAnEnumWithElements
        {
            private EnumSet<E> _enumSet;

            public enum E
            {
                A,
                B,
                C
            }

            private void AssertContains(E element)
            {
                Assert.IsTrue(_enumSet.Contains(element));
            }

            [SetUp]
            public void Init()
            {
                _enumSet = EnumSet.Of<E>();
            }

            [Test]
            public void WhenCreated_ThenShouldHaveNoElements()
            {
                CollectionAssert.IsEmpty(_enumSet);
            }

            [Test]
            public void WhenCreated_ThenCountShouldBeZero()
            {
                Assert.IsTrue(_enumSet.Count == 0);
            }

            [Test]
            public void WhenCreated_ThenShouldNotContainElement()
            {
                Assert.IsFalse(_enumSet.Contains(E.B));
                CollectionAssert.DoesNotContain(_enumSet, E.B);
            }

            [Test]
            public void WhenOneElement_ThenCountShouldBeOne()
            {
                _enumSet.Add(E.B);
                Assert.IsTrue(_enumSet.Count == 1);
            }

            [Test]
            public void WhenOneElement_ThenShouldContainElement()
            {
                _enumSet.Add(E.B);
                AssertContains(E.B);
            }

            [Test]
            public void WhenMultipleElements_ThenCountShouldBeNumberOfElements()
            {
                _enumSet.Add(E.B);
                _enumSet.Add(E.C);
                Assert.IsTrue(_enumSet.Count == 2);
            }

            [Test]
            public void WhenMultipleElements_ThenShouldContainAllElements()
            {
                _enumSet.Add(E.B);
                _enumSet.Add(E.C);
                AssertContains(E.B);
                AssertContains(E.C);
            }

            [Test]
            public void WhenNotAllElements_ThenShouldNotContainElementsNotAdded()
            {
                _enumSet.Add(E.B);
                _enumSet.Add(E.C);
                CollectionAssert.DoesNotContain(_enumSet, E.A);
            }
        }

        [TestFixture]
        public class GivenTwoEnums
        {
            private EnumSet<E> _a;
            private EnumSet<E> _b;

            public enum E
            {
                A,
                B,
                C
            }

            public class AClass {}


            [SetUp]
            public void Init()
            {
                _a = EnumSet.Of<E>();
                _b = EnumSet.Of<E>();
            }

            [Test]
            public void WhenEmpty_ThenEqual()
            {
                Assert.IsTrue(_a.SetEquals(_b));
            }

            [Test]
            public void WhenSameElements_ThenEqual()
            {
                _a.Add(E.A);
                _b.Add(E.A);
                Assert.IsTrue(_a.SetEquals(_b));
            }

            [Test]
            public void WhenDifferentElements_ThenNotEqual()
            {
                _a.Add(E.A);
                _b.Add(E.B);
                Assert.IsFalse(_a.SetEquals(_b));
            }

            [Test]
            public void WhenSameAndDifferentElements_ThenSymmetricExceptWithGivesDifferences()
            {
                var expected = EnumSet.Of(E.A, E.C);

                _a.Add(E.A);
                _a.Add(E.B);
                _b.Add(E.B);
                _b.Add(E.C);
                _a.SymmetricExceptWith(_b);
                Assert.IsTrue(_a.SetEquals(expected));
            }
        }
    }
}
