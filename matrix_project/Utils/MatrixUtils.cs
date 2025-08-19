using matrix_project.Interfaces;
using System;

namespace matrix_project.Utils
{
    /// <summary>
    /// Utility class for matrix operations.
    /// </summary>
    public static class MatrixUtils
    {
        public static T[,] GenerateRandomMatrix<T>(
            int rows,
            int cols,
            Func<T> randomValueGenerator)
        {
            if (rows <= 0 || cols <= 0)
                throw new ArgumentException("Matrix dimensions must be positive.");
            if (randomValueGenerator == null)
                throw new ArgumentNullException(nameof(randomValueGenerator));

            var matrix = new T[rows, cols];
            for (int r = 0; r < rows; r++)
                for (int c = 0; c < cols; c++)
                    matrix[r, c] = randomValueGenerator();

            return matrix;
        }

        public static void PrintMatrix<T>(IMatrix<T> matrix, int maxRows = 10, int maxCols = 10)
        {
            if (matrix == null)
                throw new ArgumentNullException(nameof(matrix));

            var rows = Math.Min(matrix.RowCount, maxRows);
            var cols = Math.Min(matrix.ColCount, maxCols);

            for (var r = 0; r < rows; r++)
            {
                for (var c = 0; c < cols; c++)
                {
                    var value = matrix[r, c];
                    Console.Write(value is IFormattable f ? $"{f.ToString("F2", null),-8}" : $"{value,-8}");
                }
                Console.WriteLine(cols < matrix.ColCount ? "..." : "");
            }

            if (rows < matrix.RowCount)
                Console.WriteLine("...");
        }

        // --- Для double ---
        public static bool AreMatricesEqual(IMatrix<double> a, IMatrix<double> b, double epsilon, out int mismatchRow, out int mismatchCol)
        {
            mismatchRow = -1;
            mismatchCol = -1;

            if (a.RowCount != b.RowCount || a.ColCount != b.ColCount)
                return false;

            for (int i = 0; i < a.RowCount; i++)
            {
                for (int j = 0; j < a.ColCount; j++)
                {
                    if (Math.Abs(a[i, j] - b[i, j]) > epsilon)
                    {
                        mismatchRow = i;
                        mismatchCol = j;
                        return false;
                    }
                }
            }
            return true;
        }

        public static bool AreMatricesEqual(IMatrix<double> a, IMatrix<double> b, double epsilon)
        {
            return AreMatricesEqual(a, b, epsilon, out _, out _);
        }

        // --- Для char ---
        public static bool AreMatricesEqual(IMatrix<char> a, IMatrix<char> b, out int mismatchRow, out int mismatchCol)
        {
            mismatchRow = -1;
            mismatchCol = -1;

            if (a.RowCount != b.RowCount || a.ColCount != b.ColCount)
                return false;

            for (int i = 0; i < a.RowCount; i++)
            {
                for (int j = 0; j < a.ColCount; j++)
                {
                    if (!a[i, j].Equals(b[i, j]))
                    {
                        mismatchRow = i;
                        mismatchCol = j;
                        return false;
                    }
                }
            }
            return true;
        }

        public static bool AreMatricesEqual(IMatrix<char> a, IMatrix<char> b)
        {
            return AreMatricesEqual(a, b, out _, out _);
        }
    }
}
