using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

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
