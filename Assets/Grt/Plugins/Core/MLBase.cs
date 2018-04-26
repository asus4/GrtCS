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
        bool trained;
        bool useScaling;
        bool converged;
        DataType inputType;
        DataType outputType;
        BaseType baseType;
        uint numInputDimensions;
        uint numOutputDimensions;
        uint numTrainingIterationsToConverge;
        uint minNumEpochs;
        uint maxNumEpochs;
        uint batchSize;
        uint validationSetSize;
        uint numRestarts;
        double learningRate;
        double minChange;
        double rmsTrainingError;
        double rmsValidationError;
        double totalSquaredTrainingError;
        double validationSetAccuracy;
        bool useValidationSet;
        bool randomiseTrainingOrder;
        VectorFloat validationSetPrecision;
        VectorFloat validationSetRecall;
        Random random;
        Vector<TrainingResult> trainingResults;

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
