using matrix_project.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace matrix_project.Interfaces
{
    public interface IMatrixUnaryOperatorStrategy<T>
    {
        MatrixOperationType OperationType { get; }
        Task<IMatrix<T>> ExecuteOperationAsync(IMatrix<T> matrix, CancellationToken cancellationToken = default, int blockSize = 64);
    }
}
