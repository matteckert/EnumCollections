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
            public void WhenCreated_ShouldHaveNoElements()
            {
                Assert.IsInstanceOf<EnumSet<EmptyEnum>>(_enumSet);
            }

            [Test]
            public void WhenCompared_ShouldBeEqual()
            {
                CollectionAssert.AreEqual(_enumSet, new EnumSet<EmptyEnum>());
            }
        }
    }
}
