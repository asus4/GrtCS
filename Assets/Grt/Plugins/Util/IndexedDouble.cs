
namespace GRT
{
    public struct IndexedDouble
    {
        public uint index;
        public double value;

        public IndexedDouble(uint index, double value)
        {
            this.index = index;
            this.value = value;
        }
        public IndexedDouble(ref IndexedDouble rhs)
        {
            this.index = rhs.index;
            this.value = rhs.value;
        }

        public static int SortIndexedDoubleByIndexDescending(IndexedDouble a, IndexedDouble b)
        {
            if (a.index == b.index)
            {
                return 0;
            }
            if (a.index > b.index)
            {
                return 1;
            }
            return -1;
        }

        public static int SortIndexedDoubleByIndexAscending(IndexedDouble a, IndexedDouble b)
        {
            if (a.index == b.index)
            {
                return 0;
            }
            if (a.index < b.index)
            {
                return 1;
            }
            return -1;
        }

        public static int SortIndexedDoubleByValueDescending(IndexedDouble a, IndexedDouble b)
        {
            if (a.value == b.value)
            {
                return 0;
            }
            if (a.value > b.value)
            {
                return 1;
            }
            return -1;
        }

        public static int SortIndexedDoubleByValueAscending(IndexedDouble a, IndexedDouble b)
        {
            if (a.value == b.value)
            {
                return 0;
            }
            if (a.value < b.value)
            {
                return 1;
            }
            return -1;
        }
    }
}
