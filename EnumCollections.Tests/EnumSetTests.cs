using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NUnit.Framework;

using static EnumCollections.Tests.EnumSetTests.SmallEnum;
using static EnumCollections.Tests.EnumSetTests.LargeEnum;

namespace EnumCollections.Tests;

[TestFixture]
public class EnumSetTests
{
    private static TestCaseData Case(params object[] args)
    {
        return new TestCaseData(args);
    }

    [TestFixture]
    public class ReadMeTest
    {
        private enum Bird
        {
            BlueJay,        // 0
            Stork,          // 1
            Puffin,         // 2
            SeaParrot = 2,  // 2
            Chicken         // 3
        }

        [Test]
        public void WhenEnumValuesAreTheSame_ThenEnumValueNamesAreAliases()
        {
            var a = EnumSet.Of(Bird.Puffin);
            var b = EnumSet.Of(Bird.SeaParrot);
            Assert.That(a, Is.EqualTo(b));
        }
    }

    [TestFixture]
    public class GivenAnEnum
    {
        [Test, TestCaseSource(nameof(WhenEnumTypeArgumentPassedToOf_ThenEnumSetOfThatEnumCreated_Cases))]
        public void WhenEnumTypeArgumentPassedToOf_ThenEnumSetOfThatEnumCreated<T>(EnumSet<T> set, Type type) where T : struct, Enum
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
        public void WhenCreatedWithNoElements_ThenEmpty<T>(EnumSet<T> set) where T : struct, Enum
        {
            Assert.That(set, Is.Empty);
        }

        private static IEnumerable WhenCreatedWithNoElements_ThenEmpty_Cases()
        {
            yield return Case(EnumSet.Of<SmallEnum>());
            yield return Case(EnumSet.Of<LargeEnum>());
        }

        [Test, TestCaseSource(nameof(WhenCreated_ThenCountIsNumberOfElements_Cases))]
        public int WhenCreated_ThenCountIsNumberOfElements<T>(EnumSet<T> set) where T : struct, Enum
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
        public bool WhenCreated_ThenOnlyContainsElementsCreatedWith<T>(EnumSet<T> set, T element) where T : struct, Enum
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
        public int WhenElementsAdded_ThenCountIsTotalNumberOfElements<T>(EnumSet<T> set, List<T> elements) where T : struct, Enum
        {
            foreach (var e in elements)
                set.Add(e);
            return set.Count;
        }

        private static IEnumerable WhenElementsAdded_ThenCountIsTotalNumberOfElements_Cases()
        {
            yield return Case(EnumSet.Of<SmallEnum>(), new List<SmallEnum> { A }).Returns(1);
            yield return Case(EnumSet.Of<SmallEnum>(), new List<SmallEnum> { A, B, C }).Returns(3);
            yield return Case(EnumSet.Of(A), new List<SmallEnum> { A, B }).Returns(2);
            yield return Case(EnumSet.Of<LargeEnum>(), new List<LargeEnum> { E00 }).Returns(1);
            yield return Case(EnumSet.Of<LargeEnum>(), new List<LargeEnum> { E00, E01, E69 }).Returns(3);
            yield return Case(EnumSet.Of(E00), new List<LargeEnum> { E00, E69 }).Returns(2);
        }

        [Test, TestCaseSource(nameof(WhenElementsRemoved_ThenCountIsRemainingNumberOfElements_Cases))]
        public int WhenElementsRemoved_ThenCountIsRemainingNumberOfElements<T>(EnumSet<T> set, List<T> elements) where T : struct, Enum
        {
            foreach (var e in elements)
                set.Remove(e);
            return set.Count;
        }

        private static IEnumerable WhenElementsRemoved_ThenCountIsRemainingNumberOfElements_Cases()
        {
            yield return Case(EnumSet.Of(A, B), new List<SmallEnum> { A }).Returns(1);
            yield return Case(EnumSet.Of(A, B, C), new List<SmallEnum> { B, C }).Returns(1);
            yield return Case(EnumSet.Of(A), new List<SmallEnum> { A }).Returns(0);
            yield return Case(EnumSet.Of(E00, E01), new List<LargeEnum> { E00 }).Returns(1);
            yield return Case(EnumSet.Of(E00, E01, E69), new List<LargeEnum> { E01, E69 }).Returns(1);
            yield return Case(EnumSet.Of(E00), new List<LargeEnum> { E00 }).Returns(0);
        }

        [Test, TestCaseSource(nameof(WhenComparedToNull_ThenThrows_Cases))]
        public void WhenComparedToNull_ThenThrows<T>(EnumSet<T> set) where T : struct, Enum
        {
            Assert.That(() => set.SetEquals(null!), Throws.ArgumentNullException);
        }

        private static IEnumerable WhenComparedToNull_ThenThrows_Cases()
        {
            yield return Case(EnumSet.Of<SmallEnum>());
            yield return Case(EnumSet.Of<LargeEnum>());
        }

        [Test, TestCaseSource(nameof(WhenCleared_SetIsEmpty_Cases))]
        public void WhenCleared_SetIsEmpty<T>(EnumSet<T> set) where T : struct, Enum
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
        public void WhenCopyToNullArray_ThenThrows<T>(EnumSet<T> set) where T : struct, Enum
        {
            Assert.That(() => set.CopyTo(null!, 0), Throws.ArgumentNullException);
        }

        private static IEnumerable WhenCopyToNullArray_ThenThrows_Cases()
        {
            yield return Case(EnumSet.Of<SmallEnum>());
            yield return Case(EnumSet.Of<LargeEnum>());
        }

        [Test, TestCaseSource(nameof(WhenCopyToArrayWithTooFewElements_ThenThrows_Cases))]
        public void WhenCopyToArrayWithTooFewElements_ThenThrows<T>(EnumSet<T> set, int size) where T : struct, Enum
        {
            Assert.That(() => set.CopyTo(new T[size], 0), Throws.ArgumentException);
        }

        private static IEnumerable WhenCopyToArrayWithTooFewElements_ThenThrows_Cases()
        {
            yield return Case(EnumSet.Of(A, B, C), 2);
            yield return Case(EnumSet.Of(E00, E68), 1);
        }

        [Test, TestCaseSource(nameof(WhenCopyToIndexWithTooFewElementsRemaining_ThenThrows_Cases))]
        public void WhenCopyToIndexWithTooFewElementsRemaining_ThenThrows<T>(EnumSet<T> set, int size, int index) where T : struct, Enum
        {
            Assert.That(() => set.CopyTo(new T[size], index), Throws.ArgumentException);
        }

        private static IEnumerable WhenCopyToIndexWithTooFewElementsRemaining_ThenThrows_Cases()
        {
            yield return Case(EnumSet.Of(A), 1, 1);
            yield return Case(EnumSet.Of(E68), 1, 1);
        }

        [Test, TestCaseSource(nameof(WhenCopyToIndexIsLessThanZero_ThenThrows_Cases))]
        public void WhenCopyToIndexIsLessThanZero_ThenThrows<T>(EnumSet<T> set) where T : struct, Enum
        {
            Assert.That(() => set.CopyTo(new T[1], -1), Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        private static IEnumerable WhenCopyToIndexIsLessThanZero_ThenThrows_Cases()
        {
            yield return Case(EnumSet.Of(A));
            yield return Case(EnumSet.Of(E68));
        }

        [Test, TestCaseSource(nameof(WhenCopyToArray_ArrayHasAllElements_Cases))]
        public void WhenCopyToArray_ArrayHasAllElements<T>(EnumSet<T> set, int size) where T : struct, Enum
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
        public bool WhenSetEquals_ThenTrueIfSameElements<T>(EnumSet<T> a, EnumSet<T> b) where T : struct, Enum
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
        public EnumSet<T> WhenSymmetricExceptWith_ThenDifferentElementsKept<T>(EnumSet<T> a, EnumSet<T> b) where T : struct, Enum
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
        public EnumSet<T> WhenExceptWith_ThenSameElementsRemoved<T>(EnumSet<T> a, EnumSet<T> b) where T : struct, Enum
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
        public EnumSet<T> WhenIntersectWith_ThenOnlySameElementsKept<T>(EnumSet<T> a, EnumSet<T> b) where T : struct, Enum
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
        public EnumSet<T> WhenUnionWith_ThenDifferentElementsAdded<T>(EnumSet<T> a, EnumSet<T> b) where T : struct, Enum
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
        public bool WhenIsProperSubsetOf_ThenReturnsIfStrictSubset<T>(EnumSet<T> a, EnumSet<T> b) where T : struct, Enum
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
        public bool WhenIsSubsetOf_ThenTrueIfSubset<T>(EnumSet<T> a, EnumSet<T> b) where T : struct, Enum
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
        public bool WhenIsProperSupersetOf_ThenTrueIfStrictSuperset<T>(EnumSet<T> a, EnumSet<T> b) where T : struct, Enum
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
        public bool WhenIsSupersetOf_ThenTrueIfSuperset<T>(EnumSet<T> a, EnumSet<T> b) where T : struct, Enum
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
        public bool WhenOverlaps_ThenTrueWhenSomeElementsShared<T>(EnumSet<T> a, EnumSet<T> b) where T : struct, Enum
        {
            return a.Overlaps(b);
        }

        private static IEnumerable WhenOverlaps_ThenTrueWhenSomeElementsShared_Cases()
        {
            yield return Case(EnumSet.Of<SmallEnum>(), EnumSet.Of<SmallEnum>()).Returns(false);
            yield return Case(EnumSet.Of(A, D), EnumSet.Of(B)).Returns(true);
            yield return Case(EnumSet.Of(A, D), EnumSet.Of(C)).Returns(false);
            yield return Case(EnumSet.Of<LargeEnum>(), EnumSet.Of<LargeEnum>()).Returns(false);
            yield return Case(EnumSet.Of(E00, E67), EnumSet.Of(E67)).Returns(true);
            yield return Case(EnumSet.Of(E00, E67), EnumSet.Of(E11)).Returns(false);
        }
    }

    [TestFixture]
    public class GivenDifferentConstructionMethods
    {
        [Test, TestCaseSource(nameof(WhenCreatedFromList_ThenContainsAllElements_Cases))]
        public bool WhenCreatedFromList_ThenContainsAllElements<T>(List<T> items, T element) where T : struct, Enum
        {
            return EnumSet.Of(items).Contains(element);
        }

        private static IEnumerable WhenCreatedFromList_ThenContainsAllElements_Cases()
        {
            yield return Case(new List<SmallEnum> { A, B }, A).Returns(true);
            yield return Case(new List<SmallEnum> { A, B }, C).Returns(false);
            yield return Case(new List<LargeEnum> { E00, E68 }, E68).Returns(true);
            yield return Case(new List<LargeEnum> { E00, E68 }, E11).Returns(false);
        }

        [Test, TestCaseSource(nameof(WhenCreatedFromArray_ThenContainsAllElements_Cases))]
        public int WhenCreatedFromArray_ThenContainsAllElements<T>(T[] items) where T : struct, Enum
        {
            return EnumSet.Of(items).Count;
        }

        private static IEnumerable WhenCreatedFromArray_ThenContainsAllElements_Cases()
        {
            yield return Case(new[] { A, B, C }).Returns(3);
            yield return Case(Array.Empty<SmallEnum>()).Returns(0);
            yield return Case(new[] { E00, E01, E68, E69 }).Returns(4);
            yield return Case(Array.Empty<LargeEnum>()).Returns(0);
        }

        [Test, TestCaseSource(nameof(WhenCreatedFromHashSet_ThenContainsAllElements_Cases))]
        public int WhenCreatedFromHashSet_ThenContainsAllElements<T>(HashSet<T> items) where T : struct, Enum
        {
            return EnumSet.Of(items).Count;
        }

        private static IEnumerable WhenCreatedFromHashSet_ThenContainsAllElements_Cases()
        {
            yield return Case(new HashSet<SmallEnum> { A, B, C }).Returns(3);
            yield return Case(new HashSet<SmallEnum>()).Returns(0);
            yield return Case(new HashSet<LargeEnum> { E00, E68, E69 }).Returns(3);
        }

        [Test, TestCaseSource(nameof(WhenCreatedFromAnotherEnumSet_ThenHasSameElements_Cases))]
        public bool WhenCreatedFromAnotherEnumSet_ThenHasSameElements<T>(EnumSet<T> source) where T : struct, Enum
        {
            var copy = EnumSet.Of(source);
            return copy.SetEquals(source);
        }

        private static IEnumerable WhenCreatedFromAnotherEnumSet_ThenHasSameElements_Cases()
        {
            yield return Case(EnumSet.Of<SmallEnum>()).Returns(true);
            yield return Case(EnumSet.Of(A, B, C)).Returns(true);
            yield return Case(EnumSet.Of<LargeEnum>()).Returns(true);
            yield return Case(EnumSet.Of(E00, E11, E68, E69)).Returns(true);
        }

        [Test]
        public void WhenCreatedFromEnumSet_ThenCopyIsIndependent()
        {
            var original = EnumSet.Of(A, B);
            var copy = EnumSet.Of(original);
            copy.Add(C);
            Assert.That(original, Does.Not.Contain(C));
            Assert.That(copy, Contains.Item(C));
        }

        [Test, TestCaseSource(nameof(WhenCreatedWithDuplicates_ThenDuplicatesIgnored_Cases))]
        public int WhenCreatedWithDuplicates_ThenDuplicatesIgnored<T>(EnumSet<T> set) where T : struct, Enum
        {
            return set.Count;
        }

        private static IEnumerable WhenCreatedWithDuplicates_ThenDuplicatesIgnored_Cases()
        {
            yield return Case(EnumSet.Of(A, A, A)).Returns(1);
            yield return Case(EnumSet.Of(A, B, A, B, C)).Returns(3);
            yield return Case(EnumSet.Of(E00, E00, E00)).Returns(1);
            yield return Case(EnumSet.Of(E00, E68, E00, E68, E69)).Returns(3);
        }

        [Test, TestCaseSource(nameof(WhenCreatedFromLinqEnumerable_ThenContainsAllElements_Cases))]
        public int WhenCreatedFromLinqEnumerable_ThenContainsAllElements<T>(IEnumerable<T> items) where T : struct, Enum
        {
            return EnumSet.Of(items).Count;
        }

        private static IEnumerable WhenCreatedFromLinqEnumerable_ThenContainsAllElements_Cases()
        {
            yield return Case(new[] { A, B, C }.Where(x => x != C)).Returns(2);
            yield return Case(new[] { E00, E01, E02, E03 }.Where(x => x != E02)).Returns(3);
        }

        [Test]
        public void WhenCreatedViaPublicConstructor_ThenContainsAllElements()
        {
            var set = new EnumSet<SmallEnum>(A, B);
            Assert.That(set, Has.Count.EqualTo(2));
            Assert.That(set, Contains.Item(A));
            Assert.That(set, Contains.Item(B));
        }

        [Test]
        public void WhenCreatedViaPublicConstructorWithNoArgs_ThenEmpty()
        {
            var set = new EnumSet<SmallEnum>();
            Assert.That(set, Is.Empty);
        }

        [Test]
        public void WhenCreatedViaPublicConstructorWithList_ThenContainsAllElements()
        {
            var set = new EnumSet<LargeEnum>(new List<LargeEnum> { E00, E68 });
            Assert.That(set, Has.Count.EqualTo(2));
            Assert.That(set, Contains.Item(E00));
            Assert.That(set, Contains.Item(E68));
        }

        [Test]
        public void WhenSameElementsProvidedInDifferentOrders_ThenSetsAreEqual()
        {
            Assert.That(EnumSet.Of(A, B, C), Is.EqualTo(EnumSet.Of(C, B, A)));
            Assert.That(EnumSet.Of(E69, E00, E11), Is.EqualTo(EnumSet.Of(E11, E00, E69)));
        }
    }

    [TestFixture]
    public class GivenAnEnumSetAddRemoveBehavior
    {
        [Test, TestCaseSource(nameof(WhenAddNewItem_ThenReturnsTrue_Cases))]
        public bool WhenAddNewItem_ThenReturnsTrue<T>(EnumSet<T> set, T item) where T : struct, Enum
        {
            return set.Add(item);
        }

        private static IEnumerable WhenAddNewItem_ThenReturnsTrue_Cases()
        {
            yield return Case(EnumSet.Of<SmallEnum>(), A).Returns(true);
            yield return Case(EnumSet.Of(A), B).Returns(true);
            yield return Case(EnumSet.Of(A, B), C).Returns(true);
            yield return Case(EnumSet.Of<LargeEnum>(), E00).Returns(true);
            yield return Case(EnumSet.Of(E00), E68).Returns(true);
            yield return Case(EnumSet.Of(E00, E68), E69).Returns(true);
        }

        [Test, TestCaseSource(nameof(WhenAddExistingItem_ThenReturnsFalse_Cases))]
        public bool WhenAddExistingItem_ThenReturnsFalse<T>(EnumSet<T> set, T item) where T : struct, Enum
        {
            return set.Add(item);
        }

        private static IEnumerable WhenAddExistingItem_ThenReturnsFalse_Cases()
        {
            yield return Case(EnumSet.Of(A), A).Returns(false);
            yield return Case(EnumSet.Of(A, B, C), B).Returns(false);
            yield return Case(EnumSet.Of(E00), E00).Returns(false);
            yield return Case(EnumSet.Of(E00, E68, E69), E68).Returns(false);
        }

        [Test, TestCaseSource(nameof(WhenRemoveExistingItem_ThenReturnsTrue_Cases))]
        public bool WhenRemoveExistingItem_ThenReturnsTrue<T>(EnumSet<T> set, T item) where T : struct, Enum
        {
            return set.Remove(item);
        }

        private static IEnumerable WhenRemoveExistingItem_ThenReturnsTrue_Cases()
        {
            yield return Case(EnumSet.Of(A), A).Returns(true);
            yield return Case(EnumSet.Of(A, B, C), B).Returns(true);
            yield return Case(EnumSet.Of(E00), E00).Returns(true);
            yield return Case(EnumSet.Of(E00, E68, E69), E69).Returns(true);
        }

        [Test, TestCaseSource(nameof(WhenRemoveNonExistingItem_ThenReturnsFalse_Cases))]
        public bool WhenRemoveNonExistingItem_ThenReturnsFalse<T>(EnumSet<T> set, T item) where T : struct, Enum
        {
            return set.Remove(item);
        }

        private static IEnumerable WhenRemoveNonExistingItem_ThenReturnsFalse_Cases()
        {
            yield return Case(EnumSet.Of<SmallEnum>(), A).Returns(false);
            yield return Case(EnumSet.Of(A), B).Returns(false);
            yield return Case(EnumSet.Of(A, B), C).Returns(false);
            yield return Case(EnumSet.Of<LargeEnum>(), E00).Returns(false);
            yield return Case(EnumSet.Of(E00), E68).Returns(false);
            yield return Case(EnumSet.Of(E00, E68), E11).Returns(false);
        }

        [Test]
        public void WhenItemAddedThenRemoved_ThenNotContained()
        {
            var set = EnumSet.Of<SmallEnum>();
            set.Add(A);
            Assert.That(set, Contains.Item(A));
            set.Remove(A);
            Assert.That(set, Does.Not.Contain(A));
        }

        [Test]
        public void WhenItemAddedThenRemoved_Large_ThenNotContained()
        {
            var set = EnumSet.Of<LargeEnum>();
            set.Add(E68);
            Assert.That(set, Contains.Item(E68));
            set.Remove(E68);
            Assert.That(set, Does.Not.Contain(E68));
        }

        [Test]
        public void WhenICollectionAddCalled_ThenItemAdded()
        {
            ICollection<SmallEnum> set = EnumSet.Of<SmallEnum>();
            set.Add(A);
            Assert.That(set, Contains.Item(A));
            Assert.That(set, Has.Count.EqualTo(1));
        }

        [Test]
        public void WhenICollectionAddCalled_Large_ThenItemAdded()
        {
            ICollection<LargeEnum> set = EnumSet.Of<LargeEnum>();
            set.Add(E68);
            Assert.That(set, Contains.Item(E68));
            Assert.That(set, Has.Count.EqualTo(1));
        }

        [Test]
        public void WhenManyAddRemoveCyclesPerformed_ThenStateRemainsConsistent()
        {
            var set = EnumSet.Of<LargeEnum>();
            foreach (var e in Enum.GetValues<LargeEnum>())
                set.Add(e);
            Assert.That(set.Count, Is.EqualTo(Enum.GetValues<LargeEnum>().Length));
            foreach (var e in Enum.GetValues<LargeEnum>())
                set.Remove(e);
            Assert.That(set, Is.Empty);
        }
    }

    [TestFixture]
    public class GivenAnEnumSetEnumeration
    {
        [Test]
        public void WhenIterated_ThenAllElementsYielded_Small()
        {
            var set = EnumSet.Of(A, B, C);
            var items = new List<SmallEnum>();
            foreach (var e in set)
                items.Add(e);
            Assert.That(items, Is.EquivalentTo([A, B, C]));
        }

        [Test]
        public void WhenIterated_ThenAllElementsYielded_Large()
        {
            var set = EnumSet.Of(E00, E11, E68, E69);
            var items = new List<LargeEnum>();
            foreach (var e in set)
                items.Add(e);
            Assert.That(items, Is.EquivalentTo([E00, E11, E68, E69]));
        }

        [Test]
        public void WhenEmptySetIterated_ThenNoElementsYielded()
        {
            var set = EnumSet.Of<SmallEnum>();
            var items = new List<SmallEnum>();
            foreach (var e in set)
                items.Add(e);
            Assert.That(items, Is.Empty);
        }

        [Test]
        public void WhenEmptyLargeSetIterated_ThenNoElementsYielded()
        {
            var set = EnumSet.Of<LargeEnum>();
            var items = new List<LargeEnum>();
            foreach (var e in set)
                items.Add(e);
            Assert.That(items, Is.Empty);
        }

        [Test]
        public void WhenIteratedViaNonGenericIEnumerable_ThenAllElementsYielded()
        {
            IEnumerable set = EnumSet.Of(A, B);
            var items = new List<object>();
            foreach (var e in set)
                items.Add(e);
            Assert.That(items, Is.EquivalentTo(new object[] { A, B }));
        }

        [Test]
        public void WhenGetEnumeratorCalledTwice_ThenEachEnumeratorIsIndependent()
        {
            var set = EnumSet.Of(A, B, C);
            using var e1 = set.GetEnumerator();
            using var e2 = set.GetEnumerator();
            Assert.That(e1.MoveNext(), Is.True);
            Assert.That(e2.MoveNext(), Is.True);
            Assert.That(e1.Current, Is.EqualTo(e2.Current));
        }

        [Test]
        public void WhenMoveNextCalledBeyondEnd_ThenReturnsFalse()
        {
            var set = EnumSet.Of(A);
            using var enumerator = set.GetEnumerator();
            Assert.That(enumerator.MoveNext(), Is.True);
            Assert.That(enumerator.MoveNext(), Is.False);
            Assert.That(enumerator.MoveNext(), Is.False);
        }

        [Test]
        public void WhenMoveNextCalledBeyondEnd_Large_ThenReturnsFalse()
        {
            var set = EnumSet.Of(E69);
            using var enumerator = set.GetEnumerator();
            Assert.That(enumerator.MoveNext(), Is.True);
            Assert.That(enumerator.MoveNext(), Is.False);
            Assert.That(enumerator.MoveNext(), Is.False);
        }

        [Test]
        public void WhenResetCalled_ThenIterationRestarts()
        {
            var set = EnumSet.Of(A, B);
            using var enumerator = set.GetEnumerator();
            enumerator.MoveNext();
            enumerator.MoveNext();
            Assert.That(enumerator.MoveNext(), Is.False);
            enumerator.Reset();
            Assert.That(enumerator.MoveNext(), Is.True);
            Assert.That(enumerator.MoveNext(), Is.True);
            Assert.That(enumerator.MoveNext(), Is.False);
        }

        [Test]
        public void WhenResetCalled_Large_ThenIterationRestarts()
        {
            var set = EnumSet.Of(E00, E68);
            using var enumerator = set.GetEnumerator();
            enumerator.MoveNext();
            enumerator.MoveNext();
            Assert.That(enumerator.MoveNext(), Is.False);
            enumerator.Reset();
            Assert.That(enumerator.MoveNext(), Is.True);
            Assert.That(enumerator.MoveNext(), Is.True);
            Assert.That(enumerator.MoveNext(), Is.False);
        }

        [Test]
        public void WhenIteratedMultipleTimes_ThenYieldsSameElements()
        {
            var set = EnumSet.Of(A, B, C);
            var first = new List<SmallEnum>();
            foreach (var e in set) first.Add(e);
            var second = new List<SmallEnum>();
            foreach (var e in set) second.Add(e);
            Assert.That(first, Is.EqualTo(second));
        }

        [Test]
        public void WhenToListCalled_ThenAllElementsPresent()
        {
            var items = EnumSet.Of(A, B, C).ToList();
            Assert.That(items, Is.EquivalentTo([A, B, C]));
        }
    }

    [TestFixture]
    public class GivenAnEnumSetWithAliases
    {
        [Test]
        public void WhenAliasAdded_ThenOriginalIsContained()
        {
            var set = EnumSet.Of(D);
            Assert.That(set, Contains.Item(B));
        }

        [Test]
        public void WhenOriginalAdded_ThenAliasIsContained()
        {
            var set = EnumSet.Of(B);
            Assert.That(set, Contains.Item(D));
        }

        [Test]
        public void WhenBothAliasAndOriginalAdded_ThenCountIsOne()
        {
            var set = EnumSet.Of(B, D);
            Assert.That(set, Has.Count.EqualTo(1));
        }

        [Test]
        public void WhenAliasRemoved_ThenOriginalAlsoRemoved()
        {
            var set = EnumSet.Of(B);
            set.Remove(D);
            Assert.That(set, Is.Empty);
        }

        [Test]
        public void WhenAliasAndOriginalProduceEqualSets_ThenSetEqualsTrue()
        {
            Assert.That(EnumSet.Of(B).SetEquals(EnumSet.Of(D)), Is.True);
        }

        [Test]
        public void WhenAddReturnsTrueOnce_ThenAliasAddReturnsFalse()
        {
            var set = EnumSet.Of<SmallEnum>();
            Assert.That(set.Add(B), Is.True);
            Assert.That(set.Add(D), Is.False);
        }
    }

    [TestFixture]
    public class GivenAnEnumSetMiscBehavior
    {
        [Test, TestCaseSource(nameof(WhenCheckingIsReadOnly_ThenFalse_Cases))]
        public bool WhenCheckingIsReadOnly_ThenFalse<T>(EnumSet<T> set) where T : struct, Enum
        {
            return set.IsReadOnly;
        }

        private static IEnumerable WhenCheckingIsReadOnly_ThenFalse_Cases()
        {
            yield return Case(EnumSet.Of<SmallEnum>()).Returns(false);
            yield return Case(EnumSet.Of<LargeEnum>()).Returns(false);
            yield return Case(EnumSet.Of(A, B)).Returns(false);
            yield return Case(EnumSet.Of(E00, E68)).Returns(false);
        }

        [Test]
        public void WhenClearCalledOnAlreadyEmptySet_ThenStillEmpty()
        {
            var set = EnumSet.Of<SmallEnum>();
            set.Clear();
            Assert.That(set, Is.Empty);
        }

        [Test]
        public void WhenClearCalledOnLargeEmptySet_ThenStillEmpty()
        {
            var set = EnumSet.Of<LargeEnum>();
            set.Clear();
            Assert.That(set, Is.Empty);
        }

        [Test]
        public void WhenClearCalledOnFullSet_ThenEmpty()
        {
            var set = EnumSet.Of(A, B, C);
            set.Clear();
            Assert.That(set, Is.Empty);
            Assert.That(set.Count, Is.EqualTo(0));
        }

        [Test]
        public void WhenClearCalledOnLargeFullSet_ThenEmpty()
        {
            var set = EnumSet.Of(Enum.GetValues<LargeEnum>());
            set.Clear();
            Assert.That(set, Is.Empty);
        }

        [Test]
        public void WhenElementsAddedAfterClear_ThenSetContainsThem()
        {
            var set = EnumSet.Of(A, B);
            set.Clear();
            set.Add(C);
            Assert.That(set, Has.Count.EqualTo(1));
            Assert.That(set, Contains.Item(C));
        }

        [Test, TestCaseSource(nameof(WhenContainsCheckedWithAllElements_ThenCorrectResults_Cases))]
        public bool WhenContainsCheckedWithAllElements_ThenCorrectResults<T>(EnumSet<T> set, T element) where T : struct, Enum
        {
            return set.Contains(element);
        }

        private static IEnumerable WhenContainsCheckedWithAllElements_ThenCorrectResults_Cases()
        {
            yield return Case(EnumSet.Of(A, B, C), A).Returns(true);
            yield return Case(EnumSet.Of(A, B, C), B).Returns(true);
            yield return Case(EnumSet.Of(A, B, C), C).Returns(true);
            yield return Case(EnumSet.Of(E00, E01, E32, E63, E64, E68, E69), E00).Returns(true);
            yield return Case(EnumSet.Of(E00, E01, E32, E63, E64, E68, E69), E32).Returns(true);
            yield return Case(EnumSet.Of(E00, E01, E32, E63, E64, E68, E69), E63).Returns(true);
            yield return Case(EnumSet.Of(E00, E01, E32, E63, E64, E68, E69), E64).Returns(true);
            yield return Case(EnumSet.Of(E00, E01, E32, E63, E64, E68, E69), E69).Returns(true);
            yield return Case(EnumSet.Of(E00, E01, E32, E63, E64, E68, E69), E02).Returns(false);
            yield return Case(EnumSet.Of(E00, E01, E32, E63, E64, E68, E69), E33).Returns(false);
        }

        [Test]
        public void WhenAllValuesAdded_ThenCountEqualsEnumValueCount()
        {
            var set = EnumSet.Of(Enum.GetValues<LargeEnum>());
            Assert.That(set.Count, Is.EqualTo(Enum.GetValues<LargeEnum>().Length));
        }

        [Test]
        public void WhenCopyToExactSizeArray_ThenArrayMatchesSet()
        {
            var set = EnumSet.Of(A, B, C);
            var array = new SmallEnum[3];
            set.CopyTo(array, 0);
            Assert.That(array, Is.EquivalentTo(set));
        }

        [Test]
        public void WhenCopyToLargerArrayWithOffset_ThenElementsPlacedCorrectly()
        {
            var set = EnumSet.Of(A, B);
            var array = new SmallEnum[5];
            set.CopyTo(array, 2);
            Assert.That(array[0], Is.EqualTo(default(SmallEnum)));
            Assert.That(array[1], Is.EqualTo(default(SmallEnum)));
            Assert.That(new[] { array[2], array[3] }, Is.EquivalentTo(set));
        }

        [Test]
        public void WhenCopyToLargeSetLargerArray_ThenElementsPlacedCorrectly()
        {
            var set = EnumSet.Of(E00, E68);
            var array = new LargeEnum[4];
            set.CopyTo(array, 1);
            Assert.That(new[] { array[1], array[2] }, Is.EquivalentTo(set));
        }

        [Test]
        public void WhenCopyToEmptySetIntoEmptyArrayWithZeroIndex_ThenNoThrow()
        {
            var set = EnumSet.Of<SmallEnum>();
            var array = Array.Empty<SmallEnum>();
            Assert.DoesNotThrow(() => set.CopyTo(array, 0));
        }

        [Test]
        public void WhenCopyToNonEmptyIntoArrayOfSufficientRemaining_ThenNoThrow()
        {
            var set = EnumSet.Of(A);
            var array = new SmallEnum[5];
            Assert.DoesNotThrow(() => set.CopyTo(array, 4));
        }
    }

    [TestFixture]
    public class GivenTwoEnumSetsOfTheSameEnumNullOperand
    {
        [Test, TestCaseSource(nameof(NullOperand_Cases))]
        public void WhenNullPassedToSetOperation_ThenThrows(Action action)
        {
            Assert.That(action, Throws.ArgumentNullException);
        }

        [SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed")]
        private static IEnumerable NullOperand_Cases()
        {
            yield return Case((Action)(() => EnumSet.Of(A).ExceptWith(null!)));
            yield return Case((Action)(() => EnumSet.Of(A).IntersectWith(null!)));
            yield return Case((Action)(() => EnumSet.Of(A).SymmetricExceptWith(null!)));
            yield return Case((Action)(() => EnumSet.Of(A).UnionWith(null!)));
            yield return Case((Action)(() => EnumSet.Of(A).IsSubsetOf(null!)));
            yield return Case((Action)(() => EnumSet.Of(A).IsSupersetOf(null!)));
            yield return Case((Action)(() => EnumSet.Of(A).IsProperSubsetOf(null!)));
            yield return Case((Action)(() => EnumSet.Of(A).IsProperSupersetOf(null!)));
            yield return Case((Action)(() => EnumSet.Of(A).Overlaps(null!)));
            yield return Case((Action)(() => EnumSet.Of(A).SetEquals(null!)));
            yield return Case((Action)(() => EnumSet.Of(E00).ExceptWith(null!)));
            yield return Case((Action)(() => EnumSet.Of(E00).IntersectWith(null!)));
            yield return Case((Action)(() => EnumSet.Of(E00).SymmetricExceptWith(null!)));
            yield return Case((Action)(() => EnumSet.Of(E00).UnionWith(null!)));
            yield return Case((Action)(() => EnumSet.Of(E00).IsSubsetOf(null!)));
            yield return Case((Action)(() => EnumSet.Of(E00).IsSupersetOf(null!)));
            yield return Case((Action)(() => EnumSet.Of(E00).IsProperSubsetOf(null!)));
            yield return Case((Action)(() => EnumSet.Of(E00).IsProperSupersetOf(null!)));
            yield return Case((Action)(() => EnumSet.Of(E00).Overlaps(null!)));
            yield return Case((Action)(() => EnumSet.Of(E00).SetEquals(null!)));
        }
    }

    [TestFixture]
    public class GivenTwoEnumSetsOfTheSameEnumSelfOperand
    {
        [Test]
        public void WhenUnionWithSelf_ThenSetUnchanged()
        {
            var set = EnumSet.Of(A, B);
            set.UnionWith(set);
            Assert.That(set, Is.EqualTo(EnumSet.Of(A, B)));
        }

        [Test]
        public void WhenIntersectWithSelf_ThenSetUnchanged()
        {
            var set = EnumSet.Of(A, B, C);
            set.IntersectWith(set);
            Assert.That(set, Is.EqualTo(EnumSet.Of(A, B, C)));
        }

        [Test]
        public void WhenExceptWithSelf_ThenSetIsEmpty()
        {
            var set = EnumSet.Of(A, B, C);
            set.ExceptWith(set);
            Assert.That(set, Is.Empty);
        }

        [Test]
        public void WhenSymmetricExceptWithSelf_ThenSetIsEmpty()
        {
            var set = EnumSet.Of(A, B, C);
            set.SymmetricExceptWith(set);
            Assert.That(set, Is.Empty);
        }

        [Test]
        public void WhenIsSubsetOfSelf_ThenTrue()
        {
            var set = EnumSet.Of(A, B);
            Assert.That(set.IsSubsetOf(set), Is.True);
        }

        [Test]
        public void WhenIsSupersetOfSelf_ThenTrue()
        {
            var set = EnumSet.Of(A, B);
            Assert.That(set.IsSupersetOf(set), Is.True);
        }

        [Test]
        public void WhenIsProperSubsetOfSelf_ThenFalse()
        {
            var set = EnumSet.Of(A, B);
            Assert.That(set.IsProperSubsetOf(set), Is.False);
        }

        [Test]
        public void WhenIsProperSupersetOfSelf_ThenFalse()
        {
            var set = EnumSet.Of(A, B);
            Assert.That(set.IsProperSupersetOf(set), Is.False);
        }

        [Test]
        public void WhenOverlapsSelf_ThenTrueIfNotEmpty()
        {
            var set = EnumSet.Of(A);
            Assert.That(set.Overlaps(set), Is.True);
        }

        [Test]
        public void WhenEmptyOverlapsSelf_ThenFalse()
        {
            var set = EnumSet.Of<SmallEnum>();
            Assert.That(set.Overlaps(set), Is.False);
        }

        [Test]
        public void WhenSetEqualsSelf_ThenTrue()
        {
            var set = EnumSet.Of(A, B);
            Assert.That(set.SetEquals(set), Is.True);
        }

        [Test]
        public void WhenUnionWithSelf_Large_ThenSetUnchanged()
        {
            var set = EnumSet.Of(E00, E68);
            set.UnionWith(set);
            Assert.That(set, Is.EqualTo(EnumSet.Of(E00, E68)));
        }

        [Test]
        public void WhenIntersectWithSelf_Large_ThenSetUnchanged()
        {
            var set = EnumSet.Of(E00, E11, E68);
            set.IntersectWith(set);
            Assert.That(set, Is.EqualTo(EnumSet.Of(E00, E11, E68)));
        }

        [Test]
        public void WhenExceptWithSelf_Large_ThenSetIsEmpty()
        {
            var set = EnumSet.Of(E00, E68);
            set.ExceptWith(set);
            Assert.That(set, Is.Empty);
        }

        [Test]
        public void WhenSymmetricExceptWithSelf_Large_ThenSetIsEmpty()
        {
            var set = EnumSet.Of(E00, E68);
            set.SymmetricExceptWith(set);
            Assert.That(set, Is.Empty);
        }

        [Test]
        public void WhenOverlapsSelf_Large_ThenTrueIfNotEmpty()
        {
            var set = EnumSet.Of(E00);
            Assert.That(set.Overlaps(set), Is.True);
        }
    }

    [TestFixture]
    public class GivenTwoEnumSetsOfTheSameEnumNonEnumSetOperand
    {
        [Test]
        public void WhenUnionWithList_ThenElementsMerged()
        {
            var set = EnumSet.Of(A);
            set.UnionWith(new List<SmallEnum> { B, C });
            Assert.That(set, Is.EqualTo(EnumSet.Of(A, B, C)));
        }

        [Test]
        public void WhenUnionWithArray_ThenElementsMerged()
        {
            var set = EnumSet.Of(A);
            set.UnionWith([B, C]);
            Assert.That(set, Is.EqualTo(EnumSet.Of(A, B, C)));
        }

        [Test]
        public void WhenUnionWithHashSet_ThenElementsMerged()
        {
            var set = EnumSet.Of(A);
            set.UnionWith(new HashSet<SmallEnum> { B, C });
            Assert.That(set, Is.EqualTo(EnumSet.Of(A, B, C)));
        }

        [Test]
        public void WhenUnionWithLinqEnumerable_ThenElementsMerged()
        {
            var set = EnumSet.Of(A);
            set.UnionWith(new[] { A, B, C }.Where(x => x != A));
            Assert.That(set, Is.EqualTo(EnumSet.Of(A, B, C)));
        }

        [Test]
        public void WhenIntersectWithList_ThenElementsFiltered()
        {
            var set = EnumSet.Of(A, B, C);
            set.IntersectWith(new List<SmallEnum> { B, C });
            Assert.That(set, Is.EqualTo(EnumSet.Of(B, C)));
        }

        [Test]
        public void WhenExceptWithArray_ThenElementsRemoved()
        {
            var set = EnumSet.Of(A, B, C);
            set.ExceptWith([A, C]);
            Assert.That(set, Is.EqualTo(EnumSet.Of(B)));
        }

        [Test]
        public void WhenSymmetricExceptWithList_ThenDifferenceKept()
        {
            var set = EnumSet.Of(A, B);
            set.SymmetricExceptWith(new List<SmallEnum> { B, C });
            Assert.That(set, Is.EqualTo(EnumSet.Of(A, C)));
        }

        [Test]
        public void WhenSetEqualsList_ThenCorrectResult()
        {
            var set = EnumSet.Of(A, B);
            Assert.That(set.SetEquals(new List<SmallEnum> { B, A }), Is.True);
            Assert.That(set.SetEquals(new List<SmallEnum> { A, B, C }), Is.False);
        }

        [Test]
        public void WhenOverlapsArray_ThenCorrectResult()
        {
            var set = EnumSet.Of(A, B);
            Assert.That(set.Overlaps([B, C]), Is.True);
            Assert.That(set.Overlaps([C]), Is.False);
        }

        [Test]
        public void WhenIsSubsetOfList_ThenCorrectResult()
        {
            var set = EnumSet.Of(A);
            Assert.That(set.IsSubsetOf(new List<SmallEnum> { A, B }), Is.True);
            Assert.That(set.IsSubsetOf(new List<SmallEnum> { B, C }), Is.False);
        }

        [Test]
        public void WhenUnionWithList_Large_ThenElementsMerged()
        {
            var set = EnumSet.Of(E00);
            set.UnionWith(new List<LargeEnum> { E68, E69 });
            Assert.That(set, Is.EqualTo(EnumSet.Of(E00, E68, E69)));
        }

        [Test]
        public void WhenIntersectWithArray_Large_ThenElementsFiltered()
        {
            var set = EnumSet.Of(E00, E11, E68);
            set.IntersectWith([E11, E68]);
            Assert.That(set, Is.EqualTo(EnumSet.Of(E11, E68)));
        }

        [Test]
        public void WhenExceptWithHashSet_Large_ThenElementsRemoved()
        {
            var set = EnumSet.Of(E00, E11, E68);
            set.ExceptWith(new HashSet<LargeEnum> { E00, E68 });
            Assert.That(set, Is.EqualTo(EnumSet.Of(E11)));
        }

        [Test]
        public void WhenSetEqualsList_Large_ThenCorrectResult()
        {
            var set = EnumSet.Of(E00, E68);
            Assert.That(set.SetEquals(new List<LargeEnum> { E68, E00 }), Is.True);
            Assert.That(set.SetEquals(new List<LargeEnum> { E00 }), Is.False);
        }

        [Test]
        public void WhenOverlapsLinqEnumerable_Large_ThenCorrectResult()
        {
            var set = EnumSet.Of(E00, E68);
            Assert.That(set.Overlaps(new[] { E00, E11 }.AsEnumerable()), Is.True);
            Assert.That(set.Overlaps(new[] { E11 }.AsEnumerable()), Is.False);
        }
    }

    [TestFixture]
    public class GivenTwoEnumSetsOfTheSameEnumEmptyAndFullOperand
    {
        [Test]
        public void WhenUnionWithEmpty_ThenSetUnchanged()
        {
            var set = EnumSet.Of(A, B);
            set.UnionWith(EnumSet.Of<SmallEnum>());
            Assert.That(set, Is.EqualTo(EnumSet.Of(A, B)));
        }

        [Test]
        public void WhenEmptyUnionWithSet_ThenReceivesAllElements()
        {
            var set = EnumSet.Of<SmallEnum>();
            set.UnionWith(EnumSet.Of(A, B));
            Assert.That(set, Is.EqualTo(EnumSet.Of(A, B)));
        }

        [Test]
        public void WhenIntersectWithEmpty_ThenSetIsEmpty()
        {
            var set = EnumSet.Of(A, B);
            set.IntersectWith(EnumSet.Of<SmallEnum>());
            Assert.That(set, Is.Empty);
        }

        [Test]
        public void WhenEmptyIntersectWithSet_ThenStillEmpty()
        {
            var set = EnumSet.Of<SmallEnum>();
            set.IntersectWith(EnumSet.Of(A, B));
            Assert.That(set, Is.Empty);
        }

        [Test]
        public void WhenExceptWithEmpty_ThenSetUnchanged()
        {
            var set = EnumSet.Of(A, B);
            set.ExceptWith(EnumSet.Of<SmallEnum>());
            Assert.That(set, Is.EqualTo(EnumSet.Of(A, B)));
        }

        [Test]
        public void WhenEmptyExceptWithSet_ThenStillEmpty()
        {
            var set = EnumSet.Of<SmallEnum>();
            set.ExceptWith(EnumSet.Of(A, B));
            Assert.That(set, Is.Empty);
        }

        [Test]
        public void WhenSymmetricExceptWithEmpty_ThenSetUnchanged()
        {
            var set = EnumSet.Of(A, B);
            set.SymmetricExceptWith(EnumSet.Of<SmallEnum>());
            Assert.That(set, Is.EqualTo(EnumSet.Of(A, B)));
        }

        [Test]
        public void WhenEmptySymmetricExceptWithSet_ThenReceivesAllElements()
        {
            var set = EnumSet.Of<SmallEnum>();
            set.SymmetricExceptWith(EnumSet.Of(A, B));
            Assert.That(set, Is.EqualTo(EnumSet.Of(A, B)));
        }

        [Test]
        public void WhenDisjointSetsIntersected_ThenEmpty()
        {
            var set = EnumSet.Of(A, B);
            set.IntersectWith(EnumSet.Of(C));
            Assert.That(set, Is.Empty);
        }

        [Test]
        public void WhenDisjointSetsChecked_OverlapsReturnsFalse()
        {
            Assert.That(EnumSet.Of(A, B).Overlaps(EnumSet.Of(C)), Is.False);
        }

        [Test]
        public void WhenEmptyIsSubsetOfEmpty_ThenTrue()
        {
            Assert.That(EnumSet.Of<SmallEnum>().IsSubsetOf(EnumSet.Of<SmallEnum>()), Is.True);
        }

        [Test]
        public void WhenEmptyIsSupersetOfEmpty_ThenTrue()
        {
            Assert.That(EnumSet.Of<SmallEnum>().IsSupersetOf(EnumSet.Of<SmallEnum>()), Is.True);
        }

        [Test]
        public void WhenEmptyIsSupersetOfNonEmpty_ThenFalse()
        {
            Assert.That(EnumSet.Of<SmallEnum>().IsSupersetOf(EnumSet.Of(A)), Is.False);
        }

        [Test]
        public void WhenNonEmptyIsSupersetOfEmpty_ThenTrue()
        {
            Assert.That(EnumSet.Of(A).IsSupersetOf(EnumSet.Of<SmallEnum>()), Is.True);
        }

        [Test]
        public void WhenFullSetIntersectedWithSubset_ThenEqualsSubset()
        {
            var all = EnumSet.Of(A, B, C);
            all.IntersectWith(EnumSet.Of(A, C));
            Assert.That(all, Is.EqualTo(EnumSet.Of(A, C)));
        }

        [Test]
        public void WhenFullSetUnionedWithSelf_ThenStillFull()
        {
            var all = EnumSet.Of(A, B, C);
            all.UnionWith(EnumSet.Of(A, B, C));
            Assert.That(all, Is.EqualTo(EnumSet.Of(A, B, C)));
        }

        [Test]
        public void WhenUnionWithEmpty_Large_ThenSetUnchanged()
        {
            var set = EnumSet.Of(E00, E68);
            set.UnionWith(EnumSet.Of<LargeEnum>());
            Assert.That(set, Is.EqualTo(EnumSet.Of(E00, E68)));
        }

        [Test]
        public void WhenIntersectWithEmpty_Large_ThenSetIsEmpty()
        {
            var set = EnumSet.Of(E00, E68);
            set.IntersectWith(EnumSet.Of<LargeEnum>());
            Assert.That(set, Is.Empty);
        }

        [Test]
        public void WhenExceptWithEmpty_Large_ThenSetUnchanged()
        {
            var set = EnumSet.Of(E00, E68);
            set.ExceptWith(EnumSet.Of<LargeEnum>());
            Assert.That(set, Is.EqualTo(EnumSet.Of(E00, E68)));
        }

        [Test]
        public void WhenSymmetricExceptWithEmpty_Large_ThenSetUnchanged()
        {
            var set = EnumSet.Of(E00, E68);
            set.SymmetricExceptWith(EnumSet.Of<LargeEnum>());
            Assert.That(set, Is.EqualTo(EnumSet.Of(E00, E68)));
        }

        [Test]
        public void WhenDisjointSetsChecked_Large_OverlapsReturnsFalse()
        {
            Assert.That(EnumSet.Of(E00, E68).Overlaps(EnumSet.Of(E11)), Is.False);
        }

        [Test]
        public void WhenDisjointSetsOverBucketBoundaries_Large_OverlapsReturnsFalse()
        {
            Assert.That(EnumSet.Of(E00).Overlaps(EnumSet.Of(E69)), Is.False);
        }

        [Test]
        public void WhenSetEqualsAcrossBuckets_Large_ThenTrue()
        {
            var a = EnumSet.Of(E00, E63, E64, E69);
            var b = EnumSet.Of(E69, E00, E64, E63);
            Assert.That(a.SetEquals(b), Is.True);
        }
    }

    [TestFixture]
    public class GivenAnEnumSetBucketBoundaryBehavior
    {
        [Test]
        public void WhenLargeEnumValueAtBitPosition63Added_ThenContained()
        {
            var set = EnumSet.Of(E63);
            Assert.That(set, Contains.Item(E63));
            Assert.That(set, Has.Count.EqualTo(1));
        }

        [Test]
        public void WhenLargeEnumValueAtBitPosition64Added_ThenContained()
        {
            var set = EnumSet.Of(E64);
            Assert.That(set, Contains.Item(E64));
            Assert.That(set, Has.Count.EqualTo(1));
        }

        [Test]
        public void WhenValuesInBothBucketsAdded_ThenBothContained()
        {
            var set = EnumSet.Of(E63, E64);
            Assert.That(set, Contains.Item(E63));
            Assert.That(set, Contains.Item(E64));
            Assert.That(set, Has.Count.EqualTo(2));
        }

        [Test]
        public void WhenValueFromBucket0Removed_ThenOnlyBucket1Remains()
        {
            var set = EnumSet.Of(E00, E69);
            set.Remove(E00);
            Assert.That(set, Does.Not.Contain(E00));
            Assert.That(set, Contains.Item(E69));
        }

        [Test]
        public void WhenValueFromBucket1Removed_ThenOnlyBucket0Remains()
        {
            var set = EnumSet.Of(E00, E69);
            set.Remove(E69);
            Assert.That(set, Contains.Item(E00));
            Assert.That(set, Does.Not.Contain(E69));
        }

        [Test]
        public void WhenAllLargeEnumValuesAdded_ThenSetEqualsLargeEnumValuesList()
        {
            var all = Enum.GetValues<LargeEnum>();
            var set = EnumSet.Of(all);
            Assert.That(set, Is.EquivalentTo(all));
        }

        [Test]
        public void WhenSymmetricExceptAcrossBuckets_ThenCorrectResult()
        {
            var a = EnumSet.Of(E00, E63, E64);
            a.SymmetricExceptWith(EnumSet.Of(E63, E69));
            Assert.That(a, Is.EqualTo(EnumSet.Of(E00, E64, E69)));
        }

        [Test]
        public void WhenIntersectAcrossBuckets_ThenCorrectResult()
        {
            var a = EnumSet.Of(E00, E63, E64, E69);
            a.IntersectWith(EnumSet.Of(E63, E64));
            Assert.That(a, Is.EqualTo(EnumSet.Of(E63, E64)));
        }

        [Test]
        public void WhenExceptAcrossBuckets_ThenCorrectResult()
        {
            var a = EnumSet.Of(E00, E63, E64, E69);
            a.ExceptWith(EnumSet.Of(E63, E69));
            Assert.That(a, Is.EqualTo(EnumSet.Of(E00, E64)));
        }
    }

    public enum SmallEnum
    {
        A = 9999,
        B = 1,
        C,
        D = 1
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