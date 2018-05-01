using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace GRT
{
    [System.Serializable]
    public class RegressionData
    {
        string datasetName;                                     ///< The name of the dataset
        string infoText;                                        ///< Some infoText about the dataset
        uint numInputDimensions;                                ///< The number of input dimensions in the dataset
        uint numTargetDimensions;                               ///< The number of target dimensions in the dataset
        uint totalNumSamples;                                   ///< The total number of training samples in the dataset
        uint kFoldValue;                                        ///< The number of folds the dataset has been spilt into for cross valiation
        bool crossValidationSetup;                              ///< A flag to show if the dataset is ready for cross validation
        bool useExternalRanges;                                 ///< A flag to show if the dataset should be scaled using the externalRanges values
        Vector<MinMax> externalInputRanges;                   ///< A Vector containing a set of externalRanges set by the user
        Vector<MinMax> externalTargetRanges;                  ///< A Vector containing a set of externalRanges set by the user
        Vector<RegressionSample> data;                        ///< The regression data
        Vector<Vector<uint>> crossValidationIndexs;      ///< A Vector to hold the indexs of the dataset for the cross validation


        /// <summary>
        /// Constructor, set the number of input dimensions, number of target dimensions, dataset name and the infotext for the dataset.
        /// The name of the dataset should not contain any spaces.
        /// </summary>
        /// <param name="numInputDimensions">the number of input dimensions of the training data, should be an unsigned integer greater than 0</param>
        /// <param name="numTargetDimensions">the number of target dimensions of the training data, should be an unsigned integer greater than 0</param>
        /// <param name="datasetName">the name of the dataset, should not contain any spaces</param>
        /// <param name="infoText">some info about the data in this dataset, this can contain spaces</param>
        public RegressionData(uint numInputDimensions = 0, uint numTargetDimensions = 0, string datasetName = "NOT_SET", string infoText = "")
        {
            this.numInputDimensions = numInputDimensions;
            this.numTargetDimensions = numTargetDimensions;
            this.datasetName = datasetName;
            this.infoText = infoText;
            kFoldValue = 0;
            crossValidationSetup = false;
            useExternalRanges = false;
        }


        /// <summary>
        /// Copy Constructor, copies the RegressionData from the rhs instance to this instance
        /// </summary>
        /// <param name="rhs">another instance of the RegressionData class from which the data will be copied to this instance</param>
        public RegressionData(RegressionData rhs)
        {
            Clone(rhs);
        }

        /// Default Destructor
        ~RegressionData()
        {
            Clear();
        }

        public void Clone(RegressionData rhs)
        {
            this.datasetName = rhs.datasetName;
            this.infoText = rhs.infoText;
            this.numInputDimensions = rhs.numInputDimensions;
            this.numTargetDimensions = rhs.numTargetDimensions;
            this.totalNumSamples = rhs.totalNumSamples;
            this.kFoldValue = rhs.kFoldValue;
            this.crossValidationSetup = rhs.crossValidationSetup;
            this.useExternalRanges = rhs.useExternalRanges;
            this.externalInputRanges = new Vector<MinMax>(rhs.externalInputRanges);
            this.externalTargetRanges = new Vector<MinMax>(rhs.externalTargetRanges);
            this.data = new Vector<RegressionSample>(rhs.data);
            this.crossValidationIndexs = new Vector<Vector<uint>>();
        }

        /// <summary>
        /// Array Subscript Operator, returns the LabelledRegressionSample at index i.
        /// It is up to the user to ensure that i is within the range of [0 totalNumSamples-1]
        /// </summary>
        /// <returns>a reference to the i'th RegressionSample</returns>
        public RegressionSample this[uint i]
        {
            get { return data[i]; }
            set { data[i] = value; }
        }

        /// <summary>
        /// Clears any previous training data and counters
        /// </summary>
        void Clear()
        {
            totalNumSamples = 0;
            kFoldValue = 0;
            crossValidationSetup = false;
            data.Clear();
            crossValidationIndexs.Clear();
        }

        /// <summary>
        /// Sets the number of input and target dimensions in the training data.
        /// These should be unsigned integers greater than zero.
        /// This will clear any previous training data and counters.
        /// This function needs to be called before any new samples can be added to the dataset, unless the numInputDimensions
        /// and numTargetDimensions variables was set in the constructor or some data was already loaded from a file
        /// </summary>
        /// <param name="numInputDimensions">numInputDimensions: the number of input dimensions of the training data.  Must be an unsigned integer greater than zero</param>
        /// <param name="numTargetDimensions">numTargetDimensions: the number of target dimensions of the training data.  Must be an unsigned integer greater than zero</param>
        /// <returns>true if the number of input and target dimensions was correctly updated, false otherwise</returns>
        bool SetInputAndTargetDimensions(uint numInputDimensions, uint numTargetDimensions)
        {
            Clear();
            if (numInputDimensions > 0 && numTargetDimensions > 0)
            {
                this.numInputDimensions = numInputDimensions;
                this.numTargetDimensions = numTargetDimensions;

                //Clear the external ranges
                useExternalRanges = false;
                externalInputRanges.Clear();
                externalTargetRanges.Clear();
                return true;
            }
            Log.Error("setInputAndTargetDimensions(UINT numInputDimensions,UINT numTargetDimensions) - The number of input and target dimensions should be greater than zero!");
            return false;
        }

        /// <summary>
        /// Sets the name of the dataset.
        /// There should not be any spaces in the name.
        /// Will return true if the name is set, or false otherwise.
        /// </summary>
        /// <param name="datasetName">the new dataset name (must not include any spaces)</param>
        /// <returns>true if the name is set, or false otherwise</returns>
        bool SetDatasetName(string datasetName)
        {
            //Make sure there are no spaces in the string
            if (string.IsNullOrWhiteSpace(datasetName))
            {
                Log.Error("setDatasetName(const string &datasetName) - The dataset name cannot contain any spaces!");
                return false;
            }

            this.datasetName = datasetName;
            return true;
        }

        /// <summary>
        /// Sets the info string.
        /// This can be any string with information about how the training data was recorded for example.
        /// </summary>
        /// <param name="infoText">the infoText</param>
        /// <returns>true if the infoText was correctly updated, false otherwise</returns>
        bool SetInfoText(string infoText)
        {
            this.infoText = infoText;
            return true;
        }

        /// <summary>
        /// Adds a new labelled sample to the dataset.
        ///  The input and target dimensionality of the sample should match that of the dataset.
        /// </summary>
        /// <param name="inputVector">the new input Vector you want to add to the dataset.  The dimensionality of this sample should match the number of input dimensions in the dataset</param>
        /// <param name="targetVector">the new target Vector you want to add to the dataset.  The dimensionality of this sample should match the number of target dimensions in the dataset</param>
        /// <returns>true if the sample was correctly added to the dataset, false otherwise</returns>
        bool AddSample(VectorFloat inputVector, VectorFloat targetVector)
        {
            if (inputVector.GetSize() == numInputDimensions && targetVector.GetSize() == numTargetDimensions)
            {
                data.Add(new RegressionSample(inputVector, targetVector));
                totalNumSamples++;

                //The dataset has changed so flag that any previous cross validation setup will now not work
                crossValidationSetup = false;
                crossValidationIndexs.Clear();
                return true;
            }
            Log.Error("addSample(const VectorFloat &inputVector,const VectorFloat &targetVector) - The inputVector size or targetVector size does not match the size of the numInputDimensions or numTargetDimensions");
            return false;
        }


        /// <summary>
        /// Removes the last training sample added to the dataset.
        /// </summary>
        /// <returns>true if the last sample was removed, false otherwise</returns>
        bool RemoveLastSample()
        {
            if (totalNumSamples > 0)
            {
                //Remove the training example from the buffer
                data.RemoveAt(data.Count - 1);
                totalNumSamples = data.GetSize();

                //The dataset has changed so flag that any previous cross validation setup will now not work
                crossValidationSetup = false;
                crossValidationIndexs.Clear();
                return true;
            }
            Log.Warning("removeLastSample() - There are no samples to remove!");
            return false;
        }

        /// <summary>
        /// Reserves that the Vector capacity be at least enough to contain N elements.
        /// If N is greater than the current Vector capacity, the function causes the container to reallocate its storage increasing its capacity to N (or greater).
        /// </summary>
        /// <param name="N">the new memory size</param>
        /// <returns>true if the memory was reserved successfully, false otherwise</returns>
        bool Reserve(uint N)
        {
            data.Capacity = (int)N;

            return data.Capacity >= N;
        }

        /// <summary>
        /// Sets the external input and target ranges of the dataset, also sets if the dataset should be scaled using these values.
        /// The dimensionality of the externalRanges Vector should match the numInputDimensions and numTargetDimensions of this dataset.
        /// </summary>
        /// <param name="externalInputRanges">an N dimensional Vector containing the min and max values of the expected input ranges of the dataset</param>
        /// <param name="externalTargetRanges">an N dimensional Vector containing the min and max values of the expected target ranges of the dataset</param>
        /// <param name="useExternalRanges">sets if these ranges should be used to scale the dataset, default value is false</param>
        /// <returns>true if the external ranges were set, false otherwise</returns>
        bool SetExternalRanges(Vector<MinMax> externalInputRanges, Vector<MinMax> externalTargetRanges, bool useExternalRanges)
        {
            if (externalInputRanges.GetSize() != numInputDimensions) return false;
            if (externalTargetRanges.GetSize() != numTargetDimensions) return false;

            this.externalInputRanges = externalInputRanges;
            this.externalTargetRanges = externalTargetRanges;
            this.useExternalRanges = useExternalRanges;

            return true;
        }


        /// <summary>
        /// Sets if the dataset should be scaled using an external range (if useExternalRanges == true) or the ranges of the dataset (if false).
        /// The external ranges need to be set FIRST before calling this function, otherwise it will return false.
        /// </summary>
        /// <param name="useExternalRanges">sets if these ranges should be used to scale the dataset</param>
        /// <returns>true if the useExternalRanges variable was set, false otherwise</returns>
        bool EnableExternalRangeScaling(bool useExternalRanges)
        {
            if (externalInputRanges.GetSize() != numInputDimensions
             && externalTargetRanges.GetSize() != numTargetDimensions)
            {
                this.useExternalRanges = useExternalRanges;
                return true;
            }
            return false;
        }


        /**
         Scales the dataset to the new target range.

         @param minTarget: the minimum target the dataset will be scaled to
         @param maxTarget: the maximum target the dataset will be scaled to
         @return true if the data was scaled correctly, false otherwise
         */
        bool Scale(double minTarget, double maxTarget)
        {
            Vector<MinMax> inputRanges = GetInputRanges();
            Vector<MinMax> targetRanges = GetTargetRanges();
            return Scale(inputRanges, targetRanges, minTarget, maxTarget);
        }


        /// <summary>
        /// Scales the dataset to the new target range, using the Vector of ranges as the min and max source ranges.
        /// </summary>
        /// <param name="inputVectorRanges"></param>
        /// <param name="targetVectorRanges"></param>
        /// <param name="minTarget"></param>
        /// <param name="maxTarget"></param>
        /// <returns>true if the data was scaled correctly, false otherwise</returns>
        bool Scale(Vector<MinMax> inputVectorRanges, Vector<MinMax> targetVectorRanges, double minTarget, double maxTarget)
        {
            if (inputVectorRanges.GetSize() != numInputDimensions
            || targetVectorRanges.GetSize() != numTargetDimensions)
            {
                return false;
            }

            var scaledInputVector = new VectorFloat(numInputDimensions, 0);
            var scaledTargetVector = new VectorFloat(numTargetDimensions, 0);
            for (uint i = 0; i < totalNumSamples; i++)
            {

                //Scale the input Vector
                for (uint j = 0; j < numInputDimensions; j++)
                {
                    scaledInputVector[j] = GRT.Scale(data[i].GetInputVectorValue(j), inputVectorRanges[j].minValue, inputVectorRanges[j].maxValue, minTarget, maxTarget);
                }
                //Scale the target Vector
                for (uint j = 0; j < numTargetDimensions; j++)
                {
                    scaledTargetVector[j] = GRT.Scale(data[i].GetTargetVectorValue(j), targetVectorRanges[j].minValue, targetVectorRanges[j].maxValue, minTarget, maxTarget);
                }
                //Update the training sample with the scaled data
                data[i].Set(scaledInputVector, scaledTargetVector);
            }

            return true;
        }


        /// <summary>
        /// Saves the data to a file.
        ///  If the file format ends in '.csv' then the data will be saved as comma-seperated-values, otherwise it will be saved
        /// to a custom GRT file(which contains the csv data with an additional header).
        /// </summary>
        /// <param name="filename">the name of the file the data will be saved to</param>
        /// <returns>true if the data was saved successfully, false otherwise</returns>
        public bool Save(string filename)
        {
            //Check if the file should be saved as a csv file
            if (Path.GetExtension(filename) == ".csv")
            {
                return SaveDatasetToCSVFile(filename);
            }

            //Otherwise save it as a custom GRT file
            return SaveDatasetToFile(filename);
        }

        /// <summary>
        /// Load the data from a file.
        /// If the file format ends in '.csv' then the function will try and load the data from a csv format.  If this fails then it will
        /// try and load the data as a custom GRT file.
        /// </summary>
        /// <param name="filename">the name of the file the data will be loaded from</param>
        /// <returns>true if the data was loaded successfully, false otherwise</returns>
        public bool Load(string filename)
        {
            //Check if the file should be loaded as a csv file
            if (Path.GetExtension(filename) == ".csv")
            {
                return LoadDatasetFromCSVFile(filename, numInputDimensions, numTargetDimensions);
            }
            //Otherwise save it as a custom GRT file
            return LoadDatasetFromFile(filename);
        }


        /// <summary>
        /// Saves the labelled regression data to a custom file format.
        /// </summary>
        /// <param name="filename">filename: the name of the file the data will be saved to</param>
        /// <returns>true if the data was saved successfully, false otherwise</returns>
        bool SaveDatasetToFile(string filename)
        {
            using (Stream stream = File.Open(filename, FileMode.Create))
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, this);
            }
            return true;
        }

        /// <summary>
        /// Loads the labelled regression data from a custom file format.
        /// </summary>
        /// <param name="filename">the name of the file the data will be loaded from</param>
        /// <returns>true if the data was loaded successfully, false otherwise</returns>
        bool LoadDatasetFromFile(string filename)
        {
            Clear();

            using (Stream stream = File.Open(filename, FileMode.Open))
            {
                var formater = new BinaryFormatter();
                var data = (RegressionData)formater.Deserialize(stream);
                Clone(data);
            }
            return true;
        }

        /// <summary>
        /// Saves the labelled regression data to a CSV file.
        /// This will save the input Vector as the first N columns and the target data as the following T columns.  Each row will represent a sample.
        /// </summary>
        /// <param name="filename">the name of the file the data will be saved to</param>
        /// <returns>true if the data was saved successfully, false otherwise</returns>
        bool SaveDatasetToCSVFile(string filename)
        {
            bool success = false;
            using (var sw = new StreamWriter(filename, false))
            {
                //Write the data to the CSV file
                for (uint i = 0; i < totalNumSamples; i++)
                {
                    for (uint j = 0; j < numInputDimensions; j++)
                    {
                        sw.Write(data[i].InputVector[j]);
                        sw.Write(',');
                    }
                    for (uint j = 0; j < numTargetDimensions; j++)
                    {
                        sw.Write(data[i].TargetVector[j]);
                        if (j != numTargetDimensions - 1)
                        {
                            sw.Write(',');
                        }
                    }
                    sw.WriteLine();
                }
                success = true;
            }
            return success;
        }

        /// <summary>
        /// Loads the labelled regression data from a CSV file.
        /// Each row represents a sample, the first N columns should represent the input Vector data with the remaining T columns representing the target sample.
        /// The user must specify the length of the input Vector (N) and the length of the target Vector (T).
        /// </summary>
        /// <param name="filename">the name of the file the data will be saved to</param>
        /// <param name="numInputDimensions">the length of an input Vector</param>
        /// <param name="numTargetDimensions">the length of a target Vector</param>
        /// <returns>true if the data was saved successfully, false otherwise</returns>
        bool LoadDatasetFromCSVFile(string filename, uint numInputDimensions, uint numTargetDimensions)
        {

        }



        /// <summary>
        /// Gets the input ranges of the dataset.
        /// </summary>
        /// <returns>a Vector of minimum and maximum values for each input dimension of the data</returns>
        Vector<MinMax> GetInputRanges()
        {
            if (useExternalRanges)
            {
                return externalInputRanges;
            }

            var ranges = new Vector<MinMax>(numInputDimensions);
            if (totalNumSamples > 0)
            {
                for (uint j = 0; j < numInputDimensions; j++)
                {
                    var minmax = new MinMax(data[0].GetInputVectorValue(j), data[0].GetInputVectorValue(j));
                    for (uint i = 0; i < totalNumSamples; i++)
                    {
                        minmax.UpdateMinMax(data[i].GetInputVectorValue(j));
                    }
                    ranges[j] = minmax;
                }
            }
            return ranges;
        }

        /// <summary>
        /// Gets the target ranges of the dataset.
        /// </summary>
        /// <returns>a Vector of minimum and maximum values for each target dimension of the data</returns>
        Vector<MinMax> GetTargetRanges()
        {
            if (useExternalRanges) { return externalTargetRanges; }

            var ranges = new Vector<MinMax>(numTargetDimensions);

            if (totalNumSamples > 0)
            {
                for (uint j = 0; j < numTargetDimensions; j++)
                {
                    var minmax = new MinMax(data[0].GetTargetVectorValue(j), data[0].GetTargetVectorValue(j));
                    for (uint i = 0; i < totalNumSamples; i++)
                    {
                        minmax.UpdateMinMax(data[i].GetTargetVectorValue(j));
                    }
                    ranges[j] = minmax;
                }
            }
            return ranges;
        }


    }
}
