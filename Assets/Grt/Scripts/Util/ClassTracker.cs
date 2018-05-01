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

        public static int SortByClassLabelDescending(ClassTracker a, ClassTracker b)
        {
            return (int)a.classLabel - (int)b.classLabel;
        }

        public static int SortByClassLabelAscending(ClassTracker a, ClassTracker b)
        {
            return (int)b.classLabel - (int)a.classLabel;
        }


    }
}
