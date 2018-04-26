using NUnit.Framework;

namespace GRT
{

    public static class MatrixFloatUnitTest
    {
        [Test]
        public static void DefaultConstructor()
        {
            var mat = new MatrixFloat();

            Assert.AreEqual(0, mat.GetSize());
            Assert.AreEqual(0, mat.GetNumRows());
            Assert.AreEqual(0, mat.GetNumCols());
        }

        // Tests the resize c'tor.
        [Test]
        public static void ResizeConstructor()
        {
            const uint numRows = 100;
            const uint numCols = 50;
            var mat = new MatrixFloat(numRows, numCols);
            Assert.AreEqual(numRows * numCols, mat.GetSize());
            Assert.AreEqual(numRows, mat.GetNumRows());
            Assert.AreEqual(numCols, mat.GetNumCols());
        }

        // Tests the copy c'tor.
        [Test]
        public static void CopyConstructor()
        {
            const uint numRows = 100;
            const uint numCols = 50;
            var mat1 = new MatrixFloat(numRows, numCols);
            Assert.AreEqual(numRows, mat1.GetNumRows());
            Assert.AreEqual(numCols, mat1.GetNumCols());
            var mat2 = new MatrixFloat(mat1);
            Assert.AreEqual(numRows, mat2.GetNumRows());
            Assert.AreEqual(numCols, mat2.GetNumCols());
        }

        // Tests the equals operator.
        [Test]
        public static void EqualsConstructor()
        {
            // Not implemented
        }

        // Tests the Vector c'tor.
        [Test]

        public static void VectorConstructor()
        {
            const uint numRows = 100;
            const uint numCols = 50;
            var vec = new Vector<VectorFloat>(numRows, new VectorFloat(numCols));
            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < numCols; j++)
                {
                    vec[i][j] = 1;
                }
            }

            var mat = new MatrixFloat(vec);
            Assert.AreEqual(numRows, mat.GetNumRows());
            Assert.AreEqual(numCols, mat.GetNumCols());
            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < numCols; j++)
                {
                    Assert.AreEqual(mat[i, j], vec[i][j]);
                }
            }
        }

        // Tests the [] operator
        [Test]

        public static void AccessOperator()
        {
            const uint numRows = 100;
            const uint numCols = 50;
            var mat = new MatrixFloat(numRows, numCols);
            Assert.AreEqual(numRows, mat.GetNumRows());
            Assert.AreEqual(numCols, mat.GetNumCols());
            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < numCols; j++)
                {
                    mat[i, j] = i * j;
                }
            }
            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < numCols; j++)
                {
                    Assert.AreEqual(mat[i, j], i * j);
                }
            }
        }

        // Tests the getRowVector
        [Test]

        public static void GetRow()
        {
            const uint numRows = 100;
            const uint numCols = 50;
            var mat = new MatrixFloat(numRows, numCols);
            Assert.AreEqual(numRows, mat.GetNumRows());
            Assert.AreEqual(numCols, mat.GetNumCols());
            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < numCols; j++)
                {
                    mat[i, j] = i * j;
                }
            }
            for (int i = 0; i < numRows; i++)
            {
                var rowVector = mat.GetRows((uint)i);
                for (int j = 0; j < numCols; j++)
                {
                    Assert.AreEqual(rowVector[j], i * j);
                }
            }
        }

        // Tests the getColVector
        [Test]

        public static void GetCol()
        {
            const uint numRows = 100;
            const uint numCols = 50;
            var mat = new MatrixFloat(numRows, numCols);
            Assert.AreEqual(numRows, mat.GetNumRows());
            Assert.AreEqual(numCols, mat.GetNumCols());
            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < numCols; j++)
                {
                    mat[i, j] = i * j;
                }
            }
            for (int j = 0; j < numCols; j++)
            {
                var colVector = mat.GetCols((uint)j);
                for (int i = 0; i < numRows; i++)
                {
                    Assert.AreEqual(colVector[i], i * j);
                }
            }
        }

        // Tests the resize
        [Test]
        public static void Resize()
        {
            const uint numRows = 100;
            const uint numCols = 50;
            const uint newRows = 200;
            const uint newCols = 100;
            var mat = new MatrixFloat(numRows, numCols);
            Assert.AreEqual(numRows, mat.GetNumRows());
            Assert.AreEqual(numCols, mat.GetNumCols());
            Assert.True(mat.Resize(newRows, newCols));
            Assert.AreEqual(newRows, mat.GetNumRows());
            Assert.AreEqual(newCols, mat.GetNumCols());
        }

        // Tests the copy
        [Test]
        public static void Copy()
        {
            const uint numRows = 100;
            const uint numCols = 50;
            var mat1 = new MatrixFloat(numRows, numCols);
            Assert.AreEqual(numRows, mat1.GetNumRows());
            Assert.AreEqual(numCols, mat1.GetNumCols());
            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < numCols; j++)
                {
                    mat1[i, j] = i * j;
                }
            }
            var mat2 = new MatrixFloat();
            Assert.True(mat2.Copy(mat1));
            Assert.AreEqual(numRows, mat2.GetNumRows());
            Assert.AreEqual(numCols, mat2.GetNumCols());
            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < numCols; j++)
                {
                    Assert.AreEqual(mat1[i, j], mat2[i, j]);
                }
            }
        }

        // Tests the setAllValues
        [Test]
        public static void SetAll()
        {
            const uint numRows = 100;
            const uint numCols = 50;
            var mat = new MatrixFloat(numRows, numCols);
            Assert.AreEqual(numRows, mat.GetNumRows());
            Assert.AreEqual(numCols, mat.GetNumCols());
            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < numCols; j++)
                {
                    mat[i, j] = i * j;
                }
            }
            Assert.True(mat.SetAll(0));
            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < numCols; j++)
                {
                    Assert.AreEqual(mat[i, j], 0);
                }
            }
        }

        // Tests the setRowVector
        [Test]
        public static void SetRowVector()
        {
            const uint numRows = 100;
            const uint numCols = 50;
            var mat = new MatrixFloat(numRows, numCols);
            Assert.AreEqual(numRows, mat.GetNumRows());
            Assert.AreEqual(numCols, mat.GetNumCols());
            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < numCols; j++)
                {
                    mat[i, j] = 0;
                }
            }
            var vec = new VectorFloat(numCols);
            Assert.True(vec.SetAll(1000));
            Assert.True(mat.SetRowVector(vec, 0));
            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < numCols; j++)
                {
                    Assert.AreEqual(mat[i, j], i == 0 ? 1000 : 0);
                }
            }
        }

        // Tests the setColVector
        [Test]
        public static void SetColVector()
        {
            const uint numRows = 100;
            const uint numCols = 50;
            var mat = new MatrixFloat(numRows, numCols);
            Assert.AreEqual(numRows, mat.GetNumRows());
            Assert.AreEqual(numCols, mat.GetNumCols());
            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < numCols; j++)
                {
                    mat[i, j] = 0;
                }
            }
            var vec = new VectorFloat(numRows);
            Assert.True(vec.SetAll(1000));
            Assert.True(mat.SetColVector(vec, 0));
            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < numCols; j++)
                {
                    Assert.AreEqual(mat[i, j], j == 0 ? 1000 : 0);
                }
            }
        }

        // Tests the CSV save and load functions
        [Test]
        public static void SaveLoad()
        {
            const uint numRows = 100;
            const uint numCols = 1000;
            var mat = new MatrixFloat(numRows, numCols);
            Assert.AreEqual(numRows, mat.GetNumRows());
            Assert.AreEqual(numCols, mat.GetNumCols());
            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < numCols; j++)
                {
                    mat[i, j] = i * j;
                }
            }

            //Save the data
            Assert.True(mat.Save("matrix_float.dat"));

            Assert.True(mat.Clear());

            //Load the data
            {
                var mat2 = new MatrixFloat();
                Assert.True(mat2.Load("matrix_float.dat"));
                Assert.AreEqual(numRows, mat2.GetNumRows());
                Assert.AreEqual(numCols, mat2.GetNumCols());
            }
        }
    }
}
