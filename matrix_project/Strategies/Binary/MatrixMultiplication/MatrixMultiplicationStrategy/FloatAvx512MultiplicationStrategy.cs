using matrix_project.Enums;
using matrix_project.Interfaces;
using matrix_project.Models; // Assuming your Matrix<T> class is here
using Microsoft.Extensions.Logging;
using System;
using System.Runtime.Intrinsics; // For Vector512
using System.Runtime.Intrinsics.X86; // For Avx512F (AVX-512 floating-point instructions)
using System.Threading;
using System.Threading.Tasks;

namespace matrix_project.Strategies.Binary.MatrixMultiplication.MatrixMultiplicationStrategy
{
    public class FloatAvx512MultiplicationStrategy : IMatrixMultiplicationStrategy<float>
    {
        private readonly ILogger<FloatAvx512MultiplicationStrategy> _logger;

        public FloatAvx512MultiplicationStrategy(ILogger<FloatAvx512MultiplicationStrategy> logger)
        {
            _logger = logger;

            // Important check: ensure AVX-512 is supported on the current CPU
            if (!Avx512F.IsSupported)
            {
                _logger.LogWarning("AVX-512 (AVX512F) is not supported on this hardware. " +
                                   "Using FloatAvx512MultiplicationStrategy may lead to very slow " +
                                   "performance (due to emulation) or throw exceptions.");
                // In a real application, you might throw an exception here
                // or modify the strategy selection logic to avoid choosing this one
                // if it's not supported.
            }
        }

        // --- Interface property implementations ---
        public MatrixMultiplicationType MultiplicationType => MatrixMultiplicationType.Avx512Float;

        public MatrixOperationType OperationType => MatrixOperationType.Multiplication;

        // --- Multiplication method implementation ---
        public Task<IMatrix<float>> ExecuteOperationAsync(
            IMatrix<float> matrixA,
            IMatrix<float> matrixB,
            CancellationToken cancellationToken = default,
            int blockSize = 64) // blockSize can be used for internal block optimization
        {
            if (matrixA.ColCount != matrixB.RowCount)
            {
                var errorMessage = "The number of columns in the first matrix must match the number of rows in the second matrix.";
                _logger.LogError(errorMessage);
                throw new InvalidOperationException(errorMessage);
            }

            _logger.LogInformation($"Starting float matrix multiplication using AVX-512. " +
                                   $"Matrix A: {matrixA.RowCount}x{matrixA.ColCount}, " +
                                   $"Matrix B: {matrixB.RowCount}x{matrixB.ColCount}.");

            return Task.Run<IMatrix<float>>(() => // Explicitly specify that Task.Run returns IMatrix<float>
            {
                cancellationToken.ThrowIfCancellationRequested();

                int rowsA = matrixA.RowCount;
                int colsA = matrixA.ColCount; // Also rowsB
                int colsB = matrixB.ColCount;

                // Create the result matrix
                var resultMatrix = new Matrix<float>(rowsA, colsB);

                // AVX-512 Vector512<float> contains 16 float elements.
                // Operations will be performed in blocks of 16 elements.
                const int VectorSize = 16;

                // Iterate through rows of matrix A
                for (int i = 0; i < rowsA; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    // Iterate through columns of matrix B, stepping by VectorSize
                    for (int j = 0; j < colsB; j += VectorSize)
                    {
                        // Initialize a vector for accumulation (16 zeros)
                        Vector512<float> sumVector = Vector512<float>.Zero;

                        // Iterate through the "inner" dimension (columns of A / rows of B)
                        for (int k = 0; k < colsA; k++)
                        {
                            // 1. Load the scalar value from A[i, k] and "broadcast" it to the entire vector.
                            // This value will be multiplied by each element of the vector from matrix B.
                            Vector512<float> a_val_vector = Vector512.Create(matrixA[i, k]);

                            // 2. Load 16 elements from matrix B.
                            // This part is a HUGE SIMPLIFICATION for demonstration.
                            // In a real AVX-512 optimized implementation, you would use UNSAFE code
                            // with direct access to aligned memory, for example:
                            // Vector512<float> b_vec = Avx512F.LoadVector512(matrixB.GetDataPointer(k, j));
                            //
                            // Since IMatrix<T> doesn't provide direct pointer access,
                            // we emulate vector loading manually, which is inefficient but demonstrates the concept.
                            float[] b_elements_temp = new float[VectorSize];
                            for (int v = 0; v < VectorSize; v++)
                            {
                                if (j + v < colsB)
                                {
                                    b_elements_temp[v] = matrixB[k, j + v];
                                }
                                else
                                {
                                    // If there aren't enough elements (tail), pad with zeros
                                    // to avoid out-of-bounds access and not affect the sum.
                                    b_elements_temp[v] = 0.0f;
                                }
                            }
                            Vector512<float> b_vec = Vector512.Create(b_elements_temp);

                            // 3. Perform the Fused Multiply-Add (FMA) operation: (A * B) + Sum
                            // This is an extremely efficient AVX-512 instruction.
                            sumVector = Avx512F.FusedMultiplyAdd(a_val_vector, b_vec, sumVector);
                        }

                        // 4. Store the result back into the result matrix.
                        // Careful handling of "tails" is also needed here if colsB is not a multiple of VectorSize.
                        for (int v = 0; v < VectorSize; v++)
                        {
                            if (j + v < colsB)
                            {
                                resultMatrix[i, j + v] = sumVector.GetElement(v);
                            }
                        }
                    }
                }

                _logger.LogInformation("Float matrix multiplication using AVX-512 completed.");
                // Explicitly cast to IMatrix<float> before returning
                return (IMatrix<float>)resultMatrix;
            }, cancellationToken);
        }
    }
}