namespace GRT
{
    public class RegressionSample
    {
        VectorFloat inputVector;
        VectorFloat targetVector;

        public RegressionSample()
        {
            inputVector = new VectorFloat();
            targetVector = new VectorFloat();
        }
        public RegressionSample(VectorFloat inputVector, VectorFloat targetVector)
        {
            Set(inputVector, targetVector);
        }
        public RegressionSample(RegressionSample rhs)
        {
            Set(rhs.inputVector, rhs.targetVector);
        }

        ~RegressionSample()
        {
            Clear();
        }

        public static bool sortByInputVectorAscending(RegressionSample a, RegressionSample b)
        {
            return a.inputVector.Count < b.inputVector.Count;
        }

        public static bool sortByInputVectorDescending(RegressionSample a, RegressionSample b)
        {
            return a.inputVector.Count > b.inputVector.Count;
        }

        public void Clear()
        {
            inputVector.Clear();
            targetVector.Clear();
        }

        public void Set(VectorFloat inputVector, VectorFloat targetVector)
        {
            this.inputVector = new VectorFloat(inputVector);
            this.targetVector = new VectorFloat(targetVector);
        }

        public uint GetNumInputDimensions() => inputVector.GetSize();
        public uint GetNumTargetDimensions() => targetVector.GetSize();
        public double GetInputVectorValue(uint index)
        {
            return (index < inputVector.GetSize()) ? inputVector[index] : 0;
        }
        public double GetTargetVectorValue(uint index)
        {
            return (index < targetVector.GetSize()) ? targetVector[index] : 0;
        }
        public VectorFloat InputVector => inputVector;
        public VectorFloat TargetVector => targetVector;
    }
}
