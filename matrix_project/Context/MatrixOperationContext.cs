using matrix_project.Enums;
using matrix_project.Interfaces;
using matrix_project.Models.OperationOptions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace matrix_project.Context
{
    /// <summary>
    /// Контекст выполнения операций над матрицами.
    /// Выбирает подходящую стратегию для унарных и бинарных операций.
    /// </summary>
    /// <typeparam name="T">Тип элементов матрицы.</typeparam>
    public class MatrixOperationContext<T> : IMatrixOperationContext<T>
    {
        private readonly ILogger<MatrixOperationContext<T>> _logger;
        private readonly IMultiplicationStrategySelector<T> _multiplicationSelector;
        private readonly IReadOnlyDictionary<MatrixOperationType, IMatrixUnaryOperatorStrategy<T>> _unaryStrategies;

        /// <summary>
        /// Создаёт контекст операций над матрицами.
        /// </summary>
        public MatrixOperationContext(
            ILogger<MatrixOperationContext<T>> logger,
            IMultiplicationStrategySelector<T> multiplicationSelector,
            IEnumerable<IMatrixUnaryOperatorStrategy<T>> unaryStrategies)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _multiplicationSelector = multiplicationSelector ?? throw new ArgumentNullException(nameof(multiplicationSelector));
            if (unaryStrategies == null) throw new ArgumentNullException(nameof(unaryStrategies));

            _unaryStrategies = unaryStrategies.ToDictionary(s => s.OperationType);

            _logger.LogInformation($"MatrixOperationContext<{typeof(T).Name}> initialized with {_unaryStrategies.Count} unary strategies.");
        }

        /// <summary>
        /// Выполняет унарную операцию над матрицей.
        /// </summary>
        public Task<IMatrix<T>> ExecuteOperationAsync(
            MatrixOperationType operationType,
            IMatrix<T> matrix,
            CancellationToken cancellationToken = default,
            int blockSize = 64)
        {
            if (matrix == null) throw new ArgumentNullException(nameof(matrix));

            if (!_unaryStrategies.TryGetValue(operationType, out var strategy))
            {
                var supported = string.Join(", ", _unaryStrategies.Keys);
                _logger.LogError("Unary strategy for operation '{Operation}' not found. Supported: {Supported}.", operationType, supported);
                throw new NotSupportedException($"Unary operation '{operationType}' is not supported. Supported: {supported}");
            }

            _logger.LogInformation("Executing unary operation '{Operation}' on matrix {Rows}x{Cols}.",
                operationType, matrix.RowCount, matrix.ColCount);

            return strategy.ExecuteOperationAsync(matrix, cancellationToken, blockSize);
        }

        /// <summary>
        /// Выполняет бинарную операцию над матрицами.
        /// </summary>
        public Task<IMatrix<T>> ExecuteBinaryOperationAsync<TOptions>(
            MatrixOperationType operationType,
            IMatrix<T> matrixA,
            IMatrix<T> matrixB,
            TOptions? options = null)
            where TOptions : MatrixBinaryOperationOptions, new()
        {
            if (matrixA == null) throw new ArgumentNullException(nameof(matrixA));
            if (matrixB == null) throw new ArgumentNullException(nameof(matrixB));

            options ??= new TOptions();

            return operationType switch
            {
                MatrixOperationType.Multiplication when options is MatrixMultiplicationOptions multOptions =>
                    ExecuteMultiplication(matrixA, matrixB, multOptions),

                _ => throw new NotSupportedException(
                    $"Binary operation '{operationType}' is not supported for type {typeof(T).Name}.")
            };
        }

        /// <summary>
        /// Выполняет умножение матриц с проверкой соответствия размеров.
        /// </summary>
        private Task<IMatrix<T>> ExecuteMultiplication(
            IMatrix<T> matrixA,
            IMatrix<T> matrixB,
            MatrixMultiplicationOptions options)
        {
            if (matrixA.ColCount != matrixB.RowCount)
            {
                _logger.LogError(
                    "Cannot multiply matrices: A({RowsA}x{ColsA}) and B({RowsB}x{ColsB}) have incompatible dimensions.",
                    matrixA.RowCount, matrixA.ColCount, matrixB.RowCount, matrixB.ColCount);
                throw new InvalidOperationException("Matrix dimensions are incompatible for multiplication.");
            }

            _logger.LogInformation(
                "Executing matrix multiplication {RowsA}x{ColsA} * {RowsB}x{ColsB} using {StrategyType}.",
                matrixA.RowCount, matrixA.ColCount, matrixB.RowCount, matrixB.ColCount, options.MultiplicationType);

            var strategy = _multiplicationSelector.SelectStrategy(options.MultiplicationType)
                           ?? throw new InvalidOperationException($"No strategy found for multiplication type {options.MultiplicationType}");

            return strategy.ExecuteOperationAsync(matrixA, matrixB, options.CancellationToken, options.BlockSize);
        }
    }
}
