using matrix_project.Enums;
using matrix_project.Extensions; // Убедитесь, что это пространство имен добавлено для методов расширения
using matrix_project.Interfaces;
using matrix_project.Models;
using matrix_project.Models.OperationOptions;
using matrix_project.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

/// <summary>
/// Главный класс программы, демонстрирующий операции с матрицами.
/// </summary>
class Program
{
    /// <summary>
    /// Основная точка входа для демонстрации операций с матрицами.
    /// </summary>
    /// <returns>Задача, представляющая асинхронную операцию.</returns>
    static async Task Main()
    {
        const int COLS = 100;
        const int ROWS = 100;
        var services = new ServiceCollection();

        // 1. Сначала настраиваем логирование
        // Хорошей практикой является настройка логирования на ранней стадии конфигурации DI.
        services.AddLogging(config =>
        {
            config.AddConsole();
            config.SetMinimumLevel(LogLevel.Information); // Устанавливаем желаемый уровень логирования
        });

        // 2. Регистрируем ВСЕ стратегии умножения матриц
        // Этот метод добавляет стратегии для всех поддерживаемых числовых типов (например, double, float).
        // Он вызывается один раз, так как не зависит от конкретного типа 'T'.
        services.AddAllMatrixMultiplicationStrategies();

        // 3. Регистрируем операции с матрицами для КОНКРЕТНЫХ типов, которые вы планируете использовать.
        // Ваш существующий код использует 'char' для унарных операций и контекста,
        // но стратегии умножения предназначены для числовых типов.
        // Давайте добавим операции для 'double', чтобы продемонстрировать умножение.
        services.AddMatrixOperations<double>(); // Регистрация операций и контекста для double
        services.AddMatrixOperations<char>();   // Сохраняем для унарных операций с char

        var provider = services.BuildServiceProvider();

        // Разрешаем сервисы из провайдера
        var logger = provider.GetRequiredService<ILogger<Program>>();

        // Разрешаем контекст для char (для унарных операций)
        var charContext = provider.GetRequiredService<IMatrixOperationContext<char>>();

        // Разрешаем контекст для double (для бинарных операций / умножения)
        var doubleContext = provider.GetRequiredService<IMatrixOperationContext<double>>();


        logger.LogInformation("Генерация матрицы {Rows}x{Cols} типа char", ROWS, COLS);

        var rnd = new Random();
        var charMatrixData = MatrixUtils.GenerateRandomMatrix(ROWS, COLS, () => (char)rnd.Next('A', 'B' + 1));

        IMatrix<char> charMatrix = new Matrix<char>(ROWS, COLS, (r, c) => charMatrixData[r, c]);
        MatrixUtils.PrintMatrix(charMatrix);
        Console.WriteLine(new string('-', 30));

        // Используем оптимизатор блока (его опции уже зарегистрированы через AddMatrixOperations)
        int optimalBlockSize = BlockSizeOptimizer.GetOptimalBlockSize(ROWS, COLS);
        logger.LogInformation("Используется оптимальный размер блока: {BlockSize}", optimalBlockSize);

        // --- Демонстрация унарных операций с char ---
        Console.WriteLine("\n--- Демонстрация унарных операций (char) ---");
        foreach (var op in new[] { MatrixOperationType.Rotate90, MatrixOperationType.Transpose, MatrixOperationType.Rotate90 })
        {
            logger.LogInformation("Начало унарной операции char {Operation}", op);
            var sw = Stopwatch.StartNew();

            var result = await charContext.ExecuteOperationAsync(op, charMatrix, blockSize: optimalBlockSize);

            sw.Stop();
            MatrixUtils.PrintMatrix(result);

            logger.LogInformation("Унарная операция char {Operation} завершена за {Elapsed} мс", op, sw.ElapsedMilliseconds);
            Console.WriteLine($"Время выполнения {op}: {sw.ElapsedMilliseconds} мс");
            Console.WriteLine(new string('-', 30));
        }

        // --- Демонстрация бинарных операций (умножения) с double ---
        Console.WriteLine("\n--- Демонстрация операций умножения (double) ---");

        const int MULT_ROWS_A = 1400;
        const int MULT_COLS_A = 1450; // Должно совпадать с MULT_ROWS_B
        const int MULT_ROWS_B = 1450; // Должно совпадать с MULT_COLS_A
        const int MULT_COLS_B = 1400;

        logger.LogInformation("Генерация double матриц для умножения.");
        IMatrix<double> matrixA_double = new Matrix<double>(MULT_ROWS_A, MULT_COLS_A, (r, c) => rnd.NextDouble() * 10 - 5); // Значения от -5 до 5
        IMatrix<double> matrixB_double = new Matrix<double>(MULT_ROWS_B, MULT_COLS_B, (r, c) => rnd.NextDouble() * 10 - 5);

        Console.WriteLine("Матрица A:");
        MatrixUtils.PrintMatrix(matrixA_double);
        Console.WriteLine("Матрица B:");
        MatrixUtils.PrintMatrix(matrixB_double);
        Console.WriteLine(new string('-', 30));

        // --- Умножение с Avx2Double ---
        logger.LogInformation("Запуск умножения матриц с использованием AVX2_DOUBLE");
        var swAvx2 = Stopwatch.StartNew();
        var resultAvx2 = await doubleContext.ExecuteBinaryOperationAsync(
            MatrixOperationType.Multiplication,
            matrixA_double,
            matrixB_double,
            options: new MatrixMultiplicationOptions
            {
                MultiplicationType = MatrixMultiplicationType.Avx2Double,
                BlockSize = optimalBlockSize
            });
        swAvx2.Stop();
        logger.LogInformation("AVX2_DOUBLE умножение завершено за {Elapsed} мс", swAvx2.ElapsedMilliseconds);
        Console.WriteLine($"AVX2_DOUBLE умножение: {swAvx2.ElapsedMilliseconds} мс");

        // --- Умножение с Block ---
        logger.LogInformation("Запуск умножения матриц с использованием BLOCK");
        var swBlock = Stopwatch.StartNew();
        var resultBlock = await doubleContext.ExecuteBinaryOperationAsync(
            MatrixOperationType.Multiplication,
            matrixA_double,
            matrixB_double,
            options: new MatrixMultiplicationOptions
            {
                MultiplicationType = MatrixMultiplicationType.Block,
                BlockSize = optimalBlockSize
            });
        swBlock.Stop();
        logger.LogInformation("BLOCK умножение завершено за {Elapsed} мс", swBlock.ElapsedMilliseconds);
        Console.WriteLine($"BLOCK умножение: {swBlock.ElapsedMilliseconds} мс");

        // Можно дополнительно проверить совпадение результатов (например, для теста)
        bool areEqual = MatrixUtils.AreMatricesEqual(resultAvx2, resultBlock, 1e-9);
        Console.WriteLine($"Результаты совпадают: {areEqual}");
    }
}