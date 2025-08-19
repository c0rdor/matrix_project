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

            // Для double достаточно AVX; AVX2 — про целые, но ок оставить как «минимальные требования».
            if (!Avx.IsSupported)
            {
                _logger.LogWarning("AVX не поддерживается на данном оборудовании. " +
                                   "Использование данной стратегии может быть неэффективным.");
            }
            if (!Fma.IsSupported)
            {
                _logger.LogInformation("FMA не поддерживается — будет использовано AVX (mul + add).");
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

            _logger.LogInformation($"Начато умножение матриц double с использованием AVX. " +
                                   $"Размеры: A({rowsA}x{colsA}), B({colsA}x{colsB})");

            var resultMatrix = new Matrix<double>(rowsA, colsB);

            const int VectorSize = 4; // 256 бит / 64 бита = 4 double

            return Task.Run(() =>
            {
                var po = new ParallelOptions { CancellationToken = cancellationToken };

                Parallel.For(0, rowsA, po, i =>
                {
                    Span<double> bTemp = stackalloc double[VectorSize];

                    for (int j = 0; j < colsB; j += VectorSize)
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        Vector256<double> sumVector = Vector256<double>.Zero;

                        for (int k = 0; k < colsA; k++)
                        {
                            var aValVector = Vector256.Create(matrixA[i, k]);

                         
                            for (int v = 0; v < VectorSize; v++)
                                bTemp[v] = (j + v < colsB) ? matrixB[k, j + v] : 0.0;

                            var bVec = Vector256.Create(bTemp[0], bTemp[1], bTemp[2], bTemp[3]);

                            // FMA если доступен, иначе AVX mul+add
                            if (Fma.IsSupported)
                                sumVector = Fma.MultiplyAdd(aValVector, bVec, sumVector);
                            else
                                sumVector = Avx.Add(sumVector, Avx.Multiply(aValVector, bVec));
                        }

                        
                        for (int v = 0; v < VectorSize; v++)
                        {
                            int col = j + v;
                            if (col < colsB)
                                resultMatrix[i, col] = sumVector.GetElement(v);
                        }
                    }
                });

                _logger.LogInformation("Умножение матриц с использованием AVX завершено.");
                return (IMatrix<double>)resultMatrix;
            }, cancellationToken);
        }
    }
}