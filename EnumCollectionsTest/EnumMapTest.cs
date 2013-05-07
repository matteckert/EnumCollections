using EnumCollections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EnumCollectionsTest
{
    [TestClass]
    public class EnumMapTest
    {
        [TestMethod]
        public void EnumMap_Alias()
        {
            var birdMap = new EnumMap<Bird, int>();
            birdMap[Bird.Puffin] = 4;
            birdMap[Bird.SeaParrot] = 5;
            Assert.IsTrue(birdMap[Bird.Puffin] == 5);
        }
    }
}
