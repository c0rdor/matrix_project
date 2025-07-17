using matrix_project.Enums;
using matrix_project.Interfaces;
using matrix_project.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using System.Threading;
using System.Threading.Tasks;

namespace matrix_project.Strategies.Binary.MatrixMultiplication.MatrixMultiplicationStrategy
{
    public class DoubleAvx2MultiplicationStrategy : IMatrixMultiplicationStrategy<double>
    {
        private readonly ILogger<DoubleAvx2MultiplicationStrategy> _logger;

        public DoubleAvx2MultiplicationStrategy(ILogger<DoubleAvx2MultiplicationStrategy> logger)
        {
            _logger = logger;

            if (!Avx2.IsSupported || !Fma.IsSupported)
            {
                _logger.LogWarning("AVX2 или FMA не поддерживаются на данном оборудовании. " +
                                   "Использование данной стратегии может быть неэффективным.");
            }
        }

        public MatrixMultiplicationType MultiplicationType => MatrixMultiplicationType.Avx2Double;

        public MatrixOperationType OperationType => MatrixOperationType.Multiplication;

        public Task<IMatrix<double>> ExecuteOperationAsync(
            IMatrix<double> matrixA,
            IMatrix<double> matrixB,
            CancellationToken cancellationToken = default,
            int blockSize = 64)
        {
            if (matrixA.ColCount != matrixB.RowCount)
            {
                var errorMessage = "Число столбцов первой матрицы должно совпадать с числом строк второй.";
                _logger.LogError(errorMessage);
                throw new InvalidOperationException(errorMessage);
            }

            int rowsA = matrixA.RowCount;
            int colsA = matrixA.ColCount;
            int colsB = matrixB.ColCount;

            _logger.LogInformation($"Начато умножение матриц double с использованием AVX2. " +
                                   $"Размеры: A({rowsA}x{colsA}), B({colsA}x{colsB})");

            var resultMatrix = new Matrix<double>(rowsA, colsB);

            const int VectorSize = 4;

            return Task.Run(() =>
            {
                Parallel.For(0, rowsA, new ParallelOptions { CancellationToken = cancellationToken }, i =>
                {
                    Span<double> bTemp = stackalloc double[VectorSize];

                    for (int j = 0; j < colsB; j += VectorSize)
                    {
                        Vector256<double> sumVector = Vector256<double>.Zero;

                        for (int k = 0; k < colsA; k++)
                        {
                            var aValVector = Vector256.Create(matrixA[i, k]);

                            for (int v = 0; v < VectorSize; v++)
                            {
                                bTemp[v] = (j + v < colsB) ? matrixB[k, j + v] : 0.0;
                            }

                            var bVec = Vector256.Create(bTemp[0], bTemp[1], bTemp[2], bTemp[3]);
                            sumVector = Fma.MultiplyAdd(aValVector, bVec, sumVector);
                        }

                        for (int v = 0; v < VectorSize; v++)
                        {
                            if (j + v < colsB)
                                resultMatrix[i, j + v] = sumVector.GetElement(v);
                        }
                    }
                });

                _logger.LogInformation("Умножение матриц с использованием AVX2 завершено.");
                return (IMatrix<double>)resultMatrix;
            }, cancellationToken);
        }
    }
}
