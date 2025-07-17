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

    public interface IMatrixBinaryOperatorStrategy<T>
    {
        MatrixOperationType OperationType { get; }
        Task<IMatrix<T>> ExecuteOperationAsync(IMatrix<T> matrixA, IMatrix<T> matrixB, CancellationToken cancellationToken = default, int blockSize = 64);
    }

}
