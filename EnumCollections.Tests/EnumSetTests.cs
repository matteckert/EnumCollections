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
            public enum E { }

            [Test]
            public void WhenCreated_ShouldBeAnEnumSet()
            {
                Assert.IsInstanceOf<EnumSet<E>>(EnumSet.Of<E>());
            }

            [Test]
            public void WhenCompared_ThenShouldBeEqual()
            {
                CollectionAssert.AreEqual(EnumSet.Of<E>(), EnumSet.Of<E>());
            }

            [Test]
            public void WhenComparedToNullSet_ThenShouldThrow()
            {
                Assert.Throws<ArgumentNullException>(() => EnumSet.Of<E>().SetEquals(null));
            }
        }

        [TestFixture]
        public class GivenAnEnumWithElements
        {
            public enum E { A, B, C }

            [Test]
            public void WhenCreated_ThenShouldHaveNoElements()
            {
                CollectionAssert.IsEmpty(EnumSet.Of<E>());
            }

            [Test]
            public void WhenCreated_ThenCountShouldBeZero()
            {
                Assert.IsTrue(EnumSet.Of<E>().Count == 0);
            }

            [Test]
            public void WhenCreated_ThenShouldNotContainElement()
            {
                Assert.IsFalse(EnumSet.Of<E>().Contains(E.B));
            }

            [Test]
            public void WhenOneElement_ThenCountShouldBeOne()
            {
                Assert.IsTrue(EnumSet.Of(E.B).Count == 1);
            }

            [Test]
            public void WhenOneElement_ThenShouldContainElement()
            {
                Assert.IsTrue(EnumSet.Of(E.B).Contains(E.B));
            }

            [Test]
            public void WhenMultipleElements_ThenCountShouldBeNumberOfElements()
            {
                Assert.IsTrue(EnumSet.Of(E.B, E.C).Count == 2);
            }

            [Test]
            public void WhenMultipleElements_ThenShouldContainAllElements()
            {
                Assert.IsTrue(EnumSet.Of(E.B, E.C).Contains(E.B));
                Assert.IsTrue(EnumSet.Of(E.B, E.C).Contains(E.C));
            }

            [Test]
            public void WhenNotAllElements_ThenShouldNotContainElementsNotAdded()
            {
                Assert.IsFalse(EnumSet.Of(E.B, E.C).Contains(E.A));
            }
        }

        [TestFixture]
        public class GivenTwoEnums
        {
            public enum E { A, B, C }

            [Test]
            public void WhenSameElements_ThenEqual()
            {
                Assert.IsTrue(EnumSet.Of(E.A).SetEquals(EnumSet.Of(E.A)));
            }

            [Test]
            public void WhenDifferentElements_ThenNotEqual()
            {
                Assert.IsFalse(EnumSet.Of(E.A).SetEquals(EnumSet.Of(E.B)));
            }

            [Test]
            public void WhenSameAndDifferentElements_ThenSymmetricExceptWithGivesDifferences()
            {
                var a = EnumSet.Of(E.A, E.B);
                a.SymmetricExceptWith(EnumSet.Of(E.B, E.C));
                Assert.IsTrue(a.SetEquals(EnumSet.Of(E.A, E.C)));
            }
        }
    }
}
