using matrix_project.Enums;
using matrix_project.Interfaces;
using matrix_project.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace matrix_project.Strategies
{
    /// <summary>
    /// Strategy for rotating a matrix 180 degrees.
    /// </summary>
    /// <typeparam name="T">The type of elements in the matrix.</typeparam>
    public class Rotate180Strategy<T> : IMatrixOperationStrategy<T>
    {
        /// <summary>
        /// Gets the type of the operation.
        /// </summary>
        public MatrixOperation OperationType => MatrixOperation.Rotate180;

        /// <summary>
        /// Rotates the matrix 180 degrees.
        /// </summary>
        /// <param name="matrix">The input matrix.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>A task that returns the rotated matrix.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the matrix is not square.</exception>
        public Task<IMatrix<T>> ExecuteOperationAsync(IMatrix<T> matrix, CancellationToken cancellationToken = default, int blockSize=64)
        {
            if (matrix.RowCount != matrix.ColCount)
                throw new InvalidOperationException("Matrix must be square for rotation");

            int n = matrix.RowCount;
            return Task.FromResult(MatrixBlockTransformer.Transform(matrix, n, n, (r, c) => (n - 1 - r, matrix.ColCount - 1 - c), blockSize));
        }
    }

}
