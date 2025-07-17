using matrix_project.Enums;
using matrix_project.Interfaces;
using matrix_project.Utils;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

namespace matrix_project.Strategies.Binary.MatrixMultiplication.MatrixMultiplicationStrategy
{
    /// <summary>
    /// Strategy for matrix multiplication using block-based parallelism.
    /// </summary>
    public class NumberBlockMatrixMultiplicationStrategy<T> : IMatrixMultiplicationStrategy<T>
        where T : INumber<T>
    {
        public MatrixOperationType OperationType => MatrixOperationType.Multiplication;

        public MatrixMultiplicationType MultiplicationType => MatrixMultiplicationType.Block;

        /// <summary>
        /// Executes block-based parallel matrix multiplication.
        /// </summary>
        public Task<IMatrix<T>> ExecuteOperationAsync(
            IMatrix<T> matrixA,
            IMatrix<T> matrixB,
            CancellationToken cancellationToken = default,
            int blockSize = 64)
        {
            if (matrixA.ColCount != matrixB.RowCount)
                throw new InvalidOperationException("Число столбцов первой матрицы должно совпадать с числом строк второй.");

            return Task.Run(() =>
            {
                return MatrixBlockMultiplier.Multiply(matrixA, matrixB, cancellationToken, blockSize);
            }, cancellationToken);
        }

        public Task<IMatrix<T>> MultiplyAsync(IMatrix<T> matrixA, IMatrix<T> matrixB, CancellationToken cancellationToken = default, int blockSize = 64)
        {
            throw new NotImplementedException();
        }
    }
}
