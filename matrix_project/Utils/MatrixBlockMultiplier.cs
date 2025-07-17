using matrix_project.Interfaces;
using matrix_project.Models;
using System.Numerics;

namespace matrix_project.Utils
{
    public static class MatrixBlockMultiplier
    {
        /// <summary>
        /// Умножает две матрицы блочно с параллельной обработкой.
        /// </summary>
        public static IMatrix<T> Multiply<T>(
            IMatrix<T> a,
            IMatrix<T> b,
            CancellationToken cancellationToken = default,
            int blockSize = 64)
            where T : INumber<T>
        {
            if (a.ColCount != b.RowCount)
                throw new ArgumentException("Кол-во столбцов A должно равняться кол-ву строк B.");

            var result = new Matrix<T>(a.RowCount, b.ColCount);
            var parallelOptions = new ParallelOptions { CancellationToken = cancellationToken };

            int aRows = a.RowCount;
            int bCols = b.ColCount;
            int innerDim = a.ColCount;

            int blockRows = (aRows + blockSize - 1) / blockSize;
            int blockCols = (bCols + blockSize - 1) / blockSize;
            int blockDepth = (innerDim + blockSize - 1) / blockSize;

            Parallel.For(0, blockRows, parallelOptions, blockRow =>
            {
                int rowStart = blockRow * blockSize;
                int rowEnd = Math.Min(rowStart + blockSize, aRows);

                for (int blockCol = 0; blockCol < blockCols; blockCol++)
                {
                    int colStart = blockCol * blockSize;
                    int colEnd = Math.Min(colStart + blockSize, bCols);

                    for (int blockK = 0; blockK < blockDepth; blockK++)
                    {
                        int kStart = blockK * blockSize;
                        int kEnd = Math.Min(kStart + blockSize, innerDim);

                        for (int i = rowStart; i < rowEnd; i++)
                        {
                            for (int j = colStart; j < colEnd; j++)
                            {
                                T sum = result[i, j];

                                for (int k = kStart; k < kEnd; k++)
                                {
                                    cancellationToken.ThrowIfCancellationRequested();

                                    T av = a[i, k];
                                    T bv = b[k, j];
                                    sum += av * bv;
                                }

                                result[i, j] = sum;
                            }
                        }
                    }
                }
            });

            return result;
        }
    }
}
