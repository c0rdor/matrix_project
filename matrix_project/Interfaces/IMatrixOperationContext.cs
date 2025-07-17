using matrix_project.Enums;
using matrix_project.Models.OperationOptions;
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
        // Метод для унарных операций (остается без изменений)
        Task<IMatrix<T>> ExecuteOperationAsync(
            MatrixOperationType operationType,
            IMatrix<T> matrix,
            CancellationToken cancellationToken = default,
            int blockSize = 64);

        // Универсальный метод для всех бинарных операций
        Task<IMatrix<T>> ExecuteBinaryOperationAsync<TOptions>(
            MatrixOperationType operationType,
            IMatrix<T> matrixA,
            IMatrix<T> matrixB,
            TOptions? options = null) // Принимает абстрактный класс опций
            where TOptions : MatrixBinaryOperationOptions, new(); // Constraint для создания объекта по умолчанию
    }
}
