using NUnit.Framework;
using static EnumCollections.Tests.E;

namespace EnumCollections.Tests
{
    public enum E { A, B, C }

    [TestFixture]
    public class EnumSetTests
    {
        [Test]
        public void WhenCreated_ThenAnEnumSet()
        {
            Assert.That(EnumSet.Of<E>(), Is.InstanceOf(typeof(EnumSet<E>)));
        }

        [Test]
        public void WhenCreated_ThenEqualToAnotherNewlyCreatedEnumSet()
        {
            Assert.That(EnumSet.Of<E>().SetEquals(EnumSet.Of<E>()));
        }

        [Test]
        public void WhenSameElements_ThenEqual()
        {
            Assert.That(EnumSet.Of(A).SetEquals(EnumSet.Of(A)));
        }

        [Test]
        public void WhenDifferentElements_ThenNotEqual()
        {
            Assert.That(EnumSet.Of(A).SetEquals(EnumSet.Of(B)), Is.False);
        }

        [Test]
        public void WhenComparedToNullSet_ThenThrows()
        {
            Assert.That(() => EnumSet.Of<E>().SetEquals(null), Throws.ArgumentNullException);
        }

        [TestCase(ExpectedResult = 0)]
        [TestCase(A, ExpectedResult = 1)]
        [TestCase(B, ExpectedResult = 1)]
        [TestCase(A, B, ExpectedResult = 2)]
        [TestCase(B, C, ExpectedResult = 2)]
        [TestCase(A, B, C, ExpectedResult = 3)]
        public int WhenCreated_ThenCountIsAccurate(params E[] elementsToAdd)
        {
            return EnumSet.Of(elementsToAdd).Count;
        }

        [TestCase(ExpectedResult = 0)]
        [TestCase(A, ExpectedResult = 1)]
        [TestCase(B, ExpectedResult = 1)]
        [TestCase(A, B, ExpectedResult = 2)]
        [TestCase(B, C, ExpectedResult = 2)]
        [TestCase(A, B, C, ExpectedResult = 3)]
        public int WhenElementsAdded_ThenCountIsUpdated(params E[] elementsToAdd)
        {
            var set = EnumSet.Of<E>();
            foreach (var e in elementsToAdd)
                set.Add(e);
            return set.Count;
        }

        [TestCase(ExpectedResult = 3)]
        [TestCase(A, ExpectedResult = 2)]
        [TestCase(B, ExpectedResult = 2)]
        [TestCase(A, B, ExpectedResult = 1)]
        [TestCase(B, C, ExpectedResult = 1)]
        [TestCase(A, B, C, ExpectedResult = 0)]
        public int WhenElementsRemoved_ThenCountIsUpdated(params E[] elementsToAdd)
        {
            var set = EnumSet.Of(A, B, C);
            foreach (var e in elementsToAdd)
                set.Remove(e);
            return set.Count;
        }

        [Test]
        public void WhenElementsAddedAndRemoved_ThenCountIsUpdated()
        {
            var set = EnumSet.Of(A);
            set.Add(B);
            set.Add(C);
            set.Remove(B);
            Assert.That(set.Count, Is.EqualTo(2));
        }

        [Test]
        public void WhenCreated_ThenHasNoElements()
        {
            Assert.That(EnumSet.Of<E>(), Is.Empty);
        }

        [Test]
        public void WhenCreated_ThenDoesNotContainElement()
        {
            Assert.That(EnumSet.Of<E>().Contains(B), Is.False);
        }

        [Test]
        public void WhenOneElement_ThenContainsElement()
        {
            Assert.That(EnumSet.Of(B).Contains(B));
        }

        [Test]
        public void WhenMultipleElements_ThenContainsAllElements()
        {
            var set = EnumSet.Of(B, C);
            Assert.That(set.Contains(B), Is.True);
            Assert.That(set.Contains(C), Is.True);
        }

        [Test]
        public void WhenNotAllElements_ThenDoesNotContainElementsNotAdded()
        {
            Assert.That(EnumSet.Of(B, C).Contains(A), Is.False);
        }

        [Test]
        public void WhenSameAndDifferentElements_ThenSymmetricExceptWithKeepsOnlyDifferentElements()
        {
            var a = EnumSet.Of(A, B);
            var b = EnumSet.Of(B, C);
            a.SymmetricExceptWith(b);
            Assert.That(a.SetEquals(EnumSet.Of(A, C)));
        }

        [Test]
        public void WhenSameAndDifferentElements_ThenExceptWithRemovesSameElements()
        {
            var a = EnumSet.Of(A, B);
            var b = EnumSet.Of(B);
            a.ExceptWith(b);
            Assert.That(a.SetEquals(EnumSet.Of(A)));
        }

        [Test]
        public void WhenSameAndDifferentElements_ThenIntersectWithKeepsOnlySameElements()
        {
            var a = EnumSet.Of(A, B);
            var b = EnumSet.Of(B, C);
            a.IntersectWith(b);
            Assert.That(a.SetEquals(EnumSet.Of(B)));
        }

        [TestCase(ExpectedResult = true)]
        [TestCase(A, ExpectedResult = true)]
        [TestCase(B, ExpectedResult = true)]
        [TestCase(C, ExpectedResult = false)]
        [TestCase(A, B, ExpectedResult = false)]
        [TestCase(A, C, ExpectedResult = false)]
        [TestCase(B, C, ExpectedResult = false)]
        [TestCase(A, B, C, ExpectedResult = false)]
        public bool IsProperSubsetOf_AB(params E[] elements)
        {
            return EnumSet.Of(elements).IsProperSubsetOf(EnumSet.Of(A, B));
        }

        [TestCase(ExpectedResult = true)]
        [TestCase(A, ExpectedResult = true)]
        [TestCase(B, ExpectedResult = true)]
        [TestCase(C, ExpectedResult = false)]
        [TestCase(A, B, ExpectedResult = true)]
        [TestCase(A, C, ExpectedResult = false)]
        [TestCase(B, C, ExpectedResult = false)]
        [TestCase(A, B, C, ExpectedResult = false)]
        public bool IsSubsetOf_AB(params E[] elements)
        {
            return EnumSet.Of(elements).IsSubsetOf(EnumSet.Of(A, B));
        }

        [TestCase(ExpectedResult = false)]
        [TestCase(A, ExpectedResult = false)]
        [TestCase(B, ExpectedResult = false)]
        [TestCase(C, ExpectedResult = false)]
        [TestCase(A, B, ExpectedResult = false)]
        [TestCase(A, C, ExpectedResult = false)]
        [TestCase(B, C, ExpectedResult = false)]
        [TestCase(A, B, C, ExpectedResult = true)]
        public bool IsProperSupersetOf_AB(params E[] elements)
        {
            return EnumSet.Of(elements).IsProperSupersetOf(EnumSet.Of(A, B));
        }

        [TestCase(ExpectedResult = false)]
        [TestCase(A, ExpectedResult = false)]
        [TestCase(B, ExpectedResult = false)]
        [TestCase(C, ExpectedResult = false)]
        [TestCase(A, B, ExpectedResult = true)]
        [TestCase(A, C, ExpectedResult = false)]
        [TestCase(B, C, ExpectedResult = false)]
        [TestCase(A, B, C, ExpectedResult = true)]
        public bool IsSupersetOf_AB(params E[] elements)
        {
            return EnumSet.Of(elements).IsSupersetOf(EnumSet.Of(A, B));
        }
    }
}
