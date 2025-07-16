using matrix_project.Enums;
using matrix_project.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace matrix_project.Context
{



    /// <summary>
    /// Context for executing matrix operations using registered strategies.
    /// </summary>
    /// <typeparam name="T">The type of elements in the matrix.</typeparam>
    public class MatrixOperationContext<T> : IMatrixOperationContext<T>
    {
        private readonly Dictionary<MatrixOperation, IMatrixOperationStrategy<T>> _strategies;
        private readonly ILogger<MatrixOperationContext<T>> _logger;

        /// <summary>
        /// Initializes a new instance of the MatrixOperationContext class.
        /// </summary>
        /// <param name="strategies">The collection of available strategies.</param>
        /// <param name="logger">The logger instance for logging operations.</param>
        public MatrixOperationContext(IEnumerable<IMatrixOperationStrategy<T>> strategies, ILogger<MatrixOperationContext<T>> logger)
        {
            _strategies = strategies.ToDictionary(s => s.OperationType);
            _logger = logger;
        }

        /// <summary>
        /// Executes a matrix operation by name.
        /// </summary>
        /// <param name="operationType">The type of the operation to execute.</param>
        /// <param name="matrix">The input matrix.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>A task that returns the transformed matrix.</returns>
        /// <exception cref="ArgumentException">Thrown if the operation type is unknown.</exception>
        public async Task<IMatrix<T>> ExecuteOperationAsync(MatrixOperation operationType, IMatrix<T> matrix, CancellationToken cancellationToken = default, int blockSize = 64)
        {
            if (!_strategies.TryGetValue(operationType, out var strategy))
            {
                _logger.LogWarning("Unknown operation: {Operation}", operationType);
                throw new ArgumentException($"Unknown operation: {operationType}");
            }

            _logger.LogInformation("Executing operation '{Operation}'", operationType);
            var sw = Stopwatch.StartNew();

            var result = await strategy.ExecuteOperationAsync(matrix, cancellationToken);

            sw.Stop();
            _logger.LogInformation("Operation '{Operation}' completed in {Elapsed}ms", operationType, sw.ElapsedMilliseconds);
            return result;
        }
    }
}
