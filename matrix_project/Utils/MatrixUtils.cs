using matrix_project.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace matrix_project.Utils
{

    /// <summary>
    /// Utility class for matrix operations.
    /// </summary>
    public static class MatrixUtils
    {
        /// <summary>
        /// Generates a random matrix with specified dimensions and value range.
        /// </summary>
        /// <typeparam name="T">The type of elements in the matrix, must be a struct implementing IConvertible.</typeparam>
        /// <param name="rows">Number of rows.</param>
        /// <param name="cols">Number of columns.</param>
        /// <param name="min">Minimum value for random numbers.</param>
        /// <param name="max">Maximum value for random numbers.</param>
        /// <param name="converter">Optional converter function for random values.</param>
        /// <returns>A random matrix with the specified dimensions.</returns>
        /// <exception cref="ArgumentException">Thrown if dimensions are invalid or min is greater than max.</exception>
        public static T[,] GenerateRandomMatrix<T>(
            int rows,
            int cols,
            double min = 0.0,
            double max = 1.0,
            Func<double, T>? converter = null)
            where T : struct, IConvertible
        {
            if (rows <= 0 || cols <= 0)
                throw new ArgumentException("Matrix dimensions must be positive.");
            if (min > max)
                throw new ArgumentException("Minimum value cannot be greater than maximum.");

            converter ??= (v => (T)Convert.ChangeType(v, typeof(T)));

            var rnd = new Random();
            var matrix = new T[rows, cols];

            for (int r = 0; r < rows; r++)
                for (int c = 0; c < cols; c++)
                {
                    double value = rnd.NextDouble() * (max - min) + min;
                    matrix[r, c] = converter(value);
                }

            return matrix;
        }

        /// <summary>
        /// Prints a matrix to the console with optional size limits.
        /// </summary>
        /// <typeparam name="T">The type of elements in the matrix.</typeparam>
        /// <param name="matrix">The matrix to print.</param>
        /// <param name="maxRows">Maximum number of rows to print.</param>
        /// <param name="maxCols">Maximum number of columns to print.</param>
        /// <exception cref="ArgumentNullException">Thrown if the matrix is null.</exception>
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
    }
}
