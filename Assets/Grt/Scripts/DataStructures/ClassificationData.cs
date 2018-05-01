using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;


namespace GRT
{
    [Serializable]
    public class ClassificationData : GRTBase
    {

        #region Private propterties
        string datasetName;                                ///< The name of the dataset
        string infoText;                                   ///< Some infoText about the dataset
        uint numDimensions;                                     ///< The number of dimensions in the dataset
        uint totalNumSamples;                                   ///< The total number of samples in the dataset
        uint kFoldValue;                                        ///< The number of folds the dataset has been spilt into for cross valiation
        bool crossValidationSetup;                              ///< A flag to show if the dataset is ready for cross validation
        bool useExternalRanges;                                 ///< A flag to show if the dataset should be scaled using the externalRanges values
        bool allowNullGestureClass;                             ///< A flag that enables/disables a user from adding new samples with a class label matching the default null gesture label
        Vector<MinMax> externalRanges;                        ///< A Vector containing a set of externalRanges set by the user
        Vector<ClassTracker> classTracker;                  ///< A Vector of ClassTracker, which keeps track of the number of samples of each class
        Vector<ClassificationSample> data;                    ///< The labelled classification data
        Vector<Vector<uint>> crossValidationIndexs;      ///< A Vector to hold the indexs of the dataset for the cross validation
        #endregion

        #region  Methods

        /// <summary>
        /// Constructor, sets the name of the dataset and the number of dimensions of the training data.
        /// The name of the dataset should not contain any spaces.
        /// </summary>
        /// <param name="numDimensions">the number of dimensions of the training data, should be an unsigned integer greater than 0</param>
        /// <param name="datasetName">the name of the dataset, should not contain any spaces</param>
        /// <param name="infoText">some info about the data in this dataset, this can contain spaces</param>
        public ClassificationData(uint numDimensions = 0, string datasetName = "NOT_SET", string infoText = "")
        {
            this.datasetName = datasetName;
            this.numDimensions = numDimensions;
            this.infoText = infoText;
            totalNumSamples = 0;
            crossValidationSetup = false;
            useExternalRanges = false;
            allowNullGestureClass = true;
            if (numDimensions > 0)
            {
                SetNumDimensions(numDimensions);
            }
        }


        /// <summary>
        /// Copy Constructor, copies the ClassificationData from the rhs instance to this instance
        /// </summary>
        /// <param name="rhs">another instance of the ClassificationData class from which the data will be copied to this instance</param>
        public ClassificationData(ClassificationData rhs)
        {
            Clone(rhs);
        }

        ClassificationData()
        {
        }

        /// <summary>
        /// Default Destructor
        /// </summary>
        ~ClassificationData()
        {
            Clear();
        }

        public bool Clone(ClassificationData rhs)
        {
            if (this != rhs)
            {
                datasetName = rhs.datasetName;
                infoText = rhs.infoText;
                numDimensions = rhs.numDimensions;
                totalNumSamples = rhs.totalNumSamples;
                kFoldValue = rhs.kFoldValue;
                crossValidationSetup = rhs.crossValidationSetup;
                useExternalRanges = rhs.useExternalRanges;
                allowNullGestureClass = rhs.allowNullGestureClass;
                externalRanges = new Vector<MinMax>(rhs.externalRanges);
                classTracker = new Vector<ClassTracker>(rhs.classTracker);
                data = new Vector<ClassificationSample>(rhs.data);
                crossValidationIndexs = new Vector<Vector<uint>>(rhs.crossValidationIndexs);
            }
            return true;
        }


        /// <summary>
        /// Array Subscript Operator, returns the ClassificationSample at index i.
        /// It is up to the user to ensure that i is within the range of [0 totalNumSamples-1]
        /// </summary>
        /// <returns>a reference to the i'th ClassificationSample</returns>
        public ClassificationSample this[uint i]
        {
            get { return data[(int)i]; }
        }


        /// <summary>
        /// Clears any previous training data and counters
        /// </summary>
        public void Clear()
        {
            totalNumSamples = 0;
            data.Clear();
            classTracker.Clear();
            crossValidationSetup = false;
            crossValidationIndexs.Clear();
        }


        /// <summary>
        /// Sets the number of dimensions in the training data.
        /// This should be an unsigned integer greater than zero.
        /// This will clear any previous training data and counters.
        /// This function needs to be called before any new samples can be added to the dataset, unless the numDimensions variable was set in the
        /// constructor or some data was already loaded from a file
        /// </summary>
        /// <param name="numDimensions">the number of dimensions of the training data.  Must be an unsigned integer greater than zero</param>
        /// <returns>true if the number of dimensions was correctly updated, false otherwise</returns>
        public bool SetNumDimensions(uint numDimensions)
        {
            if (numDimensions > 0)
            {
                //Clear any previous training data
                Clear();

                //Set the dimensionality of the data
                this.numDimensions = numDimensions;

                //Clear the external ranges
                useExternalRanges = false;
                externalRanges.Clear();

                return true;
            }

            Log.Error("setNumDimensions(const UINT numDimensions) - The number of dimensions of the dataset must be greater than zero!");
            return false;
        }


        /// <summary>
        /// Sets the name of the dataset.
        /// There should not be any spaces in the name.
        /// Will return true if the name is set, or false otherwise.
        /// </summary>
        /// <param name="datasetName"></param>
        /// <returns>true if the name is set, or false otherwise</returns>
        bool SetDatasetName(string datasetName)
        {
            if (string.IsNullOrWhiteSpace(datasetName))
            {
                Log.Error("setDatasetName(string datasetName) - The dataset name cannot contain any spaces!");
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
        /// Sets the name of the class with the given class label.
        /// There should not be any spaces in the className.
        /// Will return true if the name is set, or false if the class label does not exist.
        /// </summary>
        /// <param name="className">the className for which the label should be updated</param>
        /// <param name="classLabel">the updated class label</param>
        /// <returns>true if the name is set, or false if the class label does not exist</returns>
        bool SetClassNameForCorrespondingClassLabel(string className, uint classLabel)
        {
            for (int i = 0; i < classTracker.GetSize(); i++)
            {
                if (classTracker[i].classLabel == classLabel)
                {
                    classTracker[i].className = className;
                    return true;
                }
            }
            Log.Error($"setClassNameForCorrespondingClassLabel(string className,const UINT classLabel) - Failed to find class with label: {classLabel}");
            return false;
        }

        /// <summary>
        /// Sets if the user can add samples to the dataset with the label matching the GRT_DEFAULT_NULL_CLASS_LABEL.
        /// If the allowNullGestureClass is set to true, then the user can add labels matching the default null class label (which is normally 0).
        /// If the allowNullGestureClass is set to false, then the user will not be able to add samples that have a class
        /// label matching the default null class label.
        /// </summary>
        /// <param name="allowNullGestureClass">flag that indicates if the null gesture class should be allowed</param>
        /// <returns>true if the allowNullGestureClass was set, false otherwise</returns>
        bool SetAllowNullGestureClass(bool allowNullGestureClass)
        {
            this.allowNullGestureClass = allowNullGestureClass;
            return true;
        }

        /// <summary>
        /// Adds a new labelled sample to the dataset.
        /// The dimensionality of the sample should match the number of dimensions in the ClassificationData.
        /// The class label should be greater than zero (as zero is used as the default null rejection class label).
        /// </summary>
        /// <param name="classLabel">the class label of the corresponding sample</param>
        /// <param name="sample">the new sample you want to add to the dataset.  The dimensionality of this sample should match the number of dimensions in the ClassificationData</param>
        /// <returns>true if the sample was correctly added to the dataset, false otherwise</returns>
        bool AddSample(uint classLabel, VectorFloat sample)
        {
            if (sample.GetSize() != numDimensions)
            {
                if (totalNumSamples == 0)
                {
                    Log.Warning($"addSample(const UINT classLabel, VectorFloat &sample) - the size of the new sample ({sample.GetSize()}) does not match the number of dimensions of the dataset ({numDimensions}), setting dimensionality to: {numDimensions}");
                    numDimensions = sample.GetSize();
                }
                else
                {
                    Log.Error($"addSample(const UINT classLabel, VectorFloat &sample) - the size of the new sample ({sample.GetSize()}) does not match the number of dimensions of the dataset ({numDimensions})");
                    return false;
                }
            }

            //The class label must be greater than zero (as zero is used for the null rejection class label
            if (classLabel == GRT.DEFAULT_NULL_CLASS_LABEL && !allowNullGestureClass)
            {
                Log.Error("addSample(const UINT classLabel, VectorFloat &sample) - the class label can not be 0!");
                return false;
            }

            //The dataset has changed so flag that any previous cross validation setup will now not work
            crossValidationSetup = false;
            crossValidationIndexs.Clear();

            var newSample = new ClassificationSample(classLabel, sample);
            data.Add(newSample);
            totalNumSamples++;

            if (classTracker.GetSize() == 0)
            {
                var tracker = new ClassTracker(classLabel, 1);
                classTracker.Add(tracker);
            }
            else
            {
                bool labelFound = false;
                for (int i = 0; i < classTracker.GetSize(); i++)
                {
                    if (classLabel == classTracker[i].classLabel)
                    {
                        classTracker[i].counter++;
                        labelFound = true;
                        break;
                    }
                }
                if (!labelFound)
                {
                    var tracker = new ClassTracker(classLabel, 1);
                    classTracker.Add(tracker);
                }
            }

            //Update the class labels
            SortClassLabels();

            return true;
        }

        /// <summary>
        /// Removes the training sample at the specific index from the dataset.
        /// </summary>
        /// <param name="index">the index of the training sample that should be removed</param>
        /// <returns>true if the index is valid and the sample was removed, false otherwise</returns>
        bool RemoveSample(uint index)
        {

            if (totalNumSamples == 0)
            {
                Log.Warning("removeSample( const UINT index ) - Failed to remove sample, the training dataset is empty!");
                return false;
            }

            if (index >= totalNumSamples)
            {
                Log.Warning("removeSample( const UINT index ) - Failed to remove sample, the index is out of bounds! Number of training samples:{totalNumSamples} index:{index}");
                return false;
            }

            //The dataset has changed so flag that any previous cross validation setup will now not work
            crossValidationSetup = false;
            crossValidationIndexs.Clear();

            //Find the corresponding class ID for the last training example
            uint classLabel = data[(int)index].ClassLabel;

            //Remove the training example from the buffer
            data.RemoveAt((int)index);

            totalNumSamples = data.GetSize();

            //Remove the value from the counter
            for (int i = 0; i < classTracker.GetSize(); i++)
            {
                if (classTracker[i].classLabel == classLabel)
                {
                    classTracker[i].counter--;
                    break;
                }
            }

            return true;
        }


        /// <summary>
        /// Removes the last training sample added to the dataset.
        /// </summary>
        /// <returns>true if the last sample was removed, false otherwise</returns>
        bool RemoveLastSample()
        {
            if (totalNumSamples == 0)
            {
                Log.Warning("removeLastSample() - Failed to remove sample, the training dataset is empty!");
                return false;
            }
            return RemoveSample(totalNumSamples - 1);
        }


        /// <summary>
        /// Reserves that the Vector capacity be at least enough to contain M elements.
        ///
        /// If M is greater than the current Vector capacity, the function causes the container to reallocate its storage increasing its capacity to M (or greater).
        /// </summary>
        /// <param name="N">the new memory size</param>
        /// <returns>true if the memory was reserved successfully, false otherwise</returns>
        bool Reserve(uint N)
        {
            data.Capacity = (int)N;
            return data.Capacity >= N;
        }

        /// <summary>
        /// This function adds the class with the classLabel to the class tracker.
        /// If the class tracker already contains the classLabel then the function will return false.
        /// </summary>
        /// <param name="classLabel">the class label you want to add to the classTracker</param>
        /// <param name="className">the name associated with the new class</param>
        /// <returns>true if the classLabel was added, false otherwise</returns>
        bool AddClass(uint classLabel, string className = "NOT_SET")
        {
            //Check to make sure the class label does not exist
            for (int i = 0; i < classTracker.GetSize(); i++)
            {
                if (classTracker[i].classLabel == classLabel)
                {
                    Log.Warning("addClass(const UINT classLabel,string className) - Failed to add class, it already exists! Class label: {classLabel}");
                    return false;
                }
            }

            //Add the class label to the class tracker
            classTracker.Add(new ClassTracker(classLabel, 0, className));

            //Sort the class labels
            SortClassLabels();

            return true;
        }

        /// <summary>
        /// Deletes from the dataset all the samples with a specific class label.
        /// </summary>
        /// <param name="classLabel">the class label of the samples you wish to delete from the dataset</param>
        /// <returns>the number of samples deleted from the dataset</returns>
        uint RemoveClass(uint classLabel)
        {
            uint numExamplesRemoved = 0;
            uint numExamplesToRemove = 0;

            //The dataset has changed so flag that any previous cross validation setup will now not work
            crossValidationSetup = false;
            crossValidationIndexs.Clear();

            //Find out how many training examples we need to remove
            for (int i = 0; i < classTracker.GetSize(); i++)
            {
                if (classTracker[i].classLabel == classLabel)
                {
                    numExamplesToRemove = classTracker[i].counter;
                    classTracker.RemoveAt(i);
                    break;
                }
            }

            //Remove the samples with the matching class ID
            if (numExamplesToRemove > 0)
            {
                int i = 0;
                while (numExamplesRemoved < numExamplesToRemove)
                {
                    if (data[i].ClassLabel == classLabel)
                    {
                        data.RemoveAt(i);
                        numExamplesRemoved++;
                    }
                    else if (++i == data.GetSize())
                    {
                        break;
                    }
                }
            }

            totalNumSamples = data.GetSize();

            return numExamplesRemoved;
        }

        /// <summary>
        /// Relabels all the samples with the class label A with the new class label B.
        /// </summary>
        /// <param name="oldClassLabel">the class label of the samples you want to relabel</param>
        /// <param name="newClassLabel">the class label the samples will be relabelled with</param>
        /// <returns>true if the samples were correctly relablled, false otherwise</returns>
        bool RelabelAllSamplesWithClassLabel(uint oldClassLabel, uint newClassLabel)
        {
            bool oldClassLabelFound = false;
            bool newClassLabelAllReadyExists = false;
            uint indexOfOldClassLabel = 0;
            uint indexOfNewClassLabel = 0;

            //Find out how many training examples we need to relabel
            for (int i = 0; i < classTracker.GetSize(); i++)
            {
                if (classTracker[i].classLabel == oldClassLabel)
                {
                    indexOfOldClassLabel = (uint)i;
                    oldClassLabelFound = true;
                }
                if (classTracker[i].classLabel == newClassLabel)
                {
                    indexOfNewClassLabel = (uint)i;
                    newClassLabelAllReadyExists = true;
                }
            }

            //If the old class label was not found then we can't do anything
            if (!oldClassLabelFound)
            {
                return false;
            }

            //Relabel the old class labels
            for (int i = 0; i < totalNumSamples; i++)
            {
                if (data[i].ClassLabel == oldClassLabel)
                {
                    data[i].ClassLabel = newClassLabel;
                }
            }

            //Update the class tracler
            if (newClassLabelAllReadyExists)
            {
                //Add the old sample count to the new sample count
                classTracker[(int)indexOfNewClassLabel].counter += classTracker[(int)indexOfOldClassLabel].counter;
            }
            else
            {
                //Create a new class tracker
                classTracker.Add(new ClassTracker(newClassLabel, classTracker[(int)indexOfOldClassLabel].counter, classTracker[(int)indexOfOldClassLabel].className));
            }

            //Erase the old class tracker
            classTracker.RemoveAt((int)indexOfOldClassLabel);

            //Sort the class labels
            SortClassLabels();

            return true;
        }

        /// <summary>
        /// Sets the external ranges of the dataset, also sets if the dataset should be scaled using these values.
        /// The dimensionality of the externalRanges Vector should match the number of dimensions of this dataset.
        /// </summary>
        /// <param name="externalRanges">an N dimensional Vector containing the min and max values of the expected ranges of the dataset.</param>
        /// <param name="useExternalRanges">sets if these ranges should be used to scale the dataset, default value is false.</param>
        /// <returns>true if the external ranges were set, false otherwise</returns>
        bool SetExternalRanges(Vector<MinMax> externalRanges, bool useExternalRanges = false)
        {
            if (externalRanges.Count != numDimensions) return false;

            this.externalRanges = externalRanges;
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
            if (externalRanges.GetSize() == numDimensions)
            {
                this.useExternalRanges = useExternalRanges;
                return true;
            }
            return false;
        }


        /// <summary>
        /// Scales the dataset to the new target range.
        /// </summary>
        /// <param name="minTarget">the minimum range that the target data should be scaled to</param>
        /// <param name="maxTarget">the maximum range that the target data should be scaled to</param>
        /// <returns><true if the data was scaled correctly, false otherwise/returns>
        bool Scale(double minTarget, double maxTarget)
        {
            Vector<MinMax> ranges = GetRanges();
            return Scale(ranges, minTarget, maxTarget);
        }

        /// <summary>
        /// Scales the dataset to the new target range, using the Vector of ranges as the min and max source ranges.
        /// </summary>
        /// <param name="ranges">a vector containing the new ranges</param>
        /// <param name="minTarget">the minimum range that the target data should be scaled to</param>
        /// <param name="maxTarget">the maximum range that the target data should be scaled to</param>
        /// <returns>true if the data was scaled correctly, false otherwise</returns>
        bool Scale(Vector<MinMax> ranges, double minTarget, double maxTarget)
        {
            if (ranges.GetSize() != numDimensions) { return false; }

            //Scale the training data
            for (int i = 0; i < totalNumSamples; i++)
            {
                for (int j = 0; j < numDimensions; j++)
                {
                    data[i][j] = GRT.Scale(data[i][j], ranges[j].minValue, ranges[j].maxValue, minTarget, maxTarget);
                }
            }

            return true;
        }

        /// <summary>
        /// Saves the classification data to a file.
        /// If the file format ends in '.csv' then the data will be saved as comma-seperated-values, otherwise it will be saved
        /// to a custom GRT file (which contains the csv data with an additional header).
        /// </summary>
        /// <param name="filename">filename: the name of the file the data will be saved to</param>
        /// <returns>true if the data was saved successfully, false otherwise</returns>
        bool Save(string filename)
        {
            //Check if the file should be saved as a csv file
            if ((Path.GetExtension(filename) == ".csv"))
            {
                return SaveDatasetToCSVFile(filename);
            }
            //Otherwise save it as a custom GRT file
            return SaveDatasetToFile(filename);
        }

        /// <summary>
        /// Load the classification data from a file.
        /// If the file format ends in '.csv' then the function will try and load the data from a csv format.  If this fails then it will
        /// try and load the data as a custom GRT file.
        /// </summary>
        /// <param name="filename">the name of the file the data will be loaded from</param>
        /// <returns>true if the data was loaded successfully, false otherwise</returns>
        bool Load(string filename)
        {
            //Check if the file should be loaded as a csv file
            if (Path.GetExtension(filename) == ".csv")
            {
                return LoadDatasetFromCSVFile(filename);
            }

            //Otherwise save it as a custom GRT file
            return LoadDatasetFromFile(filename);
        }


        /// <summary>
        /// Saves the labelled classification data to a custom file format.
        /// </summary>
        /// <param name="filename">the name of the file the data will be saved to</param>
        /// <returns>true if the data was saved successfully, false otherwise</returns>
        bool SaveDatasetToFile(string filename)
        {
            bool success = false;
            using (Stream stream = File.Open(filename, FileMode.Create))
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, this);
                success = true;
            }
            return success;
        }

        /// <summary>
        /// Loads the labelled classification data from a custom file format.
        /// </summary>
        /// <param name="filename">the name of the file the data will be loaded from</param>
        /// <returns>true if the data was loaded successfully, false otherwise</returns>
        bool LoadDatasetFromFile(string filename)
        {
            Clear();
            bool success = false;

            using (Stream stream = File.Open(filename, FileMode.Open))
            {
                var formater = new BinaryFormatter();
                var data = (ClassificationData)formater.Deserialize(stream);
                success = Clone(data);
            }
            return success;
        }

        /// <summary>
        /// Saves the labelled classification data to a CSV file.
        /// This will save the class label as the first column and the sample data as the following N columns, where N is the number of dimensions in the data.  Each row will represent a sample.
        /// </summary>
        /// <param name="filename">the name of the file the data will be saved to</param>
        /// <returns>true if the data was saved successfully, false otherwise</returns>
        bool SaveDatasetToCSVFile(string filename)
        {
            bool success = false;
            using (var sw = new StreamWriter(filename, false))
            {
                //Write the data to the CSV file
                for (int i = 0; i < totalNumSamples; i++)
                {
                    sw.Write(data[i].ClassLabel);
                    for (int j = 0; j < numDimensions; j++)
                    {
                        sw.Write(',');
                        sw.Write(data[i][j]);
                    }
                    sw.WriteLine();
                }
                success = true;
            }
            return success;
        }

        /// <summary>
        /// Loads the labelled classification data from a CSV file.
        /// This assumes the data is formatted with each row representing a sample.
        /// The class label should be the first column followed by the sample data as the following N columns, where N is the number of dimensions in the data.
        /// If the class label is not the first column, you should set the 2nd argument (UINT classLabelColumnIndex) to the column index that contains the class label.
        /// </summary>
        /// <param name="filename">the name of the file the data will be loaded from</param>
        /// <param name="classLabelColumnIndex">the index of the column containing the class label. Default value = 0</param>
        /// <returns><true if the data was loaded successfully, false otherwise/returns>
        bool LoadDatasetFromCSVFile(string filename, uint classLabelColumnIndex = 0)
        {
            numDimensions = 0;
            datasetName = "NOT_SET";
            infoText = "";

            //Clear any previous data
            Clear();

            //Parse the CSV file
            var parser = new FileParser();

            if (!parser.ParseCSVFile(filename, true))
            {
                Log.Error("loadDatasetFromCSVFile(const std::string &filename,const UINT classLabelColumnIndex) - Failed to parse CSV file!");
                return false;
            }

            if (!parser.ConsistantColmunSize)
            {
                Log.Error("loadDatasetFromCSVFile(const std::string &filename,const UINT classLabelColumnIndexe) - The CSV file does not have a consistent number of columns!");
                return false;
            }

            if (parser.ColumnSize <= 1)
            {
                Log.Error("loadDatasetFromCSVFile(const std::string &filename,const UINT classLabelColumnIndex) - The CSV file does not have enough columns! It should contain at least two columns!");
                return false;
            }

            //Set the number of dimensions
            numDimensions = parser.ColumnSize - 1;

            //Reserve the memory for the data
            totalNumSamples = parser.RowSize;
            // data.Resize(parser.RowSize, new ClassificationSample(numDimensions));
            data.Clear();
            data.Capacity = (int)totalNumSamples;
            for (int i = 0; i < totalNumSamples; i++)
            {
                data.Add(new ClassificationSample(numDimensions));
            }

            uint classLabel = 0;
            int j = 0;
            int n = 0;
            for (int i = 0; i < totalNumSamples; i++)
            {
                //Get the class label
                classLabel = uint.Parse(parser[i][(int)classLabelColumnIndex]);

                //Set the class label
                data[i].ClassLabel = classLabel;

                //Get the sample data
                j = 0;
                n = 0;
                while (j != numDimensions)
                {
                    if (n != classLabelColumnIndex)
                    {
                        data[i][j++] = double.Parse(parser[i][n]);
                    }
                    n++;
                }

                //Update the class tracker
                if (classTracker.Count == 0)
                {
                    var tracker = new ClassTracker(classLabel, 1);
                    classTracker.Add(tracker);
                }
                else
                {
                    bool labelFound = false;
                    int numClasses = classTracker.Count;
                    for (int k = 0; k < numClasses; k++)
                    {
                        if (classLabel == classTracker[k].classLabel)
                        {
                            classTracker[k].counter++;
                            labelFound = true;
                            break;
                        }
                    }
                    if (!labelFound)
                    {
                        var tracker = new ClassTracker(classLabel, 1);
                        classTracker.Add(tracker);
                    }
                }
            }

            //Sort the class labels
            SortClassLabels();

            return true;
        }


        /// <summary>
        /// Prints the dataset info (such as its name and infoText) and the stats (such as the number of examples, number of dimensions, number of classes, etc.)
        /// to the std out.
        /// </summary>
        /// <returns>true if the dataset info and stats were printed successfully, false otherwise</returns>
        public bool PrintStats()
        {
            Log.Info(GetStatsAsString());
            return true;
        }


        /// <summary>
        /// Sorts the class labels (in the class tracker) in ascending order.
        /// </summary>
        /// <returns>true if the labels were sorted successfully, false otherwise</returns>
        public bool SortClassLabels()
        {
            classTracker.Sort(ClassTracker.SortByClassLabelAscending);
            return true;
        }

        /// <summary>
        /// Adds the data to the current instance of the ClassificationData.
        /// The number of dimensions in both datasets must match.
        /// The names of the classes from the data will be added to the current instance.
        /// </summary>
        /// <param name="otherData">the dataset to add to this dataset</param>
        /// <returns>true if the datasets were merged, false otherwise</returns>
        bool Merge(ClassificationData otherData)
        {
            if (otherData.numDimensions != numDimensions)
            {
                Log.Error($"merge(const ClassificationData &labelledData) - The number of dimensions in the labelledData ({otherData.GetNumDimensions()}) does not match the number of dimensions of this dataset ({numDimensions})");
                return false;
            }

            //The dataset has changed so flag that any previous cross validation setup will now not work
            crossValidationSetup = false;
            crossValidationIndexs.Clear();

            uint M = otherData.GetNumSamples();

            //Reserve the memory
            Reserve((GetNumSamples() + M));

            //Add the data from the labelledData to this instance
            for (uint i = 0; i < M; i++)
            {
                AddSample(otherData[i].ClassLabel, otherData[i].Sample);
            }

            //Set the class names from the dataset
            Vector<ClassTracker> classTracker = otherData.classTracker;
            for (int i = 0; i < classTracker.Count; i++)
            {
                SetClassNameForCorrespondingClassLabel(classTracker[i].className, classTracker[i].classLabel);
            }

            //Sort the class labels
            SortClassLabels();

            return true;
        }

        /// <summary>
        /// Splits the dataset into a training dataset (which is kept by this instance of the ClassificationData) and
        /// a testing/validation dataset (which is returned as a new instance of a ClassificationData).
        /// </summary>
        /// <param name="splitPercentage">sets the percentage of data which remains in this instance, the remaining percentage of data is then returned as the testing/validation dataset</param>
        /// <param name="useStratifiedSampling">sets if the dataset should be broken into homogeneous groups first before randomly being spilt, default value is false</param>
        /// <returns>a new ClassificationData instance, containing the remaining data not kept but this instance</returns>
        ClassificationData Split(uint trainingSizePercentage, bool useStratifiedSampling = false)
        {

            //Partitions the dataset into a training dataset (which is kept by this instance of the ClassificationData) and
            //a testing/validation dataset (which is return as a new instance of the ClassificationData).  The trainingSizePercentage
            //therefore sets the size of the data which remains in this instance and the remaining percentage of data is then added to
            //the testing/validation dataset

            //The dataset has changed so flag that any previous cross validation setup will now not work
            crossValidationSetup = false;
            crossValidationIndexs.Clear();

            var trainingSet = new ClassificationData(numDimensions);
            var testSet = new ClassificationData(numDimensions);
            trainingSet.SetAllowNullGestureClass(allowNullGestureClass);
            testSet.SetAllowNullGestureClass(allowNullGestureClass);

            //Create the random partion indexs
            // Random random;
            uint K = GetNumClasses();

            //Make sure both datasets get all the class labels, even if they have no samples in each
            // trainingSet.classTracker.resize(K);
            // testSet.classTracker.resize(K);
            for (int k = 0; k < K; k++)
            {
                trainingSet.classTracker.Add(new ClassTracker());
                testSet.classTracker.Add(new ClassTracker());
            }

            for (int k = 0; k < K; k++)
            {
                trainingSet.classTracker[k].classLabel = classTracker[k].classLabel;
                testSet.classTracker[k].classLabel = classTracker[k].classLabel;
                trainingSet.classTracker[k].counter = 0;
                testSet.classTracker[k].counter = 0;
            }

            if (useStratifiedSampling)
            {
                //Break the data into seperate classes
                var classData = new Vector<Vector<uint>>(K);

                //Add the indexs to their respective classes
                for (int i = 0; i < totalNumSamples; i++)
                {
                    classData[(int)GetClassLabelIndexValue(data[i].ClassLabel)].Add((uint)i);
                }

                //Randomize the order of the indexs in each of the class index buffers
                for (int k = 0; k < K; k++)
                {
                    classData[k].Shuffle();
                }

                //Reserve the memory
                uint numTrainingSamples = 0;
                uint numTestSamples = 0;

                for (int k = 0; k < K; k++)
                {
                    uint numTrainingExamples = (uint)Math.Floor((double)classData[k].Count / 100.0 * (double)trainingSizePercentage);
                    uint numTestExamples = ((uint)classData[k].Count) - numTrainingExamples;
                    numTrainingSamples += numTrainingExamples;
                    numTestSamples += numTestExamples;
                }

                trainingSet.Reserve(numTrainingSamples);
                testSet.Reserve(numTestSamples);

                //Loop over each class and add the data to the trainingSet and testSet
                for (int k = 0; k < K; k++)
                {
                    uint numTrainingExamples = (uint)Math.Floor((double)classData[k].Count / 100.0 * (double)trainingSizePercentage);

                    //Add the data to the training and test sets
                    for (int i = 0; i < numTrainingExamples; i++)
                    {
                        trainingSet.AddSample(data[(int)classData[k][i]].ClassLabel, data[(int)classData[k][i]].Sample);
                    }
                    for (int i = (int)numTrainingExamples; i < classData[k].Count; i++)
                    {
                        testSet.AddSample(data[(int)classData[k][i]].ClassLabel, data[(int)classData[k][i]].Sample);
                    }
                }
            }
            else
            {
                uint numTrainingExamples = (uint)Math.Floor((double)totalNumSamples / 100.0 * (double)trainingSizePercentage);

                //Create the random partion indexs
                var indexs = new Vector<uint>(totalNumSamples);
                for (uint i = 0; i < totalNumSamples; i++) { indexs[(int)i] = i; }
                indexs.Shuffle();

                //Reserve the memory
                trainingSet.Reserve(numTrainingExamples);
                testSet.Reserve(totalNumSamples - numTrainingExamples);

                //Add the data to the training and test sets
                for (int i = 0; i < numTrainingExamples; i++)
                {
                    trainingSet.AddSample(data[(int)indexs[i]].ClassLabel, data[(int)indexs[i]].Sample);
                }
                for (int i = (int)(numTrainingExamples); i < totalNumSamples; i++)
                {
                    testSet.AddSample(data[(int)indexs[i]].ClassLabel, data[(int)indexs[i]].Sample);
                }
            }

            //The training and test datasets MUST have the same number of classes as the original data
            Debug.Assert(trainingSet.GetNumClasses() == K);
            Debug.Assert(testSet.GetNumClasses() == K);

            //Overwrite the training data in this instance with the training data of the trainingSet
            // *this = trainingSet;
            Clone(trainingSet);

            //Sort the class labels in this dataset
            SortClassLabels();

            //Sort the class labels of the test dataset
            testSet.SortClassLabels();

            return testSet;
        }


        /// <summary>
        /// This function prepares the dataset for k-fold cross validation and should be called prior to calling the getTrainingFold(UINT foldIndex) or getTestingFold(UINT foldIndex) functions.  It will spilt the dataset into K-folds, as long as K < M, where M is the number of samples in the dataset.
        /// </summary>
        /// <param name="K">the number of folds the dataset will be split into, K should be less than the number of samples in the dataset</param>
        /// <param name="useStratifiedSampling">sets if the dataset should be broken into homogeneous groups first before randomly being spilt, default value is false</param>
        /// <returns>true if the dataset was split correctly, false otherwise</returns>
        bool SpiltDataIntoKFolds(uint K, bool useStratifiedSampling = false)
        {

            crossValidationSetup = false;
            crossValidationIndexs.Clear();

            //K can not be zero
            if (K == 0)
            {
                Log.Error("spiltDataIntoKFolds(const UINT K,const bool useStratifiedSampling) - K can not be zero!");
                return false;
            }

            //K can not be larger than the number of examples
            if (K > totalNumSamples)
            {
                Log.Error("spiltDataIntoKFolds(const UINT K,const bool useStratifiedSampling) - K can not be larger than the total number of samples in the dataset!");
                return false;
            }

            //K can not be larger than the number of examples in a specific class if the stratified sampling option is true
            if (useStratifiedSampling)
            {
                for (int c = 0; c < classTracker.Count; c++)
                {
                    if (K > classTracker[c].counter)
                    {
                        Log.Error("spiltDataIntoKFolds(const UINT K,const bool useStratifiedSampling) - K can not be larger than the number of samples in any given class!");
                        return false;
                    }
                }
            }

            //Setup the dataset for k-fold cross validation
            kFoldValue = K;
            var indexs = new Vector<uint>(totalNumSamples);

            //Work out how many samples are in each fold, the last fold might have more samples than the others
            uint numSamplesPerFold = (uint)Math.Floor(totalNumSamples / (double)K);

            //Add the random indexs to each fold
            // crossValidationIndexs.resize(K);
            crossValidationIndexs.Capacity = (int)K;

            //Create the random partion indexs
            Random random = new Random();
            uint randomIndex = 0;

            if (useStratifiedSampling)
            {
                //Break the data into seperate classes
                var classData = new Vector<Vector<uint>>(GetNumClasses());

                //Add the indexs to their respective classes
                for (int i = 0; i < totalNumSamples; i++)
                {
                    classData[(int)GetClassLabelIndexValue(data[i].ClassLabel)].Add((uint)i);
                }

                //Randomize the order of the indexs in each of the class index buffers
                for (int c = 0; c < GetNumClasses(); c++)
                {
                    uint numSamples = (uint)classData[c].Count;
                    for (int x = 0; x < numSamples; x++)
                    {
                        //Pick a random indexs
                        randomIndex = (uint)random.GetRandomNumberInt(0, (int)numSamples);

                        //Swap the indexs
                        uint tmp = classData[c][x];
                        classData[c][x] = classData[c][(int)randomIndex];
                        classData[c][(int)randomIndex] = tmp;
                    }
                }

                //Loop over each of the k folds, at each fold add a sample from each class
                for (uint c = 0; c < GetNumClasses(); c++)
                {
                    uint k = 0;
                    foreach (var d in classData[c])
                    {
                        crossValidationIndexs[k].Add(d);
                        k++;
                        k = k % K;
                    }
                }
            }
            else
            {
                //Randomize the order of the data
                for (uint i = 0; i < totalNumSamples; i++) indexs[i] = i;
                for (uint x = 0; x < totalNumSamples; x++)
                {
                    //Pick a random index
                    randomIndex = (uint)random.GetRandomNumberInt(0, (int)totalNumSamples);

                    //Swap the indexs
                    uint a = indexs[x];
                    uint b = indexs[randomIndex];
                    GRT.Swap(ref a, ref b);
                    indexs[x] = a;
                    indexs[randomIndex] = b;
                }

                uint counter = 0;
                uint foldIndex = 0;
                for (uint i = 0; i < totalNumSamples; i++)
                {
                    //Add the index to the current fold
                    crossValidationIndexs[foldIndex].Add(indexs[i]);

                    //Move to the next fold if ready
                    if (++counter == numSamplesPerFold && foldIndex < K - 1)
                    {
                        foldIndex++;
                        counter = 0;
                    }
                }
            }

            crossValidationSetup = true;
            return true;

        }

        /// <summary>
        /// Returns the training dataset for the k-th fold for cross validation.  The spiltDataIntoKFolds(UINT K) function should have been called once before using this function.
        /// The foldIndex should be in the range [0 K-1], where K is the number of folds the data was spilt into.
        /// </summary>
        /// <param name="foldIndex">the index of the fold you want the training data for, this should be in the range [0 K-1], where K is the number of folds the data was spilt into </param>
        /// <returns>a training dataset</returns>
        ClassificationData GetTrainingFoldData(uint foldIndex)
        {

            var trainingData = new ClassificationData();
            trainingData.SetNumDimensions(numDimensions);
            trainingData.SetAllowNullGestureClass(allowNullGestureClass);

            if (!crossValidationSetup)
            {
                Log.Error("getTrainingFoldData(const UINT foldIndex) - Cross Validation has not been setup! You need to call the spiltDataIntoKFolds(UINT K,bool useStratifiedSampling) function first before calling this function!");
                return trainingData;
            }

            if (foldIndex >= kFoldValue) return trainingData;

            //Add the class labels to make sure they all exist
            for (uint k = 0; k < GetNumClasses(); k++)
            {
                trainingData.AddClass(classTracker[k].classLabel, classTracker[k].className);
            }

            //Add the data to the training set, this will consist of all the data that is NOT in the foldIndex
            uint index = 0;
            for (uint k = 0; k < kFoldValue; k++)
            {
                if (k != foldIndex)
                {
                    for (uint i = 0; i < crossValidationIndexs[k].GetSize(); i++)
                    {

                        index = crossValidationIndexs[k][i];
                        trainingData.AddSample(data[index].ClassLabel, data[index].Sample);
                    }
                }
            }

            //Sort the class labels
            trainingData.SortClassLabels();

            return trainingData;
        }


        /// <summary>
        /// Returns the test dataset for the k-th fold for cross validation.  The spiltDataIntoKFolds(UINT K) function should have been called once before using this function.
        /// The foldIndex should be in the range [0 K-1], where K is the number of folds the data was spilt into.
        /// </summary>
        /// <param name="foldIndex">the index of the fold you want the test data for, this should be in the range [0 K-1], where K is the number of folds the data was spilt into </param>
        /// <returns>a test dataset</returns>
        ClassificationData GetTestFoldData(uint foldIndex)
        {

            var testData = new ClassificationData();
            testData.SetNumDimensions(numDimensions);
            testData.SetAllowNullGestureClass(allowNullGestureClass);

            if (!crossValidationSetup) return testData;

            if (foldIndex >= kFoldValue) return testData;

            //Add the class labels to make sure they all exist
            for (uint k = 0; k < GetNumClasses(); k++)
            {
                testData.AddClass(classTracker[k].classLabel, classTracker[k].className);
            }

            testData.Reserve(crossValidationIndexs[foldIndex].GetSize());

            //Add the data to the test fold
            uint index = 0;
            for (uint i = 0; i < crossValidationIndexs[foldIndex].GetSize(); i++)
            {

                index = crossValidationIndexs[foldIndex][i];
                testData.AddSample(data[index].ClassLabel, data[index].Sample);
            }

            //Sort the class labels
            testData.SortClassLabels();

            return testData;
        }

        /// <summary>
        /// Returns the all the data with the class label set by classLabel.
        /// The classLabel should be a valid classLabel, otherwise the dataset returned will be empty.
        /// </summary>
        /// <param name="classLabel">the class label of the class you want the data for</param>
        /// <returns>a dataset containing all the data with the matching classLabel</returns>
        ClassificationData GetClassData(uint classLabel)
        {

            var classData = new ClassificationData();
            classData.SetNumDimensions(this.numDimensions);
            classData.SetAllowNullGestureClass(allowNullGestureClass);

            //Reserve the memory for the class data
            for (uint i = 0; i < classTracker.GetSize(); i++)
            {
                if (classTracker[i].classLabel == classLabel)
                {
                    classData.Reserve(classTracker[i].counter);
                    break;
                }
            }

            for (uint i = 0; i < totalNumSamples; i++)
            {
                if (data[i].ClassLabel == classLabel)
                {
                    classData.AddSample(classLabel, data[i].Sample);
                }
            }

            return classData;
        }

        /// <summary>
        /// Gets a bootstrapped dataset from the current dataset.  If the numSamples parameter is set to zero, then the
        /// size of the bootstrapped dataset will match the size of the current dataset, otherwise the size of the bootstrapped
        /// dataset will match the numSamples parameter.
        /// </summary>
        /// <param name="numSamples">the size of the bootstrapped dataset</param>
        /// <param name="balanceDataset">if true will use stratified sampling to balance the dataset returned, otherwise will use random sampling</param>
        /// <returns>a bootstrapped ClassificationData</returns>
        ClassificationData GetBootstrappedDataset(uint numSamples_ = 0, bool balanceDataset = false)
        {
            var rand = new Random();
            var newDataset = new ClassificationData();
            newDataset.SetNumDimensions(GetNumDimensions());
            newDataset.SetAllowNullGestureClass(allowNullGestureClass);
            newDataset.SetExternalRanges(externalRanges, useExternalRanges);

            uint numBootstrapSamples = numSamples_ > 0 ? numSamples_ : totalNumSamples;

            Debug.Assert(numBootstrapSamples > 0);

            newDataset.Reserve(numBootstrapSamples);

            uint K = GetNumClasses();

            //Add all the class labels to the new dataset to ensure the dataset has a list of all the labels
            for (uint k = 0; k < K; k++)
            {
                newDataset.AddClass(classTracker[k].classLabel);
            }

            if (balanceDataset)
            {
                //Group the class indexs
                var classIndexs = new Vector<Vector<uint>>(K);
                for (uint i = 0; i < totalNumSamples; i++)
                {
                    classIndexs[GetClassLabelIndexValue(data[i].ClassLabel)].Add(i);
                }

                //Get the class with the minimum number of examples
                uint numSamplesPerClass = (uint)Math.Floor(numBootstrapSamples / (double)K);

                //Randomly select the training samples from each class
                uint classIndex = 0;
                uint classCounter = 0;
                uint randomIndex = 0;
                for (uint i = 0; i < numBootstrapSamples; i++)
                {
                    randomIndex = (uint)rand.GetRandomNumberInt(0, classIndexs[classIndex].Count);
                    randomIndex = classIndexs[classIndex][randomIndex];
                    newDataset.AddSample(data[randomIndex].ClassLabel, data[randomIndex].Sample);
                    if (classCounter++ >= numSamplesPerClass && classIndex + 1 < K)
                    {
                        classCounter = 0;
                        classIndex++;
                    }
                }

            }
            else
            {
                //Randomly select the training samples to add to the new data set
                uint randomIndex;
                for (uint i = 0; i < numBootstrapSamples; i++)
                {
                    randomIndex = (uint)rand.GetRandomNumberInt(0, (int)totalNumSamples);
                    newDataset.AddSample(data[randomIndex].ClassLabel, data[randomIndex].Sample);
                }
            }

            //Sort the class labels so they are in order
            newDataset.SortClassLabels();

            return newDataset;
        }



        /// <summary>
        /// Reformats the ClassificationData as RegressionData to enable regression algorithms like the MLP to be used as a classifier.
        /// This sets the number of targets in the regression data equal to the number of classes in the classification data.  The output target ouput of each regression sample will therefore
        /// be all zeros, except for the index matching the class label which will be 1.
        /// For this to work, the labelled classification data cannot have any samples with a class label of 0!
        /// </summary>
        /// <returns>a new RegressionData instance, containing the reformated classification data</returns>
        RegressionData ReformatAsRegressionData()
        {
            return null;
        }



        /**
         Gets the stats of the dataset as a string

         @return returns the stats of this dataset as a string
         */
        string GetStatsAsString()
        {
            var sb = new System.Text.StringBuilder();
            sb.Append($"DatasetName:\t {datasetName} \n");
            sb.Append($"DatasetInfo:\t {infoText} \n");
            sb.Append($"Number of Dimensions:\t {numDimensions} \n");
            sb.Append($"Number of Samples:\t {totalNumSamples} \n");
            sb.Append($"Number of Classes:\t {GetNumClasses()} \n");
            sb.Append($"ClassStats:\n");

            for (int k = 0; k < GetNumClasses(); k++)
            {
                sb.Append($"ClassLabel:\t {classTracker[k].classLabel}");
                sb.Append($"\tNumber of Samples:\t {classTracker[k].counter}");
                sb.Append($"\tClassName:\t {classTracker[k].className}\n");
            }

            Vector<MinMax> ranges = GetRanges();

            sb.Append("Dataset Ranges:\n");
            for (int j = 0; j < ranges.Count; j++)
            {
                sb.Append($"[{(j + 1)}] Min:\t{ranges[j].minValue}\tMax: {ranges[j].maxValue}\n");
            }
            return sb.ToString();
        }


        /// <summary>
        /// Gets the number of dimensions of the labelled classification data.
        /// </summary>
        /// <returns>an unsigned int representing the number of dimensions in the classification data</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        uint GetNumDimensions() => numDimensions;


        /// <summary>
        /// Gets the number of samples in the classification data across all the classes.
        /// </summary>
        /// <returns>an unsigned int representing the total number of samples in the classification data</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        uint GetNumSamples() => totalNumSamples;

        /// <summary>
        /// Gets the number of classes.
        /// </summary>
        /// <returns>an unsigned int representing the number of classes</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        uint GetNumClasses() => classTracker.GetSize();


        /// <summary>
        /// Gets the ranges of the classification data.
        /// </summary>
        /// <returns>a Vector of minimum and maximum values for each dimension of the data</returns>
        Vector<MinMax> GetRanges()
        {
            //If the dataset should be scaled using the external ranges then return the external ranges
            if (useExternalRanges) { return externalRanges; }

            var ranges = new Vector<MinMax>(numDimensions);
            ranges.Fill(new MinMax());
            MinMax range;

            //Otherwise return the min and max values for each column in the dataset
            if (totalNumSamples > 0)
            {
                for (int j = 0; j < numDimensions; j++)
                {
                    range.minValue = data[0][j];
                    range.maxValue = data[0][j];
                    for (int i = 0; i < totalNumSamples; i++)
                    {
                        range.UpdateMinMax(data[i][j]);
                    }
                    ranges[j] = range;
                }
            }
            return ranges;
        }


        /// <summary>
        /// Gets the index of the class label from the class tracker.
        /// </summary>
        /// <param name="classLabel">the class label you want to access the index for</param>
        /// <returns>an unsigned int representing the index of the class label in the class tracker</returns>
        uint GetClassLabelIndexValue(uint classLabel)
        {
            for (int k = 0; k < classTracker.GetSize(); k++)
            {
                if (classTracker[k].classLabel == classLabel)
                {
                    return (uint)k;
                }
            }
            Log.Warning("getClassLabelIndexValue(UINT classLabel) - Failed to find class label: {classLabel} in class tracker!");
            return 0;
        }


        #endregion
    }
}
