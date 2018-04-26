using System.IO;
using System.Linq;
using BinaryFormatter = System.Runtime.Serialization.Formatters.Binary.BinaryFormatter;

namespace GRT
{
    public class VectorFloat : Vector<double>
    {


        public VectorFloat(uint size):base(size)
        {

        }

        /// <summary>
        /// Constructor, sets the size of the vector and sets all elements to value
        /// </summary>
        /// <param name="size">sets the size of the vector</param>
        /// <param name="value">the value that will be written to all elements in the vector</param>
        /// <returns></returns>
        public VectorFloat(uint size, double value) : base(size, value)
        {
        }

        /// <summary>
        /// Saves the vector to a CSV file.  This replaces the deprecated saveToCSVFile function.
        /// </summary>
        /// <param name="filename">the name of the CSV file</param>
        /// <returns>returns true or false, indicating if the data was saved successful</returns>
        public bool Save(string filename)
        {
            int N = Count;
            if (N == 0)
            {
                Log.Warning("Vector is empty, nothing to save!");
                return false;
            }

            using (Stream stream = File.Open(filename, FileMode.Create))
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, this);
            }
            return true;
        }

        /// <summary>
        /// Loads a vector from a CSV file. This assumes that the data has been saved as rows and columns in the CSV file and that there are an equal number of columns per row.
        /// </summary>
        /// <param name="filename">the name of the CSV file</param>
        /// <returns>returns true or false, indicating if the data was loaded successful</returns>
        public bool Load(string filename)
        {
            Clear();

            using (Stream stream = File.Open(filename, FileMode.Open))
            {
                var formater = new BinaryFormatter();
                var data = (VectorFloat)formater.Deserialize(stream);
                AddRange(data);
            }
            return true;
        }

        /// <summary>
        /// Prints the VectorFloat contents to std::cout
        /// </summary>
        /// <param name="title">sets the title of the data that will be printed to std::cout</param>
        /// <returns>returns true or false, indicating if the print was successful</returns>
        public bool Print(string title = "")
        {
            var sb = new System.Text.StringBuilder();
            if (title != "")
            {
                sb.AppendFormat("{}\t", title);
            }
            foreach (var n in this)
            {
                sb.Append(n);
                sb.Append('\t');
            }
            Log.Info(sb.ToString());
            return true;
        }

        /// <summary>
        /// Scales the vector to a new range given by the min and max targets, this uses the minimum and maximum values in the
        /// existing vector as the minSource and maxSource for min-max scaling.
        /// </summary>
        /// <param name="minTarget"></param>
        /// <param name="maxTarget"></param>
        /// <param name="constrain"></param>
        /// <returns>returns true if the vector was scaled, false otherwise</returns>
        public bool Scale(double minTarget, double maxTarget, bool constrain = true)
        {
            MinMax range = MinMax;
            return Scale(range.minValue, range.maxValue, minTarget, maxTarget, constrain);
        }

        public bool Scale(double minSource, double maxSource, double minTarget, double maxTarget, bool constrain = true)
        {
            int N = Count;
            if (N == 0)
            {
                return false;
            }

            for (int i = 0; i < N; i++)
            {
                this[i] = GRTBase.Scale(this[i], ref minSource, ref maxSource, ref minTarget, ref maxTarget, constrain);
            }
            return true;
        }

        /// <returns>returns the minimum value in the vector</returns>
        public double MinValue => this.Min();

        /// <returns>returns the maximum value in the vector</returns>
        public double MaxValue => this.Max();

        /// <returns>returns the mean of the vector</returns>
        public double Mean => this.Sum() / Count;

        /// <returns>returns the standard deviation of the vector</returns>
        public double StdDev
        {
            get
            {
                double stdDev = 0.0;
                int N = Count;
                if (N == 0) { return stdDev; }

                double mean = Mean;

                for (int i = 0; i < N; i++)
                {
                    stdDev += GRT.Sqr(this[i] - mean);
                }
                stdDev = GRT.Sqrt(stdDev / (N - 1.0));
                return stdDev;
            }
        }

        /**
         @return returns the minimum and maximum values in the vector
         */
        public MinMax MinMax
        {
            get
            {
                var range = new MinMax();
                int N = Count;
                if (N == 0) { return range; }
                for (int i = 0; i < N; ++i)
                {
                    range.UpdateMinMax(this[i]);
                }
                return range;
            }
        }


    }
}
