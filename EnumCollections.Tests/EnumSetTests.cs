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
                _enumSet = EnumSet.Of<EmptyEnum>();
            }

            [Test]
            public void WhenCreated_ShouldBeAnEnumSet()
            {
                Assert.IsInstanceOf<EnumSet<EmptyEnum>>(_enumSet);
            }

            [Test]
            public void WhenCompared_ThenShouldBeEqual()
            {
                CollectionAssert.AreEqual(_enumSet, EnumSet.Of<EmptyEnum>());
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

            private void AssertContains(EnumWithElements element)
            {
                Assert.IsTrue(_enumSet.Contains(element));
            }

            [SetUp]
            public void Init()
            {
                _enumSet = EnumSet.Of<EnumWithElements>();
            }

            [Test]
            public void WhenCreated_ThenShouldHaveNoElements()
            {
                CollectionAssert.IsEmpty(_enumSet);
            }

            [Test]
            public void WhenElementHasNotBeenAdded_ThenShouldNotContainElement()
            {
                Assert.IsFalse(_enumSet.Contains(EnumWithElements.B));
                CollectionAssert.DoesNotContain(_enumSet, EnumWithElements.B);
            }

            [Test]
            public void WhenElementAdded_ThenShouldContainElement()
            {
                _enumSet.Add(EnumWithElements.B);
                AssertContains(EnumWithElements.B);
            }

            [Test]
            public void WhenTwoElementsAdded_ThenShouldContainBothElements()
            {
                _enumSet.Add(EnumWithElements.B);
                _enumSet.Add(EnumWithElements.C);
                AssertContains(EnumWithElements.B);
                AssertContains(EnumWithElements.C);
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

            public class AClass {}


            [SetUp]
            public void Init()
            {
                _a = EnumSet.Of<EnumWithElements>();
                _b = EnumSet.Of<EnumWithElements>();
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

            [Test]
            public void WhenSetsHaveBothSameAndDifferentElements_ThenSymmetricExceptWithGivesOnlyDifferences()
            {
                var expected = EnumSet.Of(EnumWithElements.A, EnumWithElements.C);

                _a.Add(EnumWithElements.A);
                _a.Add(EnumWithElements.B);
                _b.Add(EnumWithElements.B);
                _b.Add(EnumWithElements.C);
                _a.SymmetricExceptWith(_b);
                Assert.IsTrue(_a.SetEquals(expected));
            }
        }
    }
}
