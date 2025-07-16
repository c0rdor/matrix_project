using matrix_project.Interfaces;
using matrix_project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace matrix_project.Utils
{
    /// <summary>
    /// Implementation of a matrix with generic type elements.
    /// </summary>
    /// <typeparam name="T">The type of elements in the matrix.</typeparam>

    public static class MatrixBlockTransformer
    {
        /// <summary>
        /// Transforms a matrix using a coordinate mapping function.
        /// </summary>
        /// <typeparam name="T">The type of elements in the matrix.</typeparam>
        /// <param name="source">The source matrix to transform.</param>
        /// <param name="resultRows">The number of rows in the result matrix.</param>
        /// <param name="resultCols">The number of columns in the result matrix.</param>
        /// <param name="coordinateMap">Function to map source coordinates to result coordinates.</param>
        /// <returns>The transformed matrix.</returns>
        public static IMatrix<T> Transform<T>(
            IMatrix<T> source,
            int resultRows,
            int resultCols,
            Func<int, int, (int newRow, int newCol)> coordinateMap)
        {
            var result = new Matrix<T>(resultRows, resultCols);
            int blockSize = 64;

            Parallel.For(0, (source.RowCount + blockSize - 1) / blockSize, i =>
            {
                int rowStart = i * blockSize;
                int rowEnd = Math.Min(rowStart + blockSize, source.RowCount);
                for (int r = rowStart; r < rowEnd; r++)
                {
                    for (int c = 0; c < source.ColCount; c++)
                    {
                        var (newR, newC) = coordinateMap(r, c);
                        result[newR, newC] = source[r, c];
                    }
                }
            });

            return result;
        }
    }

}
