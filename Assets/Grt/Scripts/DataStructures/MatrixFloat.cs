using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace GRT
{
    [System.Serializable]
    public class MatrixFloat : Matrix<double>
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        public MatrixFloat() : base() { }


        /// <summary>
        /// Constructor, sets the size of the matrix to [rows cols]
        /// </summary>
        /// <param name="rows">sets the number of rows in the matrix, must be a value greater than zero</param>
        /// <param name="cols">sets the number of columns in the matrix, must be a value greater than zero</param>
        public MatrixFloat(uint rows, uint cols) : base(rows, cols) { }


        /// <summary>
        /// Copy Constructor, copies the values from the rhs MatrixFloat to this MatrixFloat instance
        /// </summary>
        /// <param name="rhs">the MatrixFloat from which the values will be copied</param>
        public MatrixFloat(MatrixFloat rhs) : base(rhs) { }


        /// <summary>
        /// Copy Constructor, copies the values from the rhs Matrix to this MatrixFloat instance
        /// </summary>
        /// <param name="rhs">the Matrix from which the values will be copied</param>
        public MatrixFloat(Matrix<double> rhs) : base(rhs) { }

        /// <summary>
        /// Copy Constructor, copies the values from the rhs vector to this MatrixFloat instance
        /// </summary>
        /// <param name="rhs">a vector of vectors the values will be copied</param>
        public MatrixFloat(Vector<VectorFloat> rhs) : base()
        {
            if (rhs.Count == 0) { return; }

            uint M = rhs.GetSize();
            uint N = (uint)rhs[0].GetSize();
            Resize(M, N);

            for (int i = 0; i < M; i++)
            {
                if (rhs[i].Count != N)
                {
                    Clear();
                    return;
                }
                for (int j = 0; j < N; j++)
                {
                    data[i * cols + j] = rhs[i][j];
                }
            }
        }

        /**
         Destructor, cleans up any memory
         */
        ~MatrixFloat()
        {
            Clear();
        }


        /// <summary>
        /// Gets a row vector [1 cols] from the Matrix at the row index r
        /// </summary>
        /// <param name="r">the index of the row, this should be in the range [0 rows-1]</param>
        /// <returns>a row vector from the Matrix at the row index r</returns>
        VectorFloat GetRow(uint r)
        {
            var rowVector = new VectorFloat(cols);
            for (int c = 0; c < cols; c++)
            {
                rowVector[c] = data[r * cols + c];
            }
            return rowVector;
        }

        /// <summary>
        /// Gets a  vector rows 1] from the Matrix at the column index c
        /// </summary>
        /// <param name="c">the index of the column, this should be in the range [0 cols-1]</param>
        /// <returns>a column vector from the Matrix at the column index c</returns>
        VectorFloat GetCol(uint c)
        {
            var columnVector = new VectorFloat(rows);
            for (int r = 0; r < rows; r++)
            {
                columnVector[r] = data[r * cols + c];
            }
            return columnVector;
        }


        /// <summary>
        /// Saves the matrix to a CSV file.  This replaces the deprecated saveToCSVFile function.
        /// </summary>
        /// <param name="filename">the name of the CSV file</param>
        /// <returns>true or false, indicating if the data was saved successful</returns>
        public bool Save(string filename)
        {
            using (Stream stream = File.Open(filename, FileMode.Create))
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, this);
            }
            return true;
        }

        /// <summary>
        /// Loads a matrix from a CSV file. This assumes that the data has been saved as rows and columns in the CSV file
        /// and that there are an equal number of columns per row.
        /// This replaces the deprecated loadFromCSVFile function.
        /// </summary>
        /// <param name="filename">the name of the CSV file</param>
        /// <returns>true or false, indicating if the data was loaded successful</returns>
        public bool Load(string filename)
        {
            Clear();

            using (Stream stream = File.Open(filename, FileMode.Open))
            {
                var formater = new BinaryFormatter();
                var data = (MatrixFloat)formater.Deserialize(stream);
                Copy(data);
            }
            return true;
        }

        /*
                /// Saves the matrix to a CSV file.
                /// </summary>
                /// <param name="filename">the name of the CSV file</param>
                /// <returns>true or false, indicating if the data was saved successful</returns>
                public bool SaveToCSVFile(string filename);

                /// <summary>
                /// Loads a matrix from a CSV file. This assumes that the data has been saved as rows and columns in the CSV file
                /// and that there are an equal number of columns per row.
                /// </summary>
                /// <param name="filename">the name of the CSV file</param>
                /// <returns>true or false, indicating if the data was loaded successful</returns>
                public bool LoadFromCSVFile(string filename);
         */

        /// Prints the MatrixFloat contents to std::cout
        /// </summary>
        /// <param name="title">sets the title of the data that will be printed to std::cout</param>
        /// <returns>true or false, indicating if the print was successful</returns>
        public bool Print(string title = "")
        {
            if (data == null) { return false; }

            var sb = new System.Text.StringBuilder();
            if (title != "")
            {
                sb.AppendLine(title);
            }
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    sb.Append(data[i * cols + j]);
                    sb.Append('\t');
                }
                sb.AppendLine();
            }
            Log.Info(sb.ToString());
            return true;
        }

        /// <summary>
        /// Transposes the data.
        /// </summary>
        /// <returns>true or false, indicating if the transpose was successful</returns>
        public bool Transpose()
        {
            if (data == null) { return false; }

            var temp = new MatrixFloat(cols, rows);
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    temp[j, i] = data[i * cols + j];
                }
            }
            Copy(temp);

            return true;
        }

        /// <summary>
        /// Scales the matrix to a new range given by the min and max targets.
        /// </summary>
        /// <param name="minTarget"></param>
        /// <param name="maxTarget"></param>
        /// <returns>true if the matrix was scaled, false otherwise</returns>
        public bool Scale(double minTarget, double maxTarget)
        {
            if (data == null) { return false; }

            Vector<MinMax> ranges = GetRanges();
            return Scale(ranges, minTarget, maxTarget);
        }


        /// <summary>
        /// Scales the matrix to a new range given by the min and max targets using the ranges as the source ranges.
        /// </summary>
        /// <param name="&ranges"></param>
        /// <param name="minTarget"></param>
        /// <param name="maxTarget"></param>
        /// <returns>true if the matrix was scaled, false otherwise</returns>
        public bool Scale(Vector<MinMax> ranges, double minTarget, double maxTarget)
        {
            if (data == null) return false;

            if (ranges.Count != cols)
            {
                return false;
            }

            int i, j = 0;
            for (i = 0; i < rows; i++)
            {
                for (j = 0; j < cols; j++)
                {
                    data[i * cols + j] = GRT.Scale(data[i * cols + j], ranges[j].minValue, ranges[j].maxValue, minTarget, maxTarget);
                }
            }
            return true;
        }

        /// <summary>
        /// Normalizes each row in the matrix by subtracting the row mean and dividing by the row standard deviation.
        /// A small amount (alpha) is added to the standard deviation to stop the normalization from exploding.
        /// </summary>
        /// <param name="alpha">a small value that will be added to the standard deviation</param>
        /// <returns>true if the matrix was normalized, false otherwise</returns>
        public bool ZNorm(double alpha = 0.001)
        {
            if (data == null) return false;

            int i, j = 0;
            double mean, std = 0;
            for (i = 0; i < rows; i++)
            {
                mean = 0;
                std = 0;

                //Compute the mean
                for (j = 0; j < cols; j++)
                {
                    mean += data[i * cols + j];
                }
                mean /= cols;

                //Compute the std dev
                for (j = 0; j < cols; j++)
                {
                    std += (data[i * cols + j] - mean) * (data[i * cols + j] - mean);
                }
                std /= cols;
                std = GRT.Sqrt(std + alpha);

                //Normalize the row
                for (j = 0; j < cols; j++)
                {
                    data[i * cols + j] = (data[i * cols + j] - mean) / std;
                }
            }

            return true;
        }

        /// <summary>
        /// Performs the multiplication of the data by the scalar value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>a new MatrixFloat with the results from the multiplcation</returns>
        public MatrixFloat Multiple(double value)
        {
            if (data == null) { return new MatrixFloat(); }

            var d = new MatrixFloat(rows, cols);
            int i = 0;
            for (i = 0; i < rows * cols; i++)
            {
                d.data[i] = data[i] * value;
            }

            return d;
        }

        /// Performs the multiplcation of this matrix (a) by the vector b.
        /// This will return a new vector (c): c = a * b
        /// </summary>
        /// <param name="b">the vector to multiple with this matrix</param>
        /// <returns>VectorFloat with the results from the multiplcation</returns>
        public VectorFloat Multiple(VectorFloat b)
        {
            uint M = rows;
            uint N = cols;
            uint K = (uint)b.Count;

            if (N != K)
            {
                Log.Error($"multiple(vector b) - The size of b ({b.Count}) does not match the number of columns in this matrix ({N}");
                return new VectorFloat();
            }

            var c = new VectorFloat(M);
            int i, j = 0;
            for (i = 0; i < rows; i++)
            {
                c[i] = 0;
                for (j = 0; j < cols; j++)
                {
                    c[i] += data[i * cols + j] * b[j];
                }
            }

            return c;
        }

        /// <summary>
        /// Performs the multiplcation of this matrix (a) by the matrix b.
        /// This will return a new matrix (c): c = a * b
        /// </summary>
        /// <param name="&b">the matrix to multiple with this matrix</param>
        /// <returns>a new MatrixFloat with the results from the multiplcation</returns>
        public MatrixFloat Multiple(MatrixFloat b)
        {
            uint M = rows;
            uint N = cols;
            uint K = b.GetNumRows();
            uint L = b.GetNumCols();

            if (N != K)
            {
                Log.Error($"multiple(MatrixFloat b) - The number of rows in b ({K}) does not match the number of columns in this matrix ({N})");
                return new MatrixFloat();
            }

            var c = new MatrixFloat(M, L);
            int i, j, k = 0;
            for (i = 0; i < M; i++)
            {
                for (j = 0; j < L; j++)
                {
                    c[i, j] = 0;
                    for (k = 0; k < K; k++)
                    {
                        c[i, j] += data[i * cols + k] * b[k, j];
                    }
                }
            }

            return c;
        }

        /// <summary>
        /// Performs the multiplcation of the matrix a by the matrix b, directly storing the new data in the this matrix instance.
        /// This will resize the current matrix if needed.
        /// This makes this matrix c and gives: c = a * b, or if the aTransposed value is true: c = a' * b
        /// </summary>
        /// <param name="a">the matrix to multiple with b</param>
        /// <param name="b">the matrix to multiple with a</param>
        /// <param name="aTranspose">a flag to indicate if matrix a should be transposed</param>
        /// <returns>true if the operation was completed successfully, false otherwise</returns>
        public bool Multiple(MatrixFloat a, MatrixFloat b, bool aTranspose = false)
        {
            uint M = !aTranspose ? a.GetNumRows() : a.GetNumCols();
            uint N = !aTranspose ? a.GetNumCols() : a.GetNumRows();
            uint K = b.GetNumRows();
            uint L = b.GetNumCols();

            if (N != K)
            {
                Log.Error($"multiple(const MatrixFloat &a,const MatrixFloat &b,const bool aTranspose) - The number of rows in a ({K}) does not match the number of columns in matrix b ({N})");
                return false;
            }

            if (!Resize(M, L))
            {
                Log.Error("multiple(const MatrixFloat &b,const MatrixFloat &c,const bool bTranspose) - Failed to resize matrix!");
                return false;
            }

            int i, j, k = 0;
            if (aTranspose)
            {

                for (j = 0; j < L; j++)
                {
                    for (i = 0; i < M; i++)
                    {
                        data[i * cols + j] = 0;
                        for (k = 0; k < K; k++)
                        {
                            data[i * cols + j] += a[k, i] * b[k, j];
                        }
                    }
                }

            }
            else
            {
                for (j = 0; j < L; j++)
                {
                    for (i = 0; i < M; i++)
                    {
                        data[i * cols + j] = 0;
                        for (k = 0; k < K; k++)
                        {
                            data[i * cols + j] += a[i, k] * b[k, j];
                        }
                    }
                }

            }

            return true;
        }

        /// <summary>
        /// Adds the input matrix data (b) to this matrix (a), giving: a = a + b.
        /// This rows and cols of b must match that of this matrix.
        /// </summary>
        /// <param name="b">the matrix to multiple with b</param>
        /// <returns>true if the operation was completed successfully, false otherwise</returns>
        public bool Add(MatrixFloat b)
        {
            if (b.GetNumRows() != rows)
            {
                Log.Error("add(const MatrixFloat &b) - Failed to add matrix! The rows do not match!");
                return false;
            }

            if (b.GetNumCols() != cols)
            {
                Log.Error("add(const MatrixFloat &b) - Failed to add matrix! The rows do not match!");
                return false;
            }

            int i = 0;
            for (i = 0; i < rows * cols; i++)
            {
                data[i] += b.data[i];
            }
            return true;
        }

        /// <summary>
        /// Adds the input matrix data (a) to the input matrix (b), storing the data in this matrix (c) giving: c = a + b.
        /// The rows and cols in a and b must match.
        /// This will resize the current matrix if needed.
        /// </summary>
        /// <param name="a">the matrix to add with b</param>
        /// <param name="b">the matrix to add with a</param>
        /// <returns>true if the operation was completed successfully, false otherwise</returns>
        public bool Add(MatrixFloat a, MatrixFloat b)
        {
            uint M = a.GetNumRows();
            uint N = a.GetNumCols();

            if (M != b.GetNumRows())
            {
                Log.Error($@"add(const MatrixFloat &a,const MatrixFloat &b) - Failed to add matrix! The rows do not match!
a rows: {M} b rows: {b.GetNumRows()}");
                return false;
            }

            if (N != b.GetNumCols())
            {
                Log.Error($@"add(const MatrixFloat &a,const MatrixFloat &b) - Failed to add matrix! The columns do not match!
a cols: {N}  b cols: {b.GetNumCols()}");
                return false;
            }

            Resize(M, N);

            uint size = M * N;
            for (int i = 0; i < size; i++)
            {
                data[i] = a.data[i] + b.data[i];
            }

            return true;
        }

        /// <summary>
        ///  Subtracts the input matrix data (b) from this matrix (a), giving: a = a - b.
        ///  This rows and cols of b must match that of this matrix.
        /// </summary>
        /// <param name="b">the matrix to subtract from this instance</param>
        /// <returns>true if the operation was completed successfully, false otherwise</returns>
        public bool Subtract(MatrixFloat b)
        {
            if (b.GetNumRows() != rows)
            {
                Log.Error($@"subtract(const MatrixFloat &a,const MatrixFloat &b) - Failed to add matrix! The rows do not match!
rows: {rows} b rows: {b.GetNumRows()}");
                return false;
            }

            if (b.GetNumCols() != cols)
            {
                Log.Error($@"subtract(const MatrixFloat &a,const MatrixFloat &b) - Failed to add matrix! The columns do not match!
cols: {cols} b rows: {b.GetNumCols()}");
                return false;
            }

            uint size = rows * cols;
            for (int i = 0; i < size; i++)
            {
                data[i] -= b.data[i];
            }
            return true;
        }

        /// <summary>
        /// Subtracts the input matrix data (b) from this matrix (a), giving (c): c = a - b.
        /// This rows and cols of b must match that of this matrix.
        /// This will resize the current matrix if needed.
        /// </summary>
        /// <param name="a">the matrix to subtract with b</param>
        /// <param name="b">the matrix to subtract from a</param>
        /// <returns>true if the operation was completed successfully, false otherwise</returns>
        public bool Subtract(MatrixFloat a, MatrixFloat b)
        {

            uint M = a.GetNumRows();
            uint N = a.GetNumCols();

            if (M != b.GetNumRows())
            {
                Log.Error($@"subtract(const MatrixFloat &a,const MatrixFloat &b) - Failed to add matrix! The rows do not match!
a rows: {M} b rows: {b.GetNumRows()}");
                return false;
            }

            if (N != b.GetNumCols())
            {
                Log.Error($@"subtract(const MatrixFloat &a,const MatrixFloat &b) - Failed to add matrix! The columns do not match!
a cols: {N} b cols: {b.GetNumCols()}");
                return false;
            }

            Resize(M, N);

            int i, j;
            for (i = 0; i < M; i++)
            {
                for (j = 0; j < N; j++)
                {
                    data[i * cols + j] = a[i, j] - b[i, j];
                }
            }

            return true;
        }

        /// <summary>
        /// Gets the ranges min value throughout the entire matrix.
        /// </summary>
        /// <returns>a Float value containing the minimum matrix value</returns>
        public double GetMinValue() => data.Min();

        /// <summary>
        /// Gets the ranges max value throughout the entire matrix.
        /// </summary>
        /// <returns>a Float value containing the maximum matrix value</returns>
        public double GetMaxValue() => data.Max();

        /// <summary>
        /// Gets the mean of each column in the matrix and returns this as a VectorFloat.
        /// </summary>
        /// <returns>a VectorFloat with the mean of each column</returns>
        public VectorFloat GetMean()
        {
            var mean = new VectorFloat(cols);
            for (int c = 0; c < cols; c++)
            {
                mean[c] = 0;
                for (int r = 0; r < rows; r++)
                {
                    mean[c] += data[r * cols + c];
                }
                mean[c] /= (double)rows;
            }
            return mean;
        }

        /// <summary>
        /// Gets the standard deviation of each column in the matrix and returns this as a VectorFloat.
        /// </summary>
        /// <returns>a VectorFloat with the standard deviation of each column</returns>
        public VectorFloat GetStdDev()
        {
            VectorFloat mean = GetMean();
            var stdDev = new VectorFloat(cols, 0);

            for (int j = 0; j < cols; j++)
            {
                for (int i = 0; i < rows; i++)
                {
                    stdDev[j] += (data[i * cols + j] - mean[j]) * (data[i * cols + j] - mean[j]);
                }
                stdDev[j] = GRT.Sqrt(stdDev[j] / (double)(rows - 1));
            }
            return stdDev;
        }

        /// <summary>
        /// Gets the covariance matrix of this matrix and returns this as a MatrixFloat.
        /// </summary>
        /// <returns>a MatrixFloat with the covariance matrix of this matrix</returns>
        public MatrixFloat GetCovarianceMatrix()
        {
            Vector<double> mean = GetMean();
            var covMatrix = new MatrixFloat(cols, cols);

            for (int j = 0; j < cols; j++)
            {
                for (int k = 0; k < cols; k++)
                {
                    covMatrix[j, k] = 0;
                    for (int i = 0; i < rows; i++)
                    {
                        covMatrix[j, k] += (data[i * cols + j] - mean[j]) * (data[i * cols + k] - mean[k]);
                    }
                    covMatrix[j, k] /= (double)(rows - 1);
                }
            }
            return covMatrix;
        }

        /// <summary>
        /// Gets the ranges (min and max values) of each column in the matrix.
        /// </summary>
        /// <returns>a vector with the ranges (min and max values) of each column in the matrix</returns>
        public Vector<MinMax> GetRanges()
        {
            if (rows == 0) { return new Vector<MinMax>(); }

            var ranges = new Vector<MinMax>(cols);
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    ranges[j].UpdateMinMax(data[i * cols + j]);
                }
            }
            return ranges;
        }


        /// <summary>
        /// Gets the trace of this matrix.
        /// </summary>
        /// <returns>the trace of this matrix as a Float</returns>
        public double GetTrace()
        {
            double t = 0;
            uint K = (rows < cols ? rows : cols);
            for (int i = 0; i < K; i++)
            {
                t += data[i * cols + i];
            }
            return t;
        }
    }
}
