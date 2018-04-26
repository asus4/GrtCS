namespace GRT
{
    public class TrainingResult
    {
        public enum TrainingMode { CLASSIFICATION = 0, REGRESSION };

        #region Properies
        protected TrainingMode trainingMode;
        protected uint trainingIteration;
        protected double accuracy;
        protected double totalSquaredTrainingError;
        protected double rootMeanSquaredTrainingError;
        MLBase trainer;

        #endregion

        #region Methods

        /// <summary>
        /// Default Constructor.
        /// Initializes the TrainingResult instance.
        /// </summary>
        public TrainingResult()
        {
            trainingMode = TrainingMode.CLASSIFICATION;
            trainingIteration = 0;
            accuracy = 0;
            totalSquaredTrainingError = 0;
            rootMeanSquaredTrainingError = 0;
            trainer = null;
        }

        public void Clone(TrainingResult rhs)
        {
            if (this != rhs)
            {
                this.trainingMode = rhs.trainingMode;
                this.trainingIteration = rhs.trainingIteration;
                this.accuracy = rhs.accuracy;
                this.totalSquaredTrainingError = rhs.totalSquaredTrainingError;
                this.rootMeanSquaredTrainingError = rhs.rootMeanSquaredTrainingError;
                this.trainer = rhs.trainer;
            }
        }

        /// <summary>
        /// Gets the current training mode, this will be one of the TrainingMode enums.
        /// </summary>
        public TrainingMode GetTrainingMode => trainingMode;

        /// <summary>
        /// Gets the training iteration, this represents which iteration (or epoch) the training results correspond to.
        /// </summary>
        public uint TrainingIteration => trainingIteration;

        /// <summary>
        /// Gets the accuracy for the training result at the current training iteration. This is only used if the trainingMode is in CLASSIFICATION_MODE.
        /// </summary>
        public double Accuracy => accuracy;

        /// <summary>
        /// Gets the total squared error for the training data at the current training iteration. This is only used if the trainingMode is in REGRESSION_MODE.
        /// </summary>
        public double TotalSquaredTrainingError => totalSquaredTrainingError;

        /// <summary>
        /// Gets the root mean squared error for the training data at the current training iteration. This is only used if the trainingMode is in REGRESSION_MODE.
        /// </summary>
        public double RootMeanSquaredTrainingError => rootMeanSquaredTrainingError;


        /// <summary>
        /// Gets a pointer to the class used for training.
        /// </summary>
        public MLBase Trainer => trainer;


        /// <summary>
        /// Sets the training result for classification data. This will place the training mode into CLASSIFICATION_MODE.
        /// </summary>
        /// <param name="trainingIteration">the current training iteration (or epoch)</param>
        /// <param name="accuracy">the accuracy for the current training iteration</param>
        /// <param name="trainer">a pointer to the class used to generate the result</param>
        /// <returns>returns true if the training result was set successfully</returns>
        public bool setClassificationResult(uint trainingIteration, double accuracy, MLBase trainer)
        {
            this.trainingMode = TrainingMode.CLASSIFICATION;
            this.trainingIteration = trainingIteration;
            this.accuracy = accuracy;
            this.trainer = trainer;
            return true;
        }

        #endregion
    }
}
