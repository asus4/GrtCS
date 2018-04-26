namespace GRT
{
    /// <summary>
    /// This is the main base class that all GRT machine learning algorithms should inherit from.
    ///
    /// A large number of the functions in this class are virtual and simply return false as these functions must be overwridden by the inheriting class.
    /// </summary>
    public class MLBase : GRTBase, IObserver<TrainingResult>, IObserver<TestInstanceResult>
    {
        /// <summary>
        /// Enum that defines the type of inherited class
        /// </summary>
        public enum BaseType
        {
            BASE_TYPE_NOT_SET = 0,
            CLASSIFIER,
            REGRESSIFIER,
            CLUSTERER,
            PRE_PROCSSING,
            POST_PROCESSING,
            FEATURE_EXTRACTION,
            CONTEXT
        };

        #region Properties
        protected bool trained;
        protected bool useScaling;
        protected bool converged;
        protected DataType inputType;
        protected DataType outputType;
        protected BaseType baseType;
        protected uint numInputDimensions;
        protected uint numOutputDimensions;
        protected uint numTrainingIterationsToConverge;
        protected uint minNumEpochs;
        protected uint maxNumEpochs;
        protected uint batchSize;
        protected uint validationSetSize;
        protected uint numRestarts;
        protected double learningRate;
        protected double minChange;
        protected double rmsTrainingError;
        protected double rmsValidationError;
        protected double totalSquaredTrainingError;
        protected double validationSetAccuracy;
        protected bool useValidationSet;
        protected bool randomiseTrainingOrder;
        protected VectorFloat validationSetPrecision;
        protected VectorFloat validationSetRecall;
        protected Random random;
        protected Vector<TrainingResult> trainingResults;

        #endregion

        /// <summary>
        /// Default MLBase Constructor
        /// </summary>
        /// <param name="id">the id of the inheriting class</param>
        /// <param name="type">the type of the inheriting class (e.g., classifier, regressifier, etc.)</param>
        public MLBase(string id = "", BaseType type = BaseType.BASE_TYPE_NOT_SET)
        {
        }


        /// <summary>
        ///This copies all the MLBase variables from the instance mlBaseA to the instance mlBaseA.
        /// </summary>
        /// <param name="mlBase">a pointer to a MLBase class from which the values will be copied to the instance that calls the function</param>
        /// <returns>returns true if the copy was successfull, false otherwise</returns>
        public bool CopyMLBaseVariables(MLBase mlBase)
        {
            return true;
        }



        #region Interface implementation
        public void Notify(TrainingResult result)
        {

        }

        public void Notify(TestInstanceResult result)
        {

        }
        #endregion // Interface implementation

    }
}
