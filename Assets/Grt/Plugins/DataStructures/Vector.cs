using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace GRT
{
    public class Vector<T> : List<T>
    {
        #region Methods

        /// <summary>
        /// Constructor, sets the size of the vector and sets all elements to value
        /// </summary>
        /// <param name="size">the size of the vector</param>
        /// <param name="value">the initial value</param>
        /// <returns></returns>
        public Vector(uint size , T value) : base((int)size)
        {
            Fill(value);
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
            if (Count == 0)
            {
                return false;
            }
            for (int i = 0; i < Count; ++i)
            {
                this[i] = value;
            }
            return true;
        }

        #endregion
    }
}
