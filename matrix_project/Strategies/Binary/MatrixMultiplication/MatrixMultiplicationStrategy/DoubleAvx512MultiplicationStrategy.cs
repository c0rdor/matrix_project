using matrix_project.Enums;
using matrix_project.Interfaces;
using matrix_project.Models; // Предполагается, что ваш класс Matrix<T> находится здесь
using Microsoft.Extensions.Logging;
using System;
using System.Runtime.Intrinsics; // Для Vector512
using System.Runtime.Intrinsics.X86; // Для Avx512F (инструкции AVX-512 для чисел с плавающей точкой)
using System.Threading;
using System.Threading.Tasks;

namespace matrix_project.Strategies.Binary.MatrixMultiplication.MatrixMultiplicationStrategy
{
    public class DoubleAvx512MultiplicationStrategy : IMatrixMultiplicationStrategy<double>
    {
        private readonly ILogger<DoubleAvx512MultiplicationStrategy> _logger;

        public DoubleAvx512MultiplicationStrategy(ILogger<DoubleAvx512MultiplicationStrategy> logger)
        {
            _logger = logger;

            if (!Avx512F.IsSupported)
            {
                _logger.LogWarning("AVX-512 (AVX512F) не поддерживается на данном оборудовании. " +
                                   "Использование DoubleAvx512MultiplicationStrategy может привести к очень медленной " +
                                   "работе (из-за эмуляции) или выбросу исключений.");
            }
        }

        public MatrixMultiplicationType MultiplicationType => MatrixMultiplicationType.Avx512Double;

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

            _logger.LogInformation($"Начато умножение матриц double с использованием AVX-512. " +
                                   $"Матрица A: {matrixA.RowCount}x{matrixA.ColCount}, " +
                                   $"Матрица B: {matrixB.RowCount}x{matrixB.ColCount}.");

            return Task.Run<IMatrix<double>>(() => // Явно указываем, что Task.Run возвращает IMatrix<double>
            {
                cancellationToken.ThrowIfCancellationRequested();

                int rowsA = matrixA.RowCount;
                int colsA = matrixA.ColCount;
                int colsB = matrixB.ColCount;

                var resultMatrix = new Matrix<double>(rowsA, colsB);

                const int VectorSize = 8;

                for (int i = 0; i < rowsA; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    for (int j = 0; j < colsB; j += VectorSize)
                    {
                        Vector512<double> sumVector = Vector512<double>.Zero;

                        for (int k = 0; k < colsA; k++)
                        {
                            Vector512<double> a_val_vector = Vector512.Create(matrixA[i, k]);

                            double[] b_elements_temp = new double[VectorSize];
                            for (int v = 0; v < VectorSize; v++)
                            {
                                if (j + v < colsB)
                                {
                                    b_elements_temp[v] = matrixB[k, j + v];
                                }
                                else
                                {
                                    b_elements_temp[v] = 0.0;
                                }
                            }
                            Vector512<double> b_vec = Vector512.Create(b_elements_temp);

                            sumVector = Avx512F.FusedMultiplyAdd(a_val_vector, b_vec, sumVector);
                        }

                        for (int v = 0; v < VectorSize; v++)
                        {
                            if (j + v < colsB)
                            {
                                resultMatrix[i, j + v] = sumVector.GetElement(v);
                            }
                        }
                    }
                }

                _logger.LogInformation("Умножение матриц с использованием AVX-512 завершено.");
                // Явное приведение типа перед возвратом
                return (IMatrix<double>)resultMatrix;
            }, cancellationToken);
        }
    }
}