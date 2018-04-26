using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace GRT
{
    public class Random
    {
        public Random(ulong seed = 0)
        {
            SetSeed(seed);
        }

        /// <summary>
        /// Sets the current seed used to compute the random distrubutions.
        /// </summary>
        /// <param name="seed">sets the current seed, If no seed is supplied then the seed will be set using the current system time</param>
        /// <returns>true if the seed was set successfully, false otherwise</returns>
        public bool SetSeed(ulong seed_ = 0)
        {
            ulong seed = seed_;
            if (seed == 0)
            {
                seed = (ulong)DateTime.Now.Ticks;
            }
            v = 4101842887655102017;
            w = 1;
            storedval = 0.0;
            u = seed ^ v; int64();
            v = u; int64();
            w = v; int64();
            return true;
        }

        /// <summary>
        /// Gets a random integer in the range [minRange maxRange-1], using a uniform distribution
        /// </summary>
        /// <param name="minRange">the minimum value in the range (inclusive)</param>
        /// <param name="maxRange">the maximum value in the range (not inclusive)</param>
        /// <returns>an integer in the range [minRange maxRange-1]</returns>
        public int GetRandomNumberInt(int minRange, int maxRange)
        {
            return (int)(Math.Floor(GetRandomNumberUniform(minRange, maxRange)));
        }

        /// <summary>
        /// Gets a random integer from the Vector values. The probability of choosing a specific integer from the
        /// values Vector is given by the corresponding weight in the weights Vector. The size of the values
        /// Vector must match the size of the weights Vector. The weights do not need to sum to 1.
        ///
        /// For example, if the input values are: [1 2 3] and weights are: [0.7 0.2 0.1], then the 1 value would
        /// be randomly returned 70% of the time, the 2 value returned 20% of the time and the 3 value returned
        /// 10% of the time.
        /// </summary>
        /// <param name="values">values: a Vector containing the N possible values the function can return</param>
        /// <param name="weights">weights: the corresponding weights for the values Vector (must be the same size as the values Vector)</param>
        /// <returns>a random integer from the values Vector, with a probability relative to the values weight</returns>
        public int GetRandomNumberWeighted(Vector<int> values, VectorFloat weights)
        {
            if (values.Count != weights.Count) { return 0; }

            uint N = (uint)values.Count;
            var weightedValues = new Vector<IndexedDouble>(N);
            for (int i = 0; i < N; i++)
            {
                weightedValues[i] = new IndexedDouble((uint)values[i], weights[i]);
            }

            return GetRandomNumberWeighted(weightedValues);
        }

        /// <summary>
        /// Gets a random integer from the input Vector. The probability of choosing a specific integer is given by the
        /// corresponding weight of that value. The weights do not need to sum to 1.
        ///
        /// For example, if the input values are: [{1 0.7},{2 0.2}, {3 0.1}], then the 1 value would be randomly returned
        /// 70% of the time, the 2 value returned 20% of the time and the 3 value returned 10% of the time.

        /// </summary>
        /// <param name="weightedValues">a Vector of IndexedDouble values, the (int) indexs represent the value that will be returned while the (Float) values represent the weight of choosing that specific index</param>
        /// <returns>a random integer from the values Vector, with a probability relative to the values weight</returns>
        public int GetRandomNumberWeighted(Vector<IndexedDouble> weightedValues)
        {
            uint N = (uint)weightedValues.Count;

            if (N == 0) return 0;
            if (N == 1) return (int)weightedValues[0].index;


            //Sort the weighted values by value in ascending order (so the least likely value is first, the second most likely is second, etc...
            weightedValues.Sort(IndexedDouble.SortIndexedDoubleByValueAscending);


            //Create the accumulated sum lookup table
            var x = new Vector<double>(N);
            x[0] = weightedValues[0].value;
            for (int i = 1; i < N; ++i)
            {
                x[i] = x[i - 1] + weightedValues[i].value;
            }

            //Generate a random value between min and the max weighted Float values
            double randValue = GetUniform(0.0, x[(int)N - 1]);

            //Find which bin the rand value falls into, return the index of that bin
            for (int i = 0; i < N; ++i)
            {
                if (randValue <= x[i])
                {
                    return (int)weightedValues[i].index;
                }
            }
            return 0;
        }

        /// <summary>
        /// This function is similar to the getRandomNumberWeighted(Vector< IndexedDouble > weightedValues), with the exception that the user needs
        /// to sort the weightedValues Vector and create the accumulated lookup table (x). This is useful if you need to call the same function
        /// multiple times on the same weightedValues, allowing you to only sort and build the loopup table once.
        ///
        /// Gets a random integer from the input Vector. The probability of choosing a specific integer is given by the
        /// corresponding weight of that value. The weights do not need to sum to 1.
        ///
        /// For example, if the input values are: [{1 0.7},{2 0.2}, {3 0.1}], then the 1 value would be randomly returned
        /// 70% of the time, the 2 value returned 20% of the time and the 3 value returned 10% of the time.
        /// </summary>
        /// <param name="weightedValues">a sorted Vector of IndexedDouble values, the (int) indexs represent the value that will be returned while the (Float) values represent the weight of choosing that specific index</param>
        /// <param name="x">a Vector containing the accumulated lookup table</param>
        /// <returns>a random integer from the values Vector, with a probability relative to the values weight</returns>
        public int GetRandomNumberWeighted(Vector<IndexedDouble> weightedValues, VectorFloat x)
        {
            uint N = (uint)weightedValues.Count;

            if (weightedValues.Count != x.Count) { return 0; }

            //Generate a random value between min and the max weighted Float values
            double randValue = GetUniform(0.0, x[(int)N - 1]);

            //Find which bin the rand value falls into, return the index of that bin
            for (int i = 0; i < N; i++)
            {
                if (randValue <= x[i])
                {
                    return (int)weightedValues[i].index;
                }
            }
            return 0;
        }

        /// <summary>
        /// Gets a random Float in the range [minRange maxRange], using a uniform distribution
        /// </summary>
        /// <param name="minRange">the minimum value in the range (inclusive)</param>
        /// <param name="maxRange">the maximum value in the range (inclusive)</param>
        /// <returns>a Float in the range [minRange maxRange]</returns>
        public double GetRandomNumberUniform(double minRange = 0.0, double maxRange = 1.0)
        {
            return GetUniform(minRange, maxRange);
        }

        /// <summary>
        /// Gets a random Float in the range [minRange maxRange], using a uniform distribution
        /// </summary>
        /// <param name="minRange">the minimum value in the range (inclusive)</param>
        /// <param name="maxRange">the maximum value in the range (inclusive)</param>
        /// <returns>a Float in the range [minRange maxRange]</returns>
        public double GetUniform(double minRange = 0.0, double maxRange = 1.0)
        {
            double r = doub();
            return (r * (maxRange - minRange)) + minRange;
        }

        /// <summary>
        /// Gets a random Float, using a Gaussian distribution with mu 0 and sigma 1.0
        /// </summary>
        /// <param name="mu">the mu parameter for the Gaussian distribution</param>
        /// <param name="sigma">the sigma parameter for the Gaussian distribution</param>
        /// <returns>a Float from the Gaussian distribution controlled by mu and sigma</returns>
        public double GetRandomNumberGauss(double mu = 0.0, double sigma = 1.0)
        {
            return GetGauss(mu, sigma);
        }

        /// <summary>
        /// Gets a random Float, using a Gaussian distribution with mu 0 and sigma 1.0
        /// </summary>
        /// <param name="mu">the mu parameter for the Gaussian distribution</param>
        /// <param name="sigma">the sigma parameter for the Gaussian distribution</param>
        /// <returns>a Float from the Gaussian distribution controlled by mu and sigma</returns>
        double GetGauss(double mu = 0.0, double sigma = 1.0)
        {
            double v1, v2, rsq, fac;

            if (storedval == 0.0)
            {
                do
                {
                    v1 = 2.0 * doub() - 1.0;
                    v2 = 2.0 * doub() - 1.0;
                    rsq = v1 * v1 + v2 * v2;
                } while (rsq >= 1.0 || rsq == 0.0);

                fac = GRT.Sqrt(-2.0 * GRT.Log(rsq) / rsq);
                storedval = v1 * fac;
                return mu + sigma * v2 * fac;
            }
            else
            {
                fac = storedval;
                storedval = 0.0;
                return mu + sigma * fac;
            }
        }

        /// <summary>
        /// Gets an N-dimensional Vector of random Floats drawn from the uniform distribution set by the minRange and maxRange.
        /// </summary>
        /// <param name="numDimensions">the size of the Vector you require</param>
        /// <param name="minRange">the minimum value in the range (inclusive)</param>
        /// <param name="maxRange">the maximum value in the range (inclusive)</param>
        /// <returns>a Vector of Floats drawn from the uniform distribution set by the minRange and maxRange</returns>
        VectorFloat GetRandomVectorUniform(uint numDimensions, double minRange = 0.0, double maxRange = 1.0)
        {
            var randomValues = new VectorFloat(numDimensions);
            for (int i = 0; i < numDimensions; i++)
            {
                randomValues[i] = GetRandomNumberUniform(minRange, maxRange);
            }
            return randomValues;
        }

        /// <summary>
        /// Gets an N-dimensional Vector of random Floats drawn from the Gaussian distribution controlled by mu and sigma.
        /// </summary>
        /// <param name="numDimensions">the size of the Vector you require</param>
        /// <param name="mu">the mu parameter for the Gaussian distribution</param>
        /// <param name="sigma">the sigma parameter for the Gaussian distribution</param>
        /// <returns>a Vector of Floats drawn from the Gaussian distribution controlled by mu and sigma</returns>
        VectorFloat GetRandomVectorGauss(uint numDimensions, double mu = 0.0, double sigma = 1.0)
        {
            var randomValues = new VectorFloat(numDimensions);
            for (int i = 0; i < numDimensions; i++)
            {
                randomValues[i] = GetRandomNumberGauss(mu, sigma);
            }
            return randomValues;
        }

        /// <summary>
        /// Gets an N-dimensional Vector of random unsigned ints drawn from the range controlled by the start and end range parameters.
        /// </summary>
        /// <param name="startRange">indicates the start of the range the random subset will selected from (e.g. 0)</param>
        /// <param name="endRange">indicates the end of the range the random subset will selected from (e.g. 100)</param>
        /// <param name="subsetSize">controls the size of the Vector returned by the function (e.g. 50)</param>
        /// <returns>a Vector of unsigned ints selected from the</returns>
        Vector<uint> GetRandomSubset(uint startRange, uint endRange, uint subsetSize)
        {
            uint rangeSize = endRange - startRange;

            Debug.Assert(rangeSize > 0);
            Debug.Assert(endRange > startRange);
            Debug.Assert(subsetSize <= rangeSize);

            var indexs = new Vector<uint>(rangeSize);
            var subset = new Vector<uint>(subsetSize);

            //Fill up the range buffer and the randomly suffle it
            for (uint i = startRange; i < endRange; i++)
            {
                indexs[(int)i] = i;
            }
            indexs.Shuffle();

            //Select the first X values from the randomly shuffled range buffer as the subset
            for (int i = 0; i < subsetSize; i++)
            {
                subset[i] = indexs[i];
            }
            return subset;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        ulong int64()
        {
            u = u * 2862933555777941757 + 7046029254386353087;
            v ^= v >> 17; v ^= v << 31; v ^= v >> 8;
            w = 4294957665U * (w & 0xffffffff) + (w >> 32);
            ulong x = u ^ (u << 21); x ^= x >> 35; x ^= x << 4;
            return (x + v) ^ w;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        double doub() { return 5.42101086242752217E-20 * int64(); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        uint int32() { return (uint)int64(); }

        private ulong u;
        private ulong v;
        private ulong w;
        double storedval;  //This is for the Gauss Box-Muller
    }
}
