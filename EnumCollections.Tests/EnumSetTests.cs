using System;
using NUnit.Framework;
using static EnumCollections.Tests.EnumSetTests.GivenAnEnumWithLessThan64Constants.E;
using static EnumCollections.Tests.EnumSetTests.GivenAnEnumWithMoreThan64Constants.E;

namespace EnumCollections.Tests
{
    [TestFixture]
    public class EnumSetTests
    {
        [TestFixture]
        public class GivenAnEnumWithLessThan64Constants
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

            [Test]
            public void WhenSameAndDifferentElements_ThenUnionWithAddsDifferentElements()
            {
                var a = EnumSet.Of(A, B);
                var b = EnumSet.Of(B, C);
                a.UnionWith(b);
                Assert.That(a.SetEquals(EnumSet.Of(A, B, C)));
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

            [TestCase(ExpectedResult = false)]
            [TestCase(A, ExpectedResult = true)]
            [TestCase(B, ExpectedResult = true)]
            [TestCase(C, ExpectedResult = false)]
            [TestCase(A, B, ExpectedResult = true)]
            [TestCase(A, C, ExpectedResult = true)]
            [TestCase(B, C, ExpectedResult = true)]
            [TestCase(A, B, C, ExpectedResult = true)]
            public bool Overlaps_AB(params E[] elements)
            {
                return EnumSet.Of(elements).Overlaps(EnumSet.Of(A, B));
            }

            [Test]
            public void WhenCleared_CountIsZero()
            {
                var a = EnumSet.Of(A, B, C);
                a.Clear();
                Assert.That(a.Count, Is.EqualTo(0));
            }

            [Test]
            public void WhenCopiedToNullArray_ThenThrows()
            {
                Assert.That(() => EnumSet.Of(A, B, C).CopyTo(null, 0), Throws.ArgumentNullException);
            }

            [Test]
            public void WhenCopiedToArrayWithTooFewElements_ThenThrows()
            {
                Assert.That(() => EnumSet.Of(A, B, C).CopyTo(new E[2], 0), Throws.ArgumentException);
            }

            [Test]
            public void WhenCopiedToIndexWithTooFewElementsRemaining_ThenThrows()
            {
                Assert.That(() => EnumSet.Of(A, B, C).CopyTo(new E[3], 1), Throws.ArgumentException);
            }

            [Test]
            public void WhenCopyIndexIsLessThanZero_ThenThrows()
            {
                Assert.That(() => EnumSet.Of(A, B, C).CopyTo(new E[3], -1), Throws.TypeOf<ArgumentOutOfRangeException>());
            }

            [Test]
            public void WhenCopiedToArray_ArrayHasAllElements()
            {
                var a = EnumSet.Of(A, B, C);
                var array = new E[3];
                a.CopyTo(array, 0);
                Assert.That(array, Is.EquivalentTo(new[] {A, B, C}));
            }

            public enum E
            {
                A,
                B,
                C
            }
        }

        [TestFixture]
        public class GivenAnEnumWithMoreThan64Constants
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
                Assert.That(EnumSet.Of(E00).SetEquals(EnumSet.Of(E00)));
            }

            [Test]
            public void WhenDifferentElements_ThenNotEqual()
            {
                Assert.That(EnumSet.Of(E00).SetEquals(EnumSet.Of(E64)), Is.False);
            }

            [Test]
            public void WhenComparedToNullSet_ThenThrows()
            {
                Assert.That(() => EnumSet.Of<E>().SetEquals(null), Throws.ArgumentNullException);
            }

            [TestCase(ExpectedResult = 0)]
            [TestCase(E01, ExpectedResult = 1)]
            [TestCase(E65, ExpectedResult = 1)]
            [TestCase(E03, E66, ExpectedResult = 2)]
            [TestCase(E67, E44, ExpectedResult = 2)]
            [TestCase(E03, E66, E67, ExpectedResult = 3)]
            public int WhenCreated_ThenCountIsAccurate(params E[] elementsToAdd)
            {
                return EnumSet.Of(elementsToAdd).Count;
            }

            [TestCase(ExpectedResult = 0)]
            [TestCase(E01, ExpectedResult = 1)]
            [TestCase(E65, ExpectedResult = 1)]
            [TestCase(E03, E66, ExpectedResult = 2)]
            [TestCase(E67, E44, ExpectedResult = 2)]
            [TestCase(E03, E66, E67, ExpectedResult = 3)]
            public int WhenElementsAdded_ThenCountIsUpdated(params E[] elementsToAdd)
            {
                var set = EnumSet.Of<E>();
                foreach (var e in elementsToAdd)
                    set.Add(e);
                return set.Count;
            }

            [TestCase(ExpectedResult = 3)]
            [TestCase(E03, ExpectedResult = 2)]
            [TestCase(E66, ExpectedResult = 2)]
            [TestCase(E03, E66, ExpectedResult = 1)]
            [TestCase(E03, E67, ExpectedResult = 1)]
            [TestCase(E03, E66, E67, ExpectedResult = 0)]
            public int WhenElementsRemoved_ThenCountIsUpdated(params E[] elementsToAdd)
            {
                var set = EnumSet.Of(E03, E66, E67);
                foreach (var e in elementsToAdd)
                    set.Remove(e);
                return set.Count;
            }

            [Test]
            public void WhenElementsAddedAndRemoved_ThenCountIsUpdated()
            {
                var set = EnumSet.Of(E02);
                set.Add(E68);
                set.Add(E22);
                set.Remove(E68);
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
                Assert.That(EnumSet.Of<E>().Contains(E01), Is.False);
            }

            [Test]
            public void WhenOneElement_ThenContainsElement()
            {
                Assert.That(EnumSet.Of(E68).Contains(E68));
            }

            [Test]
            public void WhenMultipleElements_ThenContainsAllElements()
            {
                var set = EnumSet.Of(E68, E22);
                Assert.That(set.Contains(E68), Is.True);
                Assert.That(set.Contains(E22), Is.True);
            }

            [Test]
            public void WhenNotAllElements_ThenDoesNotContainElementsNotAdded()
            {
                Assert.That(EnumSet.Of(E68, E22).Contains(E02), Is.False);
            }

            [Test]
            public void WhenSameAndDifferentElements_ThenSymmetricExceptWithKeepsOnlyDifferentElements()
            {
                var a = EnumSet.Of(E02, E68);
                var b = EnumSet.Of(E68, E22);
                a.SymmetricExceptWith(b);
                Assert.That(a.SetEquals(EnumSet.Of(E02, E22)));
            }

            [Test]
            public void WhenSameAndDifferentElements_ThenExceptWithRemovesSameElements()
            {
                var a = EnumSet.Of(E02, E68);
                var b = EnumSet.Of(E68);
                a.ExceptWith(b);
                Assert.That(a.SetEquals(EnumSet.Of(E02)));
            }

            [Test]
            public void WhenSameAndDifferentElements_ThenIntersectWithKeepsOnlySameElements()
            {
                var a = EnumSet.Of(E02, E68);
                var b = EnumSet.Of(E68, E22);
                a.IntersectWith(b);
                Assert.That(a.SetEquals(EnumSet.Of(E68)));
            }

            [Test]
            public void WhenSameAndDifferentElements_ThenUnionWithAddsDifferentElements()
            {
                var a = EnumSet.Of(E02, E68);
                var b = EnumSet.Of(E68, E22);
                a.UnionWith(b);
                Assert.That(a.SetEquals(EnumSet.Of(E02, E68, E22)));
            }

            [TestCase(ExpectedResult = true)]
            [TestCase(E02, ExpectedResult = true)]
            [TestCase(E68, ExpectedResult = true)]
            [TestCase(E22, ExpectedResult = false)]
            [TestCase(E02, E68, ExpectedResult = false)]
            [TestCase(E02, E22, ExpectedResult = false)]
            [TestCase(E68, E22, ExpectedResult = false)]
            [TestCase(E02, E68, E22, ExpectedResult = false)]
            public bool IsProperSubsetOf_E02_E68(params E[] elements)
            {
                return EnumSet.Of(elements).IsProperSubsetOf(EnumSet.Of(E02, E68));
            }

            [TestCase(ExpectedResult = true)]
            [TestCase(E02, ExpectedResult = true)]
            [TestCase(E68, ExpectedResult = true)]
            [TestCase(E22, ExpectedResult = false)]
            [TestCase(E02, E68, ExpectedResult = true)]
            [TestCase(E02, E22, ExpectedResult = false)]
            [TestCase(E68, E22, ExpectedResult = false)]
            [TestCase(E02, E68, E22, ExpectedResult = false)]
            public bool IsSubsetOf_E02_E68(params E[] elements)
            {
                return EnumSet.Of(elements).IsSubsetOf(EnumSet.Of(E02, E68));
            }

            [TestCase(ExpectedResult = false)]
            [TestCase(E02, ExpectedResult = false)]
            [TestCase(E68, ExpectedResult = false)]
            [TestCase(E22, ExpectedResult = false)]
            [TestCase(E02, E68, ExpectedResult = false)]
            [TestCase(E02, E22, ExpectedResult = false)]
            [TestCase(E68, E22, ExpectedResult = false)]
            [TestCase(E02, E68, E22, ExpectedResult = true)]
            public bool IsProperSupersetOf_E02_E68(params E[] elements)
            {
                return EnumSet.Of(elements).IsProperSupersetOf(EnumSet.Of(E02, E68));
            }

            [TestCase(ExpectedResult = false)]
            [TestCase(E02, ExpectedResult = false)]
            [TestCase(E68, ExpectedResult = false)]
            [TestCase(E22, ExpectedResult = false)]
            [TestCase(E02, E68, ExpectedResult = true)]
            [TestCase(E02, E22, ExpectedResult = false)]
            [TestCase(E68, E22, ExpectedResult = false)]
            [TestCase(E02, E68, E22, ExpectedResult = true)]
            public bool IsSupersetOf_E02_E68(params E[] elements)
            {
                return EnumSet.Of(elements).IsSupersetOf(EnumSet.Of(E02, E68));
            }

            [TestCase(ExpectedResult = false)]
            [TestCase(E02, ExpectedResult = true)]
            [TestCase(E68, ExpectedResult = true)]
            [TestCase(E22, ExpectedResult = false)]
            [TestCase(E02, E68, ExpectedResult = true)]
            [TestCase(E02, E22, ExpectedResult = true)]
            [TestCase(E68, E22, ExpectedResult = true)]
            [TestCase(E02, E68, E22, ExpectedResult = true)]
            public bool Overlaps_E02_E68(params E[] elements)
            {
                return EnumSet.Of(elements).Overlaps(EnumSet.Of(E02, E68));
            }

            [Test]
            public void WhenCleared_CountIsZero()
            {
                var a = EnumSet.Of(E02, E68, E22);
                a.Clear();
                Assert.That(a.Count, Is.EqualTo(0));
            }

            [Test]
            public void WhenCopiedToNullArray_ThenThrows()
            {
                Assert.That(() => EnumSet.Of(E02, E68, E22).CopyTo(null, 0), Throws.ArgumentNullException);
            }

            [Test]
            public void WhenCopiedToArrayWithTooFewElements_ThenThrows()
            {
                Assert.That(() => EnumSet.Of(E02, E68, E22).CopyTo(new E[2], 0), Throws.ArgumentException);
            }

            [Test]
            public void WhenCopiedToIndexWithTooFewElementsRemaining_ThenThrows()
            {
                Assert.That(() => EnumSet.Of(E02, E68, E22).CopyTo(new E[3], 1), Throws.ArgumentException);
            }

            [Test]
            public void WhenCopyIndexIsLessThanZero_ThenThrows()
            {
                Assert.That(() => EnumSet.Of(E02, E68, E22).CopyTo(new E[3], -1), Throws.TypeOf<ArgumentOutOfRangeException>());
            }

            [Test]
            public void WhenCopiedToArray_ArrayHasAllElements()
            {
                var a = EnumSet.Of(E02, E68, E22);
                var array = new E[3];
                a.CopyTo(array, 0);
                Assert.That(array, Is.EquivalentTo(new[] { E02, E68, E22 }));
            }

            public enum E
            {
                E00,
                E01,
                E02,
                E03,
                E04,
                E05,
                E06,
                E07,
                E08,
                E09,
                E10,
                E11,
                E12,
                E13,
                E14,
                E15,
                E16,
                E17,
                E18,
                E19,
                E20,
                E21,
                E22,
                E23,
                E24,
                E25,
                E26,
                E27,
                E28,
                E29,
                E30,
                E31,
                E32,
                E33,
                E34,
                E35,
                E36,
                E37,
                E38,
                E39,
                E40,
                E41,
                E42,
                E43,
                E44,
                E45,
                E46,
                E47,
                E48,
                E49,
                E50,
                E51,
                E52,
                E53,
                E54,
                E55,
                E56,
                E57,
                E58,
                E59,
                E60,
                E61,
                E62,
                E63,
                E64,
                E65,
                E66,
                E67,
                E68,
                E69
            }
        }
    }
}
