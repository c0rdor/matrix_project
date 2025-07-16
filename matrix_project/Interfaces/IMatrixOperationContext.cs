using matrix_project.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace matrix_project.Interfaces
{
    /// <summary>
    /// Defines a context for executing matrix operations.
    /// </summary>
    /// <typeparam name="T">The type of elements in the matrix.</typeparam>
    public interface IMatrixOperationContext<T>
    {
        /// <summary>
        /// Executes a matrix operation by name.
        /// </summary>
        /// <param name="operationType">The type of the operation to execute.</param>
        /// <param name="matrix">The input matrix.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>A task that returns the transformed matrix.</returns>
        Task<IMatrix<T>> ExecuteOperationAsync(MatrixOperation operationType, IMatrix<T> matrix, CancellationToken cancellationToken = default, int blockSize = 64);
    }
}
