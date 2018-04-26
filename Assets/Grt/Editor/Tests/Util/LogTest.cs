using NUnit.Framework;

namespace GRT
{
    public static class LogTest
    {
        [Test]
        public static void IsUnity()
        {
            // Just check at console
            Log.Info("AAA");
        }
    }
}
