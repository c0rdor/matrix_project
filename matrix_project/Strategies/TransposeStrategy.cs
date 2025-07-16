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
    /// Strategy for transposing a matrix.
    /// </summary>
    /// <typeparam name="T">The type of elements in the matrix.</typeparam>
    public class TransposeStrategy<T> : IMatrixOperationStrategy<T>
    {
        /// <summary>
        /// Gets the type of the operation.
        /// </summary>
        public MatrixOperation OperationType => MatrixOperation.Transpose;

        /// <summary>
        /// Transposes the matrix (swaps rows and columns).
        /// </summary>
        /// <param name="matrix">The input matrix.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>A task that returns the transposed matrix.</returns>
        public Task<IMatrix<T>> ExecuteOperationAsync(IMatrix<T> matrix, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(MatrixBlockTransformer.Transform(matrix, matrix.ColCount, matrix.RowCount, (r, c) => (c, r)));
        }
    }
}
