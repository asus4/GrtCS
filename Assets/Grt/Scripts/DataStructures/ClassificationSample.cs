using System.Runtime.CompilerServices;

namespace GRT
{
    public class ClassificationSample
    {
        #region Properties
        protected uint numDimensions;
        protected uint classLabel;
        protected VectorFloat sample;
        #endregion

        #region Methods
        public ClassificationSample()
        {
            numDimensions = 0;
            classLabel = 0;
            sample = new VectorFloat(0);
        }
        public ClassificationSample(uint numDimensions)
        {
            this.numDimensions = numDimensions;
            classLabel = 0;
            sample = new VectorFloat(numDimensions);
        }
        public ClassificationSample(uint classLabel, VectorFloat sample)
        {
            this.classLabel = classLabel;
            this.sample = sample;
            this.numDimensions = (uint)sample.Count;
        }

        public ClassificationSample(ClassificationSample rhs)
        {
            this.classLabel = rhs.classLabel;
            this.sample = new VectorFloat(rhs.sample);
            this.numDimensions = rhs.numDimensions;
        }

        ~ClassificationSample()
        {
            Clear();
        }


        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double this[uint n]
        {
            get { return sample[(int)n]; }
            set { sample[(int)n] = value; }
        }

        public double this[int n]
        {
            get { return sample[n]; }
            set { sample[n] = value; }
        }


        public bool Clear()
        {
            numDimensions = 0;
            classLabel = 0;
            sample.Clear();
            return true;
        }

        public uint NumDimensions => numDimensions;
        public uint ClassLabel
        {
            get { return classLabel; }
            set { classLabel = value; }
        }
        public VectorFloat Sample
        {
            get { return sample; }
            set
            {
                sample = new VectorFloat(sample); // Clones
                numDimensions = (uint)sample.Count;
            }
        }

        public bool Set(uint classLabel, VectorFloat sample)
        {
            this.classLabel = classLabel;
            this.sample = sample;
            this.numDimensions = (uint)sample.Count;
            return true;
        }

        #endregion

    }
}
