using matrix_project.Enums;
using matrix_project.Interfaces;
using matrix_project.Models.OperationOptions; // Убедитесь, что этот using добавлен для классов опций
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading; // Убедитесь, что CancellationToken доступен
using System.Threading.Tasks;

namespace matrix_project.Context
{
    public class MatrixOperationContext<T> : IMatrixOperationContext<T>
    {
        private readonly ILogger<MatrixOperationContext<T>> _logger;
        private readonly IMultiplicationStrategySelector<T> _multiplicationSelector;
        // Добавьте здесь другие селекторы стратегий по мере их появления:
        // private readonly IAdditionStrategy<T> _additionStrategy; // Если сложение имеет одну стратегию без выбора
        // private readonly ISubtractionStrategy<T> _subtractionStrategy; // Для вычитания
        // private readonly IAdditionStrategySelector<T> _additionSelector; // Если сложение тоже будет иметь несколько алгоритмов

        private readonly IEnumerable<IMatrixUnaryOperatorStrategy<T>> _unaryStrategies;

        public MatrixOperationContext(
            ILogger<MatrixOperationContext<T>> logger,
            IMultiplicationStrategySelector<T> multiplicationSelector,
            IEnumerable<IMatrixUnaryOperatorStrategy<T>> unaryStrategies
            /* Добавляйте сюда другие зависимости для бинарных операций */)
        {
            _logger = logger;
            _multiplicationSelector = multiplicationSelector;
            _unaryStrategies = unaryStrategies;

            _logger.LogInformation($"MatrixOperationContext<{typeof(T).Name}> инициализирован.");
        }

        public Task<IMatrix<T>> ExecuteOperationAsync(
            MatrixOperationType operationType,
            IMatrix<T> matrix,
            CancellationToken cancellationToken = default,
            int blockSize = 64)
        {
            // Находим подходящую унарную стратегию по типу операции
            var strategy = _unaryStrategies.FirstOrDefault(s => s.OperationType == operationType);

            if (strategy == null)
            {
                _logger.LogError($"Унарная стратегия для операции '{operationType}' не найдена или не зарегистрирована для типа {typeof(T).Name}.");
                throw new NotSupportedException($"Унарная операция '{operationType}' не поддерживается.");
            }

            _logger.LogInformation($"Выполнение унарной операции '{operationType}' с матрицей {matrix.RowCount}x{matrix.ColCount}.");
            return strategy.ExecuteOperationAsync(matrix, cancellationToken, blockSize);
        }

        public Task<IMatrix<T>> ExecuteBinaryOperationAsync<TOptions>(
            MatrixOperationType operationType,
            IMatrix<T> matrixA,
            IMatrix<T> matrixB,
            TOptions? options = null)
            where TOptions : MatrixBinaryOperationOptions, new()
        {
            // Если опции не предоставлены, создаём экземпляр по умолчанию.
            // Это гарантирует, что 'options' никогда не будет null внутри метода.
            options ??= new TOptions();

            switch (operationType)
            {
                case MatrixOperationType.Multiplication:
                    // Проверяем, что предоставленные опции являются MatrixMultiplicationOptions.
                    // Это важно для доступа к специфическим параметрам умножения.
                    if (options is MatrixMultiplicationOptions multOptions)
                    {
                        _logger.LogInformation($"Выполнение умножения матриц. Тип умножения: {multOptions.MultiplicationType}.");
                        // Делегируем выбор конкретной стратегии умножения селектору.
                        // Селектор вернёт стратегию, основываясь на multOptions.MultiplicationType.
                        var multiplicationStrategy = _multiplicationSelector.SelectStrategy(multOptions.MultiplicationType);
                        return multiplicationStrategy.ExecuteOperationAsync(
                            matrixA, matrixB, multOptions.CancellationToken, multOptions.BlockSize);
                    }
                    else
                    {
                        // Логическая ошибка: переданы неверные опции для умножения.
                        _logger.LogError($"Для операции умножения ожидались опции типа {nameof(MatrixMultiplicationOptions)}, но получены опции типа {options.GetType().Name}.");
                        throw new ArgumentException($"Неверный тип опций для операции '{operationType}'. Ожидался {nameof(MatrixMultiplicationOptions)}.");
                    }

                // Пример, как можно добавить другие бинарные операции:
                // case MatrixOperationType.Addition:
                //     if (options is MatrixAdditionOptions addOptions)
                //     {
                //         _logger.LogInformation($"Выполнение сложения матриц. Использование параллелизма: {addOptions.UseParallelism}.");
                //         // Здесь вы бы вызывали стратегию сложения:
                //         // var additionStrategy = _additionSelector.SelectStrategy(addOptions.AdditionType); // Если есть селектор
                //         // return additionStrategy.ExecuteOperationAsync(matrixA, matrixB, addOptions.CancellationToken, addOptions.BlockSize);
                //         throw new NotImplementedException("Операция сложения пока не реализована.");
                //     }
                //     else
                //     {
                //         _logger.LogError($"Для операции сложения ожидались опции типа {nameof(MatrixAdditionOptions)}, но получены опции типа {options.GetType().Name}.");
                //         throw new ArgumentException($"Неверный тип опций для операции '{operationType}'. Ожидался {nameof(MatrixAdditionOptions)}.");
                //     }

                default:
                    // Если запрошенная операция не распознана или не реализована.
                    _logger.LogError($"Бинарная операция '{operationType}' не поддерживается или не реализована для типа {typeof(T).Name}.");
                    throw new NotSupportedException($"Бинарная операция '{operationType}' не поддерживается.");
            }
        }
    }
}