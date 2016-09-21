using NUnit.Framework;

namespace EnumCollections.Tests
{
    [TestFixture]
    public class EnumSetTests
    {
        public enum EmptyEnum
        {
        }

        [Test]
        public void CanCreateEnumSetWithNoElements()
        {
            var enumSet = EnumSet<EmptyEnum>.Of<EmptyEnum>();
            Assert.IsInstanceOf<EnumSet<EmptyEnum>>(enumSet);
        }
    }
}
