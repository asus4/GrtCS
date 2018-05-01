using System;
using System.IO;
using NUnit.Framework;

namespace GRT
{
    public static class VectorFloatUnitTest
    {
        [Test]
        public static void DefaultConstructor()
        {
            var vec = new VectorFloat();
            Assert.AreEqual(0, vec.GetSize());
        }

        // Tests the resize c'tor.
        [Test]
        public static void ResizeConstructor()
        {
            const uint size = 100;
            var vec = new VectorFloat(size);
            Assert.AreEqual(size, vec.GetSize());
        }

        // Tests the copy c'tor.
        [Test]
        public static void CopyConstructor()
        {
            const uint size = 100;
            var vec1 = new VectorFloat(size);
            Assert.AreEqual(size, vec1.GetSize());
            var vec2 = new VectorFloat(vec1);
            Assert.AreEqual(vec1.GetSize(), vec2.GetSize());
        }

        // Tests the equals operator.
        public static void EqualsConstructor()
        {
            // Not implemnted
        }

        // Tests the Vector< Float > equals operator.
        public static void VecFloatEqualsConstructor()
        {
            // Not implemnted
        }

        // Tests the save and load methods.
        [Test]
        public static void SaveLoad()
        {
            const uint size = 100;
            var vec1 = new VectorFloat(size);
            Assert.AreEqual(size, vec1.GetSize());
            for (int i = 0; i < size; i++) { vec1[i] = i * 1.0; }

            const string fileName = "vec.csv";

            //Save the vector to a CSV file
            Assert.True(vec1.Save(fileName));

            //Load the data from the file into another vector
            VectorFloat vec2 = new VectorFloat();
            Assert.True(vec2.Load(fileName));

            //Vector 2 should now be the same size as vector 1
            Assert.AreEqual(vec1.GetSize(), vec2.GetSize());

            //Check to make sure the values match
            for (int i = 0; i < size; i++) { Assert.AreEqual(vec1[i], vec2[i]); }

            // Cleanup file
            File.Delete(fileName);
        }

        // Tests the scale function
        [Test]
        public static void Scale()
        {
            const uint size = 100;
            var vec = new VectorFloat(size);
            Assert.AreEqual(size, vec.GetSize());
            for (int i = 0; i < size; i++) { vec[i] = i * 1.0; }

            //Scale the contents in the vector
            const double minTarget = -1.0f;
            const double maxTarget = 1.0f;
            Assert.True(vec.Scale(minTarget, maxTarget));


            Assert.AreEqual(vec.MinValue, minTarget);
            Assert.AreEqual(vec.MaxValue, maxTarget);
        }

        // Tests the min-max scale function
        [Test]
        public static void MinMaxScale()
        {
            const uint size = 100;
            var vec = new VectorFloat(size);
            Assert.AreEqual(size, vec.GetSize());
            for (int i = 0; i < size; i++) { vec[i] = i * 1.0; }

            //Scale the contents in the vector
            double minSource = 10; //Deliberately set the min source to 10, even though the min value should be 0
            double maxSource = 90; //Deliberately set the max source to 90, even though the max value should be 100
            double minTarget = -1.0f;
            double maxTarget = 1.0f;
            Assert.True(vec.Scale(minSource, maxSource, minTarget, maxTarget, true));
            Assert.AreEqual(vec.MinValue, minTarget);
            Assert.AreEqual(vec.MaxValue, maxTarget);

            //Reset the data
            for (int i = 0; i < size; i++) { vec[i] = i * 1.0; }
            Assert.True(vec.Scale(minSource, maxSource, minTarget, maxTarget, false)); //Re-run the scaling with constraining off
            Assert.True(Math.Abs(vec.MinValue - minTarget) > 0.01); //The min value should no longer match the min target
            Assert.True(Math.Abs(vec.MaxValue - maxTarget) > 0.01); //The max value should no longer match the max target
        }

        // Tests the MinValue       [Test]
        public static void GetMin()
        {
            const uint size = 100;
            var vec = new VectorFloat(size);
            Assert.AreEqual(size, vec.GetSize());
            for (int i = 0; i < size; i++) { vec[i] = i * 1.0; }
            Assert.AreEqual(vec.MinValue, 0.0f);
        }

        // Tests the MaxValue       [Test]
        public static void GetMax()
        {
            const uint size = 100;
            var vec = new VectorFloat(size);
            Assert.AreEqual(size, vec.GetSize());
            for (int i = 0; i < size; i++) { vec[i] = i * 1.0; }
            Assert.AreEqual(vec.MaxValue, 99.0f);
        }

        // Tests the getMean
        [Test]
        public static void GetMean()
        {
            const uint size = 100;
            var vec = new VectorFloat(size);
            Assert.AreEqual(size, vec.GetSize());
            for (int i = 0; i < size; i++) { vec[i] = i * 1.0; }
            double mean = 0.0;
            for (int i = 0; i < size; i++)
            {
                mean += vec[i];
            }
            mean /= size;
            Assert.AreEqual(vec.Mean, mean);
        }

        // Tests the getStdDev
        [Test]
        public static void GetStdDev()
        {
            const uint size = 100;
            var vec = new VectorFloat(size);
            Assert.AreEqual(size, vec.GetSize());
            for (int i = 0; i < size; i++) { vec[i] = i * 1.0; }
            double mean = 0.0;
            double stddev = 0.0;
            for (int i = 0; i < size; i++)
            {
                mean += vec[i];
            }
            mean /= size;
            for (int i = 0; i < size; i++)
            {
                stddev += GRT.Sqr(vec[i] - mean);
            }
            stddev = GRT.Sqrt(stddev / (size - 1));
            Assert.AreEqual(vec.StdDev, stddev);
        }

        // Tests the getMinMax
        [Test]
        public static void GetMinMax()
        {
            const uint size = 100;
            var vec = new VectorFloat(size);
            Assert.AreEqual(size, vec.GetSize());
            for (int i = 0; i < size; i++) { vec[i] = i * 1.0; }
            const double expectedMin = 0.0;
            const double expectedMax = size - 1;
            MinMax result = vec.MinMax;
            Assert.AreEqual(expectedMin, result.minValue);
            Assert.AreEqual(expectedMax, result.maxValue);
        }

    }
}
