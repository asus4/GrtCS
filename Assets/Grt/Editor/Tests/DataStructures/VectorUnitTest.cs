using NUnit.Framework;

namespace GRT
{
    public static class VectorUnitTest
    {
        [Test]
        public static void DefaultConstructor()
        {
            var vec = new Vector<int>();
            Assert.AreEqual(0, vec.GetSize());
        }

        [Test]
        public static void ResizeConstructor()
        {
            const uint size = 100;
            var vec = new Vector<int>(size);
            Assert.AreEqual(size, vec.GetSize());
        }

        [Test]
        public static void CopyConstructor()
        {
            const uint size = 100;
            var vec1 = new Vector<int>(size);
            Assert.AreEqual(size, vec1.GetSize());

            var vec2 = new Vector<int>(vec1);
            Assert.AreEqual(vec1.GetSize(), vec2.GetSize());
        }

        public static void EqualsConstructor()
        {
            // Not implemnted since C# can't have "=" overload
        }
    }
}
