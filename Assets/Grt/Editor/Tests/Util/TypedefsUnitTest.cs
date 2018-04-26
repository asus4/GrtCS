using System;
using NUnit.Framework;

namespace GRT
{
    public static class TypedefsUnitTest
    {
        // Tests the square function
        [Test]
        public static void Sqr()
        {
            double a = 0.0;
            double b = 1.0;
            double c = 2.0;
            double d = -2.0;
            double e = 1000.0;

            double expectedA = 0.0 * 0.0;
            double expectedB = 1.0 * 1.0;
            double expectedC = 2.0 * 2.0;
            double expectedD = -2.0 * -2.0;
            double expectedE = 1000.0 * 1000.0;

            Assert.AreEqual(GRT.Sqr(a), expectedA);
            Assert.AreEqual(GRT.Sqr(b), expectedB);
            Assert.AreEqual(GRT.Sqr(c), expectedC);
            Assert.AreEqual(GRT.Sqr(d), expectedD);
            Assert.AreEqual(GRT.Sqr(e), expectedE);
        }

        // Tests the square root function
        [Test]
        public static void Sqrt()
        {
            double a = 0.0;
            double b = 1.0;
            double c = 2.0;
            double d = 5.0;
            double e = 1000.0;

            double expectedA = Math.Sqrt(a);
            double expectedB = Math.Sqrt(b);
            double expectedC = Math.Sqrt(c);
            double expectedD = Math.Sqrt(d);
            double expectedE = Math.Sqrt(e);

            Assert.AreEqual(GRT.Sqrt(a), expectedA);
            Assert.AreEqual(GRT.Sqrt(b), expectedB);
            Assert.AreEqual(GRT.Sqrt(c), expectedC);
            Assert.AreEqual(GRT.Sqrt(d), expectedD);
            Assert.AreEqual(GRT.Sqrt(e), expectedE);
        }

        // Tests the to string function
        public static void ToStr()
        {
            // Not implemented
        }
    }
}
