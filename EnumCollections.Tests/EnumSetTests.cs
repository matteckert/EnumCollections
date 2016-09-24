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
        }

        [TestFixture]
        public class GivenASmallEnum
        {
            private EnumSet<SmallEnum> _enumSet;

            public enum SmallEnum
            {
                A, B, C
            }

            [SetUp]
            public void Init()
            {
                _enumSet = new EnumSet<SmallEnum>();
            }

            [Test]
            public void WhenCreated_ThenShouldHaveNoElements()
            {
                CollectionAssert.IsEmpty(_enumSet);
            }

            [Test]
            public void WhenElementHasNotBeenAddedThenShouldNotContainElement()
            {
                CollectionAssert.DoesNotContain(_enumSet, SmallEnum.B);
            }

            [Test]
            public void WhenElementAdded_ThenShouldContainElement()
            {
                _enumSet.Add(SmallEnum.B);
                CollectionAssert.Contains(_enumSet, SmallEnum.B);
            }

            [Test]
            public void WhenTwoElementsAdded_ThenShouldContainBothElements()
            {
                _enumSet.Add(SmallEnum.B);
                _enumSet.Add(SmallEnum.C);
                CollectionAssert.Contains(_enumSet, SmallEnum.B);
                CollectionAssert.Contains(_enumSet, SmallEnum.C);
            }

            [Test]
            public void WhenTwoElementsAdded_ThenShouldNotContainTheThird()
            {
                _enumSet.Add(SmallEnum.B);
                _enumSet.Add(SmallEnum.C);
                CollectionAssert.DoesNotContain(_enumSet, SmallEnum.A);
            }
        }
    }
}
