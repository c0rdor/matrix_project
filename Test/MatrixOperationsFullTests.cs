using matrix_project.Enums;
using matrix_project.Extensions;
using matrix_project.Interfaces;
using matrix_project.Models;
using matrix_project.Models.OperationOptions;
using matrix_project.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Xunit;

public class MatrixOperationsFullTests
{
    private readonly IServiceProvider _provider;
    private readonly IMatrixOperationContext<char> _charContext;
    private readonly IMatrixOperationContext<double> _doubleContext;

    public MatrixOperationsFullTests()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddAllMatrixMultiplicationStrategies();
        services.AddMatrixOperations<char>();
        services.AddMatrixOperations<double>();

        _provider = services.BuildServiceProvider();
        _charContext = _provider.GetRequiredService<IMatrixOperationContext<char>>();
        _doubleContext = _provider.GetRequiredService<IMatrixOperationContext<double>>();
    }

    [Fact]
    public async Task UnaryOperations_Char_ShouldProduceExpectedResults()
    {
        // Простая 3x3 матрица  
        char[,] data =
        {
            { 'A', 'B', 'C' },
            { 'D', 'E', 'F' },
            { 'G', 'H', 'I' }
        };
        IMatrix<char> matrix = new Matrix<char>(3, 3, (r, c) => data[r, c]);

        // Rotate90  
        var result90 = await _charContext.ExecuteOperationAsync(MatrixOperationType.Rotate90, matrix);
        IMatrix<char> expected90 = new Matrix<char>(3, 3, (r, c) => new char[,]
        {
            { 'G', 'D', 'A' },
            { 'H', 'E', 'B' },
            { 'I', 'F', 'C' }
        }[r, c]);
        Assert.True(MatrixUtils.AreMatricesEqual(result90, expected90));

        // Rotate180  
        var result180 = await _charContext.ExecuteOperationAsync(MatrixOperationType.Rotate180, matrix);
        IMatrix<char> expected180 = new Matrix<char>(3, 3, (r, c) => new char[,]
        {
            { 'I', 'H', 'G' },
            { 'F', 'E', 'D' },
            { 'C', 'B', 'A' }
        }[r, c]);
        Assert.True(MatrixUtils.AreMatricesEqual(result180, expected180));

        // Rotate270  
        var result270 = await _charContext.ExecuteOperationAsync(MatrixOperationType.Rotate270, matrix);
        IMatrix<char> expected270 = new Matrix<char>(3, 3, (r, c) => new char[,]
        {
            { 'C', 'F', 'I' },
            { 'B', 'E', 'H' },
            { 'A', 'D', 'G' }
        }[r, c]);
        Assert.True(MatrixUtils.AreMatricesEqual(result270, expected270));

        // Transpose  
        var resultTranspose = await _charContext.ExecuteOperationAsync(MatrixOperationType.Transpose, matrix);
        IMatrix<char> expectedTranspose = new Matrix<char>(3, 3, (r, c) => new char[,]
        {
            { 'A', 'D', 'G' },
            { 'B', 'E', 'H' },
            { 'C', 'F', 'I' }
        }[r, c]);
        Assert.True(MatrixUtils.AreMatricesEqual(resultTranspose, expectedTranspose));
    }

    [Fact]
    public async Task Multiplication_Double_ShouldProduceSameResultForDifferentMethods()
    {
        const int ROWS_A = 5;
        const int COLS_A = 4;
        const int ROWS_B = 4;
        const int COLS_B = 5;

        var rnd = new Random();
        IMatrix<double> matrixA = new Matrix<double>(ROWS_A, COLS_A, (r, c) => rnd.NextDouble() * 10 - 5);
        IMatrix<double> matrixB = new Matrix<double>(ROWS_B, COLS_B, (r, c) => rnd.NextDouble() * 10 - 5);

        int blockSize = 2;

        // Умножение Avx2Double
        IMatrix<double> resultAvx2 = await _doubleContext.ExecuteBinaryOperationAsync(
            MatrixOperationType.Multiplication,
            matrixA,
            matrixB,
            new MatrixMultiplicationOptions
            {
                MultiplicationType = MatrixMultiplicationType.Avx2Double,
                BlockSize = blockSize
            });

        // Умножение Block
        IMatrix<double> resultBlock = await _doubleContext.ExecuteBinaryOperationAsync(
            MatrixOperationType.Multiplication,
            matrixA,
            matrixB,
            new MatrixMultiplicationOptions
            {
                MultiplicationType = MatrixMultiplicationType.Block,
                BlockSize = blockSize
            });

        // Проверка совпадения результатов с допустимой погрешностью
        Assert.True(MatrixUtils.AreMatricesEqual(resultAvx2, resultBlock, 1e-9),
            "Результаты умножения матриц Avx2 и Block должны совпадать.");
    }
}
