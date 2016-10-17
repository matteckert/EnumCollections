using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using static EnumCollections.Tests.EnumSetTests.SmallEnum;
using static EnumCollections.Tests.EnumSetTests.LargeEnum;

namespace EnumCollections.Tests
{
    [TestFixture]
    public class EnumSetTests
    {
        private static TestCaseData Case(params object[] args)
        {
            return new TestCaseData(args);
        }

        [TestFixture]
        public class GivenAnEnum
        {
            [Test, TestCaseSource(nameof(WhenEnumTypeArgumentPassedToOf_ThenEnumSetOfThatEnumCreated_Cases))]
            public void WhenEnumTypeArgumentPassedToOf_ThenEnumSetOfThatEnumCreated<T>(EnumSet<T> set, Type type)
            {
                Assert.That(set, Is.InstanceOf(type));
            }

            private static IEnumerable WhenEnumTypeArgumentPassedToOf_ThenEnumSetOfThatEnumCreated_Cases()
            {
                yield return Case(EnumSet.Of<SmallEnum>(), typeof(EnumSet<SmallEnum>));
                yield return Case(EnumSet.Of<LargeEnum>(), typeof(EnumSet<LargeEnum>));
            }
        }

        [TestFixture]
        public class GivenAnEnumSet
        {
            [Test, TestCaseSource(nameof(WhenCreatedWithNoElements_ThenEmpty_Cases))]
            public void WhenCreatedWithNoElements_ThenEmpty<T>(EnumSet<T> set)
            {
                Assert.That(set, Is.Empty);
            }

            private static IEnumerable WhenCreatedWithNoElements_ThenEmpty_Cases()
            {
                yield return Case(EnumSet.Of<SmallEnum>());
                yield return Case(EnumSet.Of<LargeEnum>());
            }

            [Test, TestCaseSource(nameof(WhenCreated_ThenCountIsNumberOfElements_Cases))]
            public int WhenCreated_ThenCountIsNumberOfElements<T>(EnumSet<T> set)
            {
                return set.Count;
            }

            private static IEnumerable WhenCreated_ThenCountIsNumberOfElements_Cases()
            {
                yield return Case(EnumSet.Of<SmallEnum>()).Returns(0);
                yield return Case(EnumSet.Of(A)).Returns(1);
                yield return Case(EnumSet.Of(B, C)).Returns(2);
                yield return Case(EnumSet.Of(A, B, C)).Returns(3);
                yield return Case(EnumSet.Of<LargeEnum>()).Returns(0);
                yield return Case(EnumSet.Of(E00)).Returns(1);
                yield return Case(EnumSet.Of(E03, E68)).Returns(2);
                yield return Case(EnumSet.Of(E03, E68, E69)).Returns(3);
            }

            [Test, TestCaseSource(nameof(WhenCreated_ThenOnlyContainsElementsCreatedWith_Cases))]
            public bool WhenCreated_ThenOnlyContainsElementsCreatedWith<T>(EnumSet<T> set, T element)
            {
                return set.Contains(element);
            }

            private static IEnumerable WhenCreated_ThenOnlyContainsElementsCreatedWith_Cases()
            {
                yield return Case(EnumSet.Of<SmallEnum>(), A).Returns(false);
                yield return Case(EnumSet.Of(B), A).Returns(false);
                yield return Case(EnumSet.Of<LargeEnum>(), E68).Returns(false);
                yield return Case(EnumSet.Of(E00), E68).Returns(false);
                yield return Case(EnumSet.Of(A), A).Returns(true);
                yield return Case(EnumSet.Of(E00), E00).Returns(true);
                yield return Case(EnumSet.Of(E68), E68).Returns(true);
            }

            [Test, TestCaseSource(nameof(WhenElementsAdded_ThenCountIsTotalNumberOfElements_Cases))]
            public int WhenElementsAdded_ThenCountIsTotalNumberOfElements<T>(EnumSet<T> set, List<T> elements)
            {
                foreach (var e in elements)
                    set.Add(e);
                return set.Count;
            }

            private static IEnumerable WhenElementsAdded_ThenCountIsTotalNumberOfElements_Cases()
            {
                yield return Case(EnumSet.Of<SmallEnum>(), new List<SmallEnum> {A}).Returns(1);
                yield return Case(EnumSet.Of<SmallEnum>(), new List<SmallEnum> { A, B, C }).Returns(3);
                yield return Case(EnumSet.Of(A), new List<SmallEnum> { A, B }).Returns(2);
                yield return Case(EnumSet.Of<LargeEnum>(), new List<LargeEnum> { E00 }).Returns(1);
                yield return Case(EnumSet.Of<LargeEnum>(), new List<LargeEnum> { E00, E01, E69 }).Returns(3);
                yield return Case(EnumSet.Of(E00), new List<LargeEnum> { E00, E69 }).Returns(2);
            }

            [Test, TestCaseSource(nameof(WhenElementsRemoved_ThenCountIsRemainingNumberOfElements_Cases))]
            public int WhenElementsRemoved_ThenCountIsRemainingNumberOfElements<T>(EnumSet<T> set, List<T> elements)
            {
                foreach (var e in elements)
                    set.Remove(e);
                return set.Count;
            }

            private static IEnumerable WhenElementsRemoved_ThenCountIsRemainingNumberOfElements_Cases()
            {
                yield return Case(EnumSet.Of(A, B), new List<SmallEnum> {A}).Returns(1);
                yield return Case(EnumSet.Of(A, B, C), new List<SmallEnum> { B, C }).Returns(1);
                yield return Case(EnumSet.Of(A), new List<SmallEnum> { A }).Returns(0);
                yield return Case(EnumSet.Of(E00, E01), new List<LargeEnum> { E00 }).Returns(1);
                yield return Case(EnumSet.Of(E00, E01, E69), new List<LargeEnum> { E01, E69 }).Returns(1);
                yield return Case(EnumSet.Of(E00), new List<LargeEnum> { E00 }).Returns(0);
            }

            [Test, TestCaseSource(nameof(WhenComparedToNull_ThenThrows_Cases))]
            public void WhenComparedToNull_ThenThrows<T>(EnumSet<T> set)
            {
                Assert.That(() => set.SetEquals(null), Throws.ArgumentNullException);
            }

            private static IEnumerable WhenComparedToNull_ThenThrows_Cases()
            {
                yield return Case(EnumSet.Of<SmallEnum>());
                yield return Case(EnumSet.Of<LargeEnum>());
            }

            [Test, TestCaseSource(nameof(WhenCleared_SetIsEmpty_Cases))]
            public void WhenCleared_SetIsEmpty<T>(EnumSet<T> set)
            {
                set.Clear();
                Assert.That(set, Is.Empty);
                Assert.That(set.Count, Is.EqualTo(0));
            }

            private static IEnumerable WhenCleared_SetIsEmpty_Cases()
            {
                yield return Case(EnumSet.Of<SmallEnum>());
                yield return Case(EnumSet.Of<LargeEnum>());
                yield return Case(EnumSet.Of(A, B));
                yield return Case(EnumSet.Of(E00, E68));
            }

            [Test, TestCaseSource(nameof(WhenCopyToNullArray_ThenThrows_Cases))]
            public void WhenCopyToNullArray_ThenThrows<T>(EnumSet<T> set )
            {
                Assert.That(() => set.CopyTo(null, 0), Throws.ArgumentNullException);
            }

            private static IEnumerable WhenCopyToNullArray_ThenThrows_Cases()
            {
                yield return Case(EnumSet.Of<SmallEnum>());
                yield return Case(EnumSet.Of<LargeEnum>());
            }

            [Test, TestCaseSource(nameof(WhenCopyToArrayWithTooFewElements_ThenThrows_Cases))]
            public void WhenCopyToArrayWithTooFewElements_ThenThrows<T>(EnumSet<T> set, int size )
            {
                Assert.That(()=> set.CopyTo(new T[size], 0), Throws.ArgumentException);
            }

            private static IEnumerable WhenCopyToArrayWithTooFewElements_ThenThrows_Cases()
            {
                yield return Case(EnumSet.Of(A, B, C), 2);
                yield return Case(EnumSet.Of(E00, E68), 1);
            }

            [Test, TestCaseSource(nameof(WhenCopyToIndexWithTooFewElementsRemaining_ThenThrows_Cases))]
            public void WhenCopyToIndexWithTooFewElementsRemaining_ThenThrows<T>(EnumSet<T> set, int size, int index )
            {
                Assert.That(() => set.CopyTo(new T[size], index), Throws.ArgumentException);
            }

            private static IEnumerable WhenCopyToIndexWithTooFewElementsRemaining_ThenThrows_Cases()
            {
                yield return Case(EnumSet.Of(A), 1, 1);
                yield return Case(EnumSet.Of(E68), 1, 1);
            }

            [Test, TestCaseSource(nameof(WhenCopyToIndexIsLessThanZero_ThenThrows_Cases))]
            public void WhenCopyToIndexIsLessThanZero_ThenThrows<T>(EnumSet<T> set )
            {
                Assert.That(() => set.CopyTo(new T[1], -1), Throws.TypeOf<ArgumentOutOfRangeException>());
            }

            private static IEnumerable WhenCopyToIndexIsLessThanZero_ThenThrows_Cases()
            {
                yield return Case(EnumSet.Of(A));
                yield return Case(EnumSet.Of(E68));
            }

            [Test, TestCaseSource(nameof(WhenCopyToArray_ArrayHasAllElements_Cases))]
            public void WhenCopyToArray_ArrayHasAllElements<T>(EnumSet<T> set, int size )
            {
                var array = new T[size];
                set.CopyTo(array, 0);
                Assert.That(array, Is.EquivalentTo(set));
            }

            private static IEnumerable WhenCopyToArray_ArrayHasAllElements_Cases()
            {
                yield return Case(EnumSet.Of<SmallEnum>(), 0);
                yield return Case(EnumSet.Of(A, B), 2);
                yield return Case(EnumSet.Of<LargeEnum>(), 0);
                yield return Case(EnumSet.Of(E00, E68), 2);
            }
        }

        [TestFixture]
        public class GivenTwoEnumSetsOfTheSameEnum
        {
            [Test, TestCaseSource(nameof(WhenSetEquals_ThenTrueIfSameElements_Cases))]
            public bool WhenSetEquals_ThenTrueIfSameElements<T>(EnumSet<T> a, EnumSet<T> b)
            {
                return a.SetEquals(b);
            }

            private static IEnumerable WhenSetEquals_ThenTrueIfSameElements_Cases()
            {
                yield return Case(EnumSet.Of<SmallEnum>(), EnumSet.Of<SmallEnum>()).Returns(true);
                yield return Case(EnumSet.Of(A, B), EnumSet.Of(A, B)).Returns(true);
                yield return Case(EnumSet.Of<LargeEnum>(), EnumSet.Of<LargeEnum>()).Returns(true);
                yield return Case(EnumSet.Of(E00, E68), EnumSet.Of(E00, E68)).Returns(true);
                yield return Case(EnumSet.Of<SmallEnum>(), EnumSet.Of(A)).Returns(false);
                yield return Case(EnumSet.Of(A, B), EnumSet.Of(A, C)).Returns(false);
                yield return Case(EnumSet.Of<LargeEnum>(), EnumSet.Of(E00)).Returns(false);
                yield return Case(EnumSet.Of(E00, E68), EnumSet.Of(E00, E69)).Returns(false);
            }

            [Test, TestCaseSource(nameof(WhenSymmetricExceptWith_ThenDifferentElementsKept_Cases))]
            public EnumSet<T> WhenSymmetricExceptWith_ThenDifferentElementsKept<T>(EnumSet<T> a, EnumSet<T> b )
            {
                a.SymmetricExceptWith(b);
                return a;
            }

            private static IEnumerable WhenSymmetricExceptWith_ThenDifferentElementsKept_Cases()
            {
                yield return Case(EnumSet.Of<SmallEnum>(), EnumSet.Of<SmallEnum>()).Returns(EnumSet.Of<SmallEnum>());
                yield return Case(EnumSet.Of(A, B), EnumSet.Of(B, C)).Returns(EnumSet.Of(A, C));
                yield return Case(EnumSet.Of(E00, E68), EnumSet.Of(E00, E11)).Returns(EnumSet.Of(E11, E68));
            }

            [Test, TestCaseSource(nameof(WhenExceptWith_ThenSameElementsRemoved_Cases))]
            public EnumSet<T> WhenExceptWith_ThenSameElementsRemoved<T>(EnumSet<T> a, EnumSet<T> b)
            {
                a.ExceptWith(b);
                return a;
            }

            private static IEnumerable WhenExceptWith_ThenSameElementsRemoved_Cases()
            {
                yield return Case(EnumSet.Of<SmallEnum>(), EnumSet.Of<SmallEnum>()).Returns(EnumSet.Of<SmallEnum>());
                yield return Case(EnumSet.Of(A, B), EnumSet.Of(B)).Returns(EnumSet.Of(A));
                yield return Case(EnumSet.Of(E00, E68), EnumSet.Of(E68)).Returns(EnumSet.Of(E00));
            }

            [Test, TestCaseSource(nameof(WhenIntersectWith_ThenOnlySameElementsKept_Cases))]
            public EnumSet<T> WhenIntersectWith_ThenOnlySameElementsKept<T>(EnumSet<T> a, EnumSet<T> b )
            {
                a.IntersectWith(b);
                return a;
            }

            private static IEnumerable WhenIntersectWith_ThenOnlySameElementsKept_Cases()
            {
                yield return Case(EnumSet.Of<SmallEnum>(), EnumSet.Of<SmallEnum>()).Returns(EnumSet.Of<SmallEnum>());
                yield return Case(EnumSet.Of(A, B), EnumSet.Of(B, C)).Returns(EnumSet.Of(B));
                yield return Case(EnumSet.Of(E00, E68), EnumSet.Of(E68)).Returns(EnumSet.Of(E68));
            }

            [Test, TestCaseSource(nameof(WhenUnionWith_ThenDifferentElementsAdded_Cases))]
            public EnumSet<T> WhenUnionWith_ThenDifferentElementsAdded<T>(EnumSet<T> a, EnumSet<T> b)
            {
                a.UnionWith(b);
                return a;
            }

            private static IEnumerable WhenUnionWith_ThenDifferentElementsAdded_Cases()
            {
                yield return Case(EnumSet.Of(A, B), EnumSet.Of(B, C)).Returns(EnumSet.Of(A, B, C));
                yield return Case(EnumSet.Of(E00, E68), EnumSet.Of(E68, E69)).Returns(EnumSet.Of(E00, E69, E68));
            }

            [Test, TestCaseSource(nameof(WhenIsProperSubsetOf_ThenTrueIfStrictSubset_Cases))]
            public bool WhenIsProperSubsetOf_ThenReturnsIfStrictSubset<T>(EnumSet<T> a, EnumSet<T> b )
            {
                return a.IsProperSubsetOf(b);
            }

            private static IEnumerable WhenIsProperSubsetOf_ThenTrueIfStrictSubset_Cases()
            {
                yield return Case(EnumSet.Of<SmallEnum>(), EnumSet.Of<SmallEnum>()).Returns(false);
                yield return Case(EnumSet.Of<SmallEnum>(), EnumSet.Of(A)).Returns(true);
                yield return Case(EnumSet.Of(A), EnumSet.Of(A)).Returns(false);
                yield return Case(EnumSet.Of(B), EnumSet.Of(A, B)).Returns(true);
                yield return Case(EnumSet.Of(A, B, C), EnumSet.Of(A, B)).Returns(false);
                yield return Case(EnumSet.Of<LargeEnum>(), EnumSet.Of<LargeEnum>()).Returns(false);
                yield return Case(EnumSet.Of<LargeEnum>(), EnumSet.Of(E00)).Returns(true);
                yield return Case(EnumSet.Of(E67), EnumSet.Of(E00, E67)).Returns(true);
                yield return Case(EnumSet.Of(E11, E00, E67), EnumSet.Of(E00, E11)).Returns(false);
            }

            [Test, TestCaseSource(nameof(WhenIsSubsetOf_ThenTrueIfSubset_Cases))]
            public bool WhenIsSubsetOf_ThenTrueIfSubset<T>(EnumSet<T> a, EnumSet<T> b )
            {
                return a.IsSubsetOf(b);
            }

            private static IEnumerable WhenIsSubsetOf_ThenTrueIfSubset_Cases()
            {
                yield return Case(EnumSet.Of<SmallEnum>(), EnumSet.Of<SmallEnum>()).Returns(true);
                yield return Case(EnumSet.Of<SmallEnum>(), EnumSet.Of(A)).Returns(true);
                yield return Case(EnumSet.Of(A), EnumSet.Of(A)).Returns(true);
                yield return Case(EnumSet.Of(B), EnumSet.Of(A, B)).Returns(true);
                yield return Case(EnumSet.Of(A, B, C), EnumSet.Of(A, B)).Returns(false);
                yield return Case(EnumSet.Of<LargeEnum>(), EnumSet.Of<LargeEnum>()).Returns(true);
                yield return Case(EnumSet.Of<LargeEnum>(), EnumSet.Of(E00)).Returns(true);
                yield return Case(EnumSet.Of(E67), EnumSet.Of(E00, E67)).Returns(true);
                yield return Case(EnumSet.Of(E11, E00, E67), EnumSet.Of(E00, E11)).Returns(false);
            }

            [Test, TestCaseSource(nameof(WhenIsProperSupersetOf_ThenTrueIfStrictSuperset_Cases))]
            public bool WhenIsProperSupersetOf_ThenTrueIfStrictSuperset<T>(EnumSet<T> a, EnumSet<T> b )
            {
                return a.IsProperSupersetOf(b);
            }

            private static IEnumerable WhenIsProperSupersetOf_ThenTrueIfStrictSuperset_Cases()
            {
                yield return Case(EnumSet.Of<SmallEnum>(), EnumSet.Of<SmallEnum>()).Returns(false);
                yield return Case(EnumSet.Of<SmallEnum>(), EnumSet.Of(A)).Returns(false);
                yield return Case(EnumSet.Of(A), EnumSet.Of(A)).Returns(false);
                yield return Case(EnumSet.Of(A, B), EnumSet.Of(B)).Returns(true);
                yield return Case(EnumSet.Of(A, B, C), EnumSet.Of(A, B)).Returns(true);
                yield return Case(EnumSet.Of<LargeEnum>(), EnumSet.Of<LargeEnum>()).Returns(false);
                yield return Case(EnumSet.Of<LargeEnum>(), EnumSet.Of(E00)).Returns(false);
                yield return Case(EnumSet.Of(E00, E67), EnumSet.Of(E67)).Returns(true);
                yield return Case(EnumSet.Of(E11, E00, E67), EnumSet.Of(E00, E11)).Returns(true);
            }

            [Test, TestCaseSource(nameof(WhenIsSupersetOf_ThenTrueIfSuperset_Cases))]
            public bool WhenIsSupersetOf_ThenTrueIfSuperset<T>(EnumSet<T> a, EnumSet<T> b )
            {
                return a.IsSupersetOf(b);
            }

            private static IEnumerable WhenIsSupersetOf_ThenTrueIfSuperset_Cases()
            {
                yield return Case(EnumSet.Of<SmallEnum>(), EnumSet.Of<SmallEnum>()).Returns(true);
                yield return Case(EnumSet.Of<SmallEnum>(), EnumSet.Of(A)).Returns(false);
                yield return Case(EnumSet.Of(A), EnumSet.Of(A)).Returns(true);
                yield return Case(EnumSet.Of(A, B), EnumSet.Of(B)).Returns(true);
                yield return Case(EnumSet.Of(A, B, C), EnumSet.Of(A, B)).Returns(true);
                yield return Case(EnumSet.Of<LargeEnum>(), EnumSet.Of<LargeEnum>()).Returns(true);
                yield return Case(EnumSet.Of<LargeEnum>(), EnumSet.Of(E00)).Returns(false);
                yield return Case(EnumSet.Of(E00, E67), EnumSet.Of(E67)).Returns(true);
                yield return Case(EnumSet.Of(E11, E00, E67), EnumSet.Of(E00, E11)).Returns(true);
            }

            [Test, TestCaseSource(nameof(WhenOverlaps_ThenTrueWhenSomeElementsShared_Cases))]
            public bool WhenOverlaps_ThenTrueWhenSomeElementsShared<T>(EnumSet<T> a, EnumSet<T> b )
            {
                return a.Overlaps(b);
            }

            private static IEnumerable WhenOverlaps_ThenTrueWhenSomeElementsShared_Cases()
            {
                yield return Case(EnumSet.Of<SmallEnum>(), EnumSet.Of<SmallEnum>()).Returns(false);
                yield return Case(EnumSet.Of(A, B), EnumSet.Of(B)).Returns(true);
                yield return Case(EnumSet.Of(A, B), EnumSet.Of(C)).Returns(false);
                yield return Case(EnumSet.Of<LargeEnum>(), EnumSet.Of<LargeEnum>()).Returns(false);
                yield return Case(EnumSet.Of(E00, E67), EnumSet.Of(E67)).Returns(true);
                yield return Case(EnumSet.Of(E00, E67), EnumSet.Of(E11)).Returns(false);
            }
        }

        public enum SmallEnum
        {
            A,
            B,
            C
        }

        public enum LargeEnum
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

