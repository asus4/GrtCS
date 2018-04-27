namespace GRT
{
    public class ClassTracker
    {
        public uint classLabel;
        public uint counter;
        public string className;

        public ClassTracker(uint classLabel = 0, uint counter = 0, string className = "NOT_SET")
        {
            this.classLabel = classLabel;
            this.counter = counter;
            this.className = className;
        }

        public ClassTracker(ClassTracker rhs)
        {
            this.classLabel = rhs.classLabel;
            this.counter = rhs.counter;
            this.className = rhs.className;
        }

        public static bool sortByClassLabelDescending(ClassTracker a, ClassTracker b)
        {
            return a.classLabel > b.classLabel;
        }

        public static bool sortByClassLabelAscending(ClassTracker a, ClassTracker b)
        {
            return a.classLabel < b.classLabel;
        }


    }
}
