using System;
using System.Collections.Generic;
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
            public void WhenComparedToAnotherEmptyEnumSetOfTheSameType_ThenItShouldBeEqual()
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
            public void WhenCreated_ThenItShouldHaveNoElements()
            {
                CollectionAssert.IsEmpty(_enumSet);
            }

            [Test]
            public void WhenAnElementHasNotBeenAdded_ThenShouldNotContainElement()
            {
                CollectionAssert.DoesNotContain(_enumSet, SmallEnum.B);
            }

            [Test]
            public void WhenAnElementAdded_ThenShouldContainElement()
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
        }
    }
}
