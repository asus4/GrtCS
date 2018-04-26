using System;
using System.Runtime.CompilerServices;

namespace GRT
{
    public class Matrix<T>
    {
        #region Properties
        protected uint rows;      ///< The number of rows in the Matrix
        protected uint cols;      ///< The number of columns in the Matrix
        protected uint size;      ///< Stores rows * cols
        protected uint capacity;  ///< The capacity of the Matrix, this will be the number of rows, not the actual memory size

        T[] data;             ///< A pointer to the raw data
        // T[] rowPtr;             ///< A pointer to each row in the data

        #endregion

        /// <summary>
        /// Default Constructor
        /// </summary>
        public Matrix()
        {
            rows = 0;
            cols = 0;
            size = 0;
            capacity = 0;
            data = null;
        }

        /// <summary>
        /// Constructor, sets the size of the matrix to [rows cols]
        /// </summary>
        /// <param name="rows">sets the number of rows in the matrix, must be a value greater than zero</param>
        /// <param name="cols">sets the number of columns in the matrix, must be a value greater than zero</param>
        public Matrix(uint rows, uint cols)
        {
            data = null;
            Resize(rows, cols);
        }


        /// <summary>
        /// Constructor, sets the size of the matrix to [rows cols] and initalizes all the data to data
        /// </summary>
        /// <param name="rows">sets the number of rows in the matrix, must be a value greater than zero</param>
        /// <param name="cols">sets the number of columns in the matrix, must be a value greater than zero</param>
        /// <param name="data">default value that will be used to initalize all the values in the matrix</param>
        public Matrix(uint rows, uint cols, T data)
        {
            this.data = null;
            Resize(rows, cols, data);
        }

        /// <summary>
        /// Copy Constructor, copies the values from the rhs Matrix to this Matrix instance
        /// </summary>
        /// <param name="rhs">the Matrix from which the values will be copied</param>
        public Matrix(Matrix<T> rhs)
        {
            data = null;
            rows = 0;
            cols = 0;
            size = 0;
            capacity = 0;
            Copy(rhs);
        }


        /// <summary>
        /// Copy Constructor, copies the values from the input vector to this Matrix instance.
        /// The input vector must be a vector< vector< T > > in a [rows cols] format.  The number of
        /// columns in each row must be consistent.  Both the rows and columns must be greater than 0.
        /// </summary>
        /// <param name="data">the input data which will be copied to this Matrix instance</param>
        public Matrix(Vector<Vector<T>> data)
        {
            this.data = null;
            rows = 0;
            cols = 0;
            size = 0;
            capacity = 0;

            uint tempRows = data.GetSize();
            uint tempCols = 0;

            //If there is no data then return
            if (tempRows == 0) return;

            //Check to make sure all the columns are the same size
            for (int i = 0; i < tempRows; i++)
            {
                if (i == 0) tempCols = data[i].GetSize();
                else
                {
                    if (data[i].GetSize() != tempCols)
                    {
                        return;
                    }
                }
            }

            if (tempCols == 0) return;

            //Resize the matrix and copy the data
            if (Resize(tempRows, tempCols))
            {
                for (int i = 0; i < tempRows; i++)
                {
                    for (int j = 0; j < tempCols; j++)
                    {
                        this.data[(i * cols) + j] = data[i][j];
                    }
                }
            }
        }

        ~Matrix()
        {
            Clear();
        }



        public T this[int r, int c]
        {
            get
            {
                return data[r * cols + c];
            }
            set
            {
                data[r * cols + c] = value;
            }
        }

        /// <summary>
        ///  Gets a row vector [1 cols] from the Matrix at the row index r
        /// </summary>
        /// <param name="r">the index of the row, this should be in the range [0 rows-1]</param>
        /// <returns>a row vector from the Matrix at the row index r</returns>
        public T[] GetRows(uint r)
        {
            var rowVector = new T[cols];
            for (int c = 0; c < cols; c++)
            {
                rowVector[c] = data[r * cols + c];
            }
            return rowVector;
        }

        /// <summary>
        /// Gets a column vector [rows 1] from the Matrix at the column index c
        /// </summary>
        /// <param name="c">the index of the column, this should be in the range [0 cols-1]</param>
        /// <returns>a column vector from the Matrix at the column index c</returns>
        public T[] GetCols(uint c)
        {
            var columnVector = new T[rows];
            for (int r = 0; r < rows; r++)
            {
                columnVector[r] = data[r * cols + c];

            }
            return columnVector;
        }

        /// <summary>
        /// Concatenates the entire matrix into a single vector and returns the vector.
        /// The data can either be concatenated by row or by column, by setting the respective concatByRow parameter to true of false.
        /// If concatByRow is true then the data in the matrix will be added to the vector row-vector by row-vector, otherwise
        /// the data will be added column-vector by column-vector.
        /// </summary>
        /// <param name="concatByRow">sets if the matrix data will be added to the vector row-vector by row-vector</param>
        /// <returns>a vector containing the entire matrix data</returns>
        Vector<T> GetConcatenatedVector(bool concatByRow = true)
        {
            if (rows == 0 || cols == 0)
            {
                return new Vector<T>();
            }

            var vectorData = new Vector<T>(rows * cols);
            int i, j = 0;
            if (concatByRow)
            {
                for (i = 0; i < rows; i++)
                {
                    for (j = 0; j < cols; j++)
                    {
                        vectorData[(i * (int)cols) + j] = data[i * cols + j];
                    }
                }
            }
            else
            {
                for (j = 0; j < cols; j++)
                {
                    for (i = 0; i < rows; i++)
                    {
                        vectorData[(i * (int)cols) + j] = data[i * cols + j];
                    }
                }
            }
            return vectorData;
        }


        /// <summary>
        /// Resizes the Matrix to the new size of [r c].  If [r c] matches the previous size then the matrix will not be resized but the function will return true.
        /// </summary>
        /// <param name="r">the number of rows, must be greater than zero</param>
        /// <param name="c">the number of columns, must be greater than zero</param>
        /// <returns>rue or false, indicating if the resize was successful </returns>
        public virtual bool Resize(uint r, uint c)
        {
            if (r + c == 0)
            {
                Log.Error("resize(...) - Failed to resize matrix, rows and cols == zero!");
                return false;
            }

            //If the rows and cols are unchanged then do not resize the data
            if (r == rows && c == cols)
            {
                return true;
            }

            //Clear any previous memory
            Clear();

            if (r > 0 && c > 0)
            {
                try
                {
                    rows = r;
                    cols = c;
                    size = r * c;
                    capacity = r;

                    data = new T[size];

                    if (data == null)
                    {
                        rows = 0;
                        cols = 0;
                        size = 0;
                        capacity = 0;
                        Log.Error($"resize(const unsigned r,const unsigned int c) - Failed to allocate memory! r: {r}  c: {c}");
                        throw new Exception("Matrix::resize(const unsigned int r,const unsigned int c) - Failed to allocate memory!");
                        // return false;
                    }
                    return true;
                }
                catch (Exception e)
                {
                    Log.Error($"resize: Failed to allocate memory. Error: {e.ToString()} rows: {r} cols: {c}");
                    Clear();
                    return false;
                }
                // return false;
            }
            return false;
        }

        /// <summary>
        /// Resizes the Matrix to the new size of [r c].  If [r c] matches the previous size then the matrix will not be resized but the function will return true.
        /// </summary>
        /// <param name="r">the number of rows, must be greater than zero</param>
        /// <param name="c">the number of columns, must be greater than zero</param>
        /// <param name="value">the default value that will be set across all values in the buffer</param>
        /// <returns><true or false, indicating if the resize was successful/returns>
        public virtual bool Resize(uint r, uint c, T value)
        {
            if (!Resize(r, c))
            {
                return false;
            }
            return SetAll(value);
        }


        /// <summary>
        /// Copies the data from the rhs matrix to this matrix.
        /// </summary>
        /// <param name="rhs">the matrix you want to copy into this matrix</param>
        /// <returns>true or false, indicating if the copy was successful</returns>
        public virtual bool Copy(Matrix<T> rhs)
        {
            if (this != rhs)
            {
                if (size != rhs.size)
                {
                    if (!Resize(rhs.rows, rhs.cols))
                    {
                        throw new Exception("Matrix::copy( const Matrix<T> &rhs ) - Failed to allocate resize matrix!");
                        // return false;
                    }
                }

                //Copy the data
                int i = 0;
                for (i = 0; i < size; i++)
                {
                    data[i] = rhs.data[i];
                }
            }

            return true;
        }

        /// <summary>
        /// Sets all the values in the Matrix to the input value
        /// </summary>
        /// <param name="value">the value you want to set all the Matrix values to</param>
        /// <returns>true or false, indicating if the set was successful </returns>
        public bool SetAll(T value)
        {
            if (data != null)
            {
                int i = 0;
                for (i = 0; i < size; i++)
                {
                    data[i] = value;
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Sets all the values in the row at rowIndex with the values in the vector called row.
        /// The size of the row vector must match the number of columns in this Matrix.
        /// </summary>
        /// <param name="row">the vector of row values you want to add</param>
        /// <param name="rowIndex">the row index of the row you want to update, must be in the range [0 rows]</param>
        /// <returns>true or false, indicating if the set was successful</returns>
        public bool SetRowVector(Vector<T> row, uint rowIndex)
        {
            if (data == null) return false;
            if (row.Count != cols) return false;
            if (rowIndex >= rows) return false;

            int j = 0;
            for (j = 0; j < cols; j++)
            {
                data[rowIndex * cols + j] = row[j];
            }
            return true;
        }


        /// <summary>
        /// Sets all the values in the column at colIndex with the values in the vector called column.
        /// The size of the column vector must match the number of rows in this Matrix.
        /// </summary>
        /// <param name="column">the vector of column values you want to add</param>
        /// <param name="colIndex">the column index of the column you want to update, must be in the range [0 cols]</param>
        /// <returns>true or false, indicating if the set was successful</returns>
        public bool SetColVector(Vector<T> column, uint colIndex)
        {
            if (data == null) return false;
            if (column.Count != rows) return false;
            if (colIndex >= cols) return false;

            for (int i = 0; i < rows; i++)
            {
                data[i * cols + colIndex] = column[i];
            }
            return true;
        }

        /// <summary>
        /// Adds the input sample to the end of the Matrix, extending the number of rows by 1.  The number of columns in the sample must match
        /// the number of columns in the Matrix, unless the Matrix size has not been set, in which case the new sample size will define the
        /// number of columns in the Matrix.
        /// </summary>
        /// <param name="sample">the new column vector you want to add to the end of the Matrix.  Its size should match the number of columns in the Matrix</param>
        /// <returns>true or false, indicating if the push was successful</returns>
        public bool PushBack(Vector<T> sample)
        {

            int i, j = 0;

            //If there is no data, but we know how many cols are in a sample then we simply create a new buffer of size 1 and add the sample
            if (data == null)
            {
                cols = (uint)sample.Count;
                if (!Resize(1, cols))
                {
                    Clear();
                    return false;
                }
                for (j = 0; j < cols; j++)
                    data[j] = sample[j];
                return true;
            }

            //If there is data and the sample size does not match the number of columns then return false
            if (sample.Count != cols)
            {
                return false;
            }

            //Check to see if we have reached the capacity, if not then simply add the new data as there are unused rows
            if (rows < capacity)
            {
                //Add the new sample at the end
                for (j = 0; j < cols; j++)
                    data[rows * cols + j] = sample[j];

            }
            else
            { //Otherwise we copy the existing data from the data ptr into a new buffer of size (rows+1) and add the sample at the end

                int tmpRows = (int)rows + 1;
                var tmpDataPtr = new T[tmpRows * (int)cols];

                if (tmpDataPtr == null)
                {
                    //If NULL then we have run out of memory
                    return false;
                }

                //Copy the original data into the tmp buffer
                for (i = 0; i < rows * cols; i++)
                {
                    tmpDataPtr[i] = data[i];
                }

                //Add the new sample at the end of the tmp buffer
                for (j = 0; j < cols; j++)
                {
                    tmpDataPtr[rows * cols + j] = sample[j];
                }

                //Delete the original data and copy the pointer
                data = tmpDataPtr;

                //Increment the capacity so it matches the number of rows
                capacity++;
            }

            //Increment the number of rows
            rows++;

            //Update the size
            size = rows * cols;

            //Finally return true to signal that the data was added correctly
            return true;
        }


        /// <summary>
        /// This function reserves a consistent block of data so new rows can more effecitenly be pushed_back into the Matrix.
        /// The capacity variable represents the number of rows you want to reserve, based on the current number of columns.
        /// </summary>
        /// <param name="capacity">the new capacity value</param>
        /// <returns>true if the data was reserved, false otherwise</returns>
        public bool Reserve(uint capacity)
        {
            //If the number of columns has not been set, then we can not do anything
            if (cols == 0) { return false; }

            //Reserve the data and copy and existing data
            int i = 0;
            var tmpDataPtr = new T[capacity * cols];
            if (tmpDataPtr == null)
            {//If NULL then we have run out of memory
                return false;
            }

            //Copy the existing data into the new memory
            for (i = 0; i < size; i++)
            {
                tmpDataPtr[i] = data[i];
            }

            //Delete the original data and copy the pointer
            data = tmpDataPtr;

            //Store the new capacity
            this.capacity = capacity;

            //Store the size
            size = rows * cols;

            return true;
        }

        /// <summary>
        /// Cleans up any dynamic memory and sets the number of rows and columns in the matrix to zero
        /// </summary>
        /// <returns>true if the data was cleared successfully</returns>
        bool Clear()
        {
            if (data != null)
            {
                data = null;
            }
            rows = 0;
            cols = 0;
            size = 0;
            capacity = 0;
            return true;
        }

        /// <summary>
        /// Gets the number of rows in the Matrix
        /// </summary>
        /// <returns>the number of rows in the Matrix</returns>

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint GetNumRows() => rows;

        /// <summary>
        /// Gets the number of columns in the Matrix
        /// </summary>
        /// <returns>the number of columns in the Matrix</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint GetNumCols() => cols;

        /// <summary>
        /// Gets the capacity of the Matrix. This is the number of rows that have been reserved for the Matrix.
        /// You can control the capacity using the reserve function
        /// </summary>
        /// <returns>the number of columns in the Matrix</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint GetCapacity() => capacity;

        /// <summary>
        /// Gets the size of the Matrix. This is rows * size.
        /// </summary>
        /// <returns>the number of columns in the Matrix</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint GetSize() => size;


        /// <summary>
        /// Gets a pointer to the main data pointer
        /// </summary>
        /// <returns>a pointer to the raw data</returns>
        T[] getData() => data;
    }
}
