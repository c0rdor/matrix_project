using matrix_project.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace matrix_project.Interfaces
{

    /// <summary>
    /// Defines a strategy for performing matrix operations.
    /// </summary>
    /// <typeparam name="T">The type of elements in the matrix.</typeparam>
    public interface IMatrixOperationStrategy<T>
    {
        /// <summary>
        /// Gets the name of the operation.
        /// </summary>
        MatrixOperation OperationType { get; }

        /// <summary>
        /// Executes the matrix operation asynchronously.
        /// </summary>
        /// <param name="matrix">The input matrix.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>A task that returns the transformed matrix.</returns>
        Task<IMatrix<T>> ExecuteOperationAsync(IMatrix<T> matrix, CancellationToken cancellationToken = default);
    }
}
