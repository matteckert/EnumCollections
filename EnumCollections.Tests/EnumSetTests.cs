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
            private readonly EnumSet<EmptyEnum> _enumSet;

            public enum EmptyEnum
            {
            }

            public GivenAnEmptyEnum()
            {
                _enumSet = new EnumSet<EmptyEnum>();
            }

            [Test]
            public void WhenCreated_ShouldBeAnEnumSet()
            {
                Assert.IsInstanceOf<EnumSet<EmptyEnum>>(_enumSet);
            }

            [Test]
            public void WhenCompared_ThenShouldBeEqual()
            {
                CollectionAssert.AreEqual(_enumSet, new EnumSet<EmptyEnum>());
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
            private EnumSet<EnumWithElements> _enumSet;

            public enum EnumWithElements
            {
                A,
                B,
                C
            }

            [SetUp]
            public void Init()
            {
                _enumSet = new EnumSet<EnumWithElements>();
            }

            [Test]
            public void WhenCreated_ThenShouldHaveNoElements()
            {
                CollectionAssert.IsEmpty(_enumSet);
            }

            [Test]
            public void WhenElementHasNotBeenAddedThenShouldNotContainElement()
            {
                CollectionAssert.DoesNotContain(_enumSet, EnumWithElements.B);
            }

            [Test]
            public void WhenElementAdded_ThenShouldContainElement()
            {
                _enumSet.Add(EnumWithElements.B);
                CollectionAssert.Contains(_enumSet, EnumWithElements.B);
            }

            [Test]
            public void WhenTwoElementsAdded_ThenShouldContainBothElements()
            {
                _enumSet.Add(EnumWithElements.B);
                _enumSet.Add(EnumWithElements.C);
                CollectionAssert.Contains(_enumSet, EnumWithElements.B);
                CollectionAssert.Contains(_enumSet, EnumWithElements.C);
            }

            [Test]
            public void WhenTwoElementsAdded_ThenShouldNotContainTheThird()
            {
                _enumSet.Add(EnumWithElements.B);
                _enumSet.Add(EnumWithElements.C);
                CollectionAssert.DoesNotContain(_enumSet, EnumWithElements.A);
            }
        }

        [TestFixture]
        public class GivenTwoEnums
        {
            private EnumSet<EnumWithElements> _a;
            private EnumSet<EnumWithElements> _b;

            public enum EnumWithElements
            {
                A,
                B,
                C
            }

            [SetUp]
            public void Init()
            {
                _a = new EnumSet<EnumWithElements>();
                _b = new EnumSet<EnumWithElements>();
            }

            [Test]
            public void WhenBothSetsAreEmpty_ThenSetsAreEqual()
            {
                Assert.IsTrue(_a.SetEquals(_b));
            }

            [Test]
            public void WhenSetsHaveSameElements_ThenSetsAreEqual()
            {
                _a.Add(EnumWithElements.A);
                _b.Add(EnumWithElements.A);
                Assert.IsTrue(_a.SetEquals(_b));
            }

            [Test]
            public void WhenSetsHaveDifferentElements_ThenSetsAreNotEqual()
            {
                _a.Add(EnumWithElements.A);
                _b.Add(EnumWithElements.B);
                Assert.IsFalse(_a.SetEquals(_b));
            }
        }
    }
}
