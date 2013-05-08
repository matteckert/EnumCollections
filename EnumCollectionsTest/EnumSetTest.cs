using EnumCollections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EnumCollectionsTest
{
    public enum Test
    {
        A, B, C = 1, D, E, F, G, H, I, J
    }

    public enum Bird { BlueJay, Stork, Puffin, SeaParrot = 2, Chicken }

    [TestClass]
    public class EnumSetTest
    {
        [TestMethod]
        public void EnumSet_Of()
        {
            var enumSet = EnumSet<Test>.Of(Test.E, Test.F, Test.A, Test.B);

            Assert.IsTrue(enumSet.Count == 4);
        }

        [TestMethod]
        public void EnumSet_Contains()
        {
            var enumSet = EnumSet<Test>.None();

            enumSet.Add(Test.A);

            Assert.IsTrue(enumSet.Contains(Test.A));
        }

        [TestMethod]
        public void EnumSet_DoesNotContain()
        {
            var enumSet = EnumSet<Test>.None();

            enumSet.Add(Test.A);

            Assert.IsFalse(enumSet.Contains(Test.B));
        }

        [TestMethod]
        public void EnumSet_Alias()
        {
            Assert.IsTrue(EnumSet<Bird>.Of(Bird.Stork, Bird.SeaParrot) == (EnumSet<Bird>.Of(Bird.Stork, Bird.Puffin)));
        }

        [TestMethod]
        public void EnumSet_Equal()
        {
            var a = EnumSet<Test>.None();

            a.Add(Test.A);
            a.Add(Test.B);

            var b = EnumSet<Test>.Of(Test.B, Test.A);

            Assert.IsTrue(a.Equals(b));
        }

        [TestMethod]
        public void EnumSet_NotEqual()
        {
            var a = EnumSet<Test>.None();

            a.Add(Test.A);
            a.Add(Test.B);

            var b = EnumSet<Test>.Of(Test.B);

            Assert.IsFalse(a.Equals(b));
        }

        [TestMethod]
        public void EnumSet_EnumeratorCount()
        {
            var a = EnumSet<Test>.None();

            a.Add(Test.A);
            a.Add(Test.B);
            a.Add(Test.D);

            var i = 0;
            foreach (var e in a)
            {
                i++;
            }

            Assert.IsTrue(i == 3);
        }

        [TestMethod]
        public void EnumSet_UnionWith()
        {
            var expectedResult = EnumSet<Test>.Of(Test.A, Test.F, Test.D);

            var a = EnumSet<Test>.Of(Test.A);

            var b = EnumSet<Test>.Of(Test.F, Test.D);

            a.UnionWith(b);

            Assert.IsTrue(a.Equals(expectedResult));
        }

        [TestMethod]
        public void EnumSet_Accessors()
        {
            var a = EnumSet<Test>.All();

            a[Test.B] = false;
            a[Test.A] = false;
            a[Test.C] = false;

            Assert.IsTrue(a.Count == 7);
        }

        [TestMethod]
        public void Test_IntersectWithIsGood()
        {
            var a = EnumSet<Bird>.Of(Bird.BlueJay, Bird.Chicken, Bird.Puffin);
            var b = EnumSet<Bird>.Of(Bird.BlueJay, Bird.SeaParrot);
            a.IntersectWith(b);

            Assert.IsTrue(a == EnumSet<Bird>.Of(Bird.BlueJay, Bird.Puffin));
        }

        [TestMethod]
        public void Test_ExceptWith()
        {
            var a = EnumSet<Bird>.Of(Bird.BlueJay, Bird.Chicken, Bird.Puffin);
            var b = EnumSet<Bird>.Of(Bird.BlueJay, Bird.SeaParrot, Bird.Stork);
            a.ExceptWith(b);

            Assert.IsTrue(a == EnumSet<Bird>.Of(Bird.Chicken));
        }

        [TestMethod]
        public void Test_SymmetricExceptWith()
        {
            var a = EnumSet<Bird>.Of(Bird.BlueJay, Bird.Chicken, Bird.Puffin);
            var b = EnumSet<Bird>.Of(Bird.BlueJay, Bird.SeaParrot, Bird.Stork);
            a.SymmetricExceptWith(b);

            Assert.IsTrue(a == EnumSet<Bird>.Of(Bird.Stork, Bird.Chicken));
        }

        [TestMethod]
        public void Test_IsSubsetOf()
        {
            var a = EnumSet<Bird>.Of(Bird.BlueJay, Bird.Chicken);
            var b = EnumSet<Bird>.Of(Bird.BlueJay, Bird.SeaParrot, Bird.Chicken);

            Assert.IsTrue(a.IsSubsetOf(b));
        }

        [TestMethod]
        public void Test_SameElementsIsNotProperSubsetOf()
        {
            var a = EnumSet<Bird>.Of(Bird.BlueJay, Bird.Chicken, Bird.Puffin);
            var b = EnumSet<Bird>.Of(Bird.BlueJay, Bird.SeaParrot, Bird.Chicken);

            Assert.IsFalse(a.IsProperSubsetOf(b));
        }

        [TestMethod]
        public void Test_IsProperSubsetOf()
        {
            var a = EnumSet<Bird>.Of(Bird.BlueJay, Bird.Chicken);
            var b = EnumSet<Bird>.Of(Bird.BlueJay, Bird.SeaParrot, Bird.Chicken);

            Assert.IsTrue(a.IsProperSubsetOf(b));
        }

        [TestMethod]
        public void Test_IsProperSupersetOf()
        {
            var a = EnumSet<Bird>.Of(Bird.BlueJay, Bird.Chicken);
            var b = EnumSet<Bird>.Of(Bird.BlueJay, Bird.SeaParrot, Bird.Chicken);

            Assert.IsTrue(b.IsProperSupersetOf(a));
        }

        [TestMethod]
        public void Test_IsSupersetOf()
        {
            var a = EnumSet<Bird>.Of(Bird.BlueJay, Bird.Chicken);
            var b = EnumSet<Bird>.Of(Bird.BlueJay, Bird.SeaParrot, Bird.Chicken);

            Assert.IsTrue(b.IsSupersetOf(a));
        }

        [TestMethod]
        public void Test_Overlaps()
        {
            var a = EnumSet<Bird>.Of(Bird.BlueJay, Bird.Stork);
            var b = EnumSet<Bird>.Of(Bird.BlueJay, Bird.SeaParrot, Bird.Chicken);

            Assert.IsTrue(a.Overlaps(b));
        }
    }
}
