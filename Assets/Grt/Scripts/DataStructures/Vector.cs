using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace GRT
{
    [System.Serializable]
    public class Vector<T> : List<T>
    {
        #region Methods

        /// <summary>
        /// Constructor, sets the size of the vector
        /// </summary>
        /// <param name="size">the size of the vector</param>
        public Vector(uint size = 0) : base((int)size)
        {
            Fill(default(T));
        }

        /// <summary>
        /// Constructor, sets the size of the vector and sets all elements to value
        /// </summary>
        /// <param name="size">the size of the vector</param>
        /// <param name="value">the initial value</param>
        public Vector(uint size, T value) : base((int)size)
        {
            Fill(value);
        }

        public Vector(Vector<T> rhs)
        {
            var arr = new T[rhs.Count];
            rhs.CopyTo(arr);
            AddRange(arr);
        }


        public T this[uint i]
        {
            get { return this[(int)i]; }
            set { this[(int)i] = value; }
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint GetSize() => (uint)Count;


        /// <summary>
        /// Sets all the values in the Vector to the input value
        /// </summary>
        /// <param name="value">the value you want to set all the Vector values to</param>
        /// <returns>returns true or false, indicating if the set was successful </returns>
        public bool Fill(T value)
        {
            if (Capacity == 0)
            {
                return false;
            }
            var array = new T[Capacity];
            for (int i = 0; i < Capacity; ++i)
            {
                array[i] = value;
            }
            Clear();
            AddRange(array);
            return true;
        }

        /// Sets all the values in the Vector to the input value
        /// </summary>
        /// <param name="value">the value you want to set all the Vector values to</param>
        /// <returns>true or false, indicating if the set was successful </returns>
        public bool SetAll(T value)
        {
            return Fill(value);
        }

        public static void Shuffle(IList<T> list, ulong seed = 0)
        {
            int n = list.Count;
            var rnd = new Random(seed);
            while (n > 1)
            {
                int k = (rnd.GetRandomNumberInt(0, n) % n);
                n--;
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public void Shuffle(ulong seed = 0)
        {
            Vector<T>.Shuffle(this, seed);
        }

        #endregion
    }
}
