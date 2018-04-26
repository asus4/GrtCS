using NUnit.Framework;

namespace GRT
{

    public static class RandomTest
    {
        // Tests the basic functionality
        [Test]
        public static void SetSeedTest()
        {
            const uint N = 1000; //Number of iterations in each for loop
            var random = new Random();

            //Set the random seed in a for loop, the random value should be the same every time
            {
                Assert.True(random.SetSeed(1));
                int randomValue = random.GetRandomNumberInt(0, 1000);
                for (uint i = 0; i < N; i++)
                {
                    Assert.True(random.SetSeed(1)); //Set the same seed
                    Assert.AreEqual(random.GetRandomNumberInt(0, 1000), randomValue);
                }
            }

            //Do the same thing, but this time use a different seed everytime
            {
                ulong nextSeed = 0;
                Assert.True(random.SetSeed(++nextSeed));

                int randomValue = random.GetRandomNumberInt(0, 1000);
                float numMatches = 0;
                for (uint i = 0; i < N; i++)
                {
                    Assert.True(random.SetSeed(++nextSeed));
                    if (random.GetRandomNumberInt(0, 1000) == randomValue)
                    {
                        numMatches++;
                    }
                }
                Assert.Less(numMatches, N * 0.5); //There should be much less than 50% of matches
            }

            //Do the same thing, but this time don't change the seed
            {
                Assert.True(random.SetSeed(1));
                int randomValue = random.GetRandomNumberInt(0, 1000);
                float numMatches = 0;
                for (uint i = 0; i < N; i++)
                {
                    if (random.GetRandomNumberInt(0, 1000) == randomValue)
                    {
                        numMatches++;
                    }
                }
                Assert.Less(numMatches, N * 0.5); //There should be much less than 50% of matches
            }
        }

        // Tests the random int functionality
        [Test]
        public static void UniformIntTest()
        {
            var random = new Random();
            Assert.True(random.SetSeed(1));

            const uint N = 10000; //Number of tests
            const int rangeMin = 1000;
            const int rangeMax = 2000;
            const int range = rangeMax - rangeMin;
            VectorFloat histogram = new VectorFloat(range, 0);

            for (uint i = 0; i < N; i++)
            {
                int randomValue = random.GetRandomNumberInt(rangeMin, rangeMax);
                Assert.Less(randomValue, rangeMax);
                Assert.GreaterOrEqual(randomValue, rangeMin);

                int index = randomValue - rangeMin;
                Assert.Less(index, range);
                Assert.GreaterOrEqual(index, 0);

                histogram[index] += 1.0 / N;
            }

            //Compute the histogram min/max difference, the max should be close to the min (as the histogram should be almost equal)
            double maxValue = histogram.MaxValue;
            double minValue = histogram.MinValue;
            double diff = maxValue - minValue;
            Assert.Less(diff, N * 0.01);
        }


    }
}
