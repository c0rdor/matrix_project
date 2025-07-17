using matrix_project.Context;
using matrix_project.Extensions; // Добавляем using для MultiplicationExtensions
using matrix_project.Infrastructure.Hardware;
using matrix_project.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using static BlockSizeOptimizer; // Убедитесь, что этот класс корректно определён и доступен

namespace matrix_project.Extensions
{
    public static class CoreExtensions
    {
        /// <summary>
        /// Добавляет основные операции с матрицами для заданного типа T,
        /// а также регистрирует все общие стратегии умножения.
        /// </summary>
        /// <typeparam name="T">Тип элементов матрицы (например, double, float).</typeparam>
        /// <param name="services">Коллекция сервисов.</param>
        /// <param name="options">Опции для оптимизатора размера блока.</param>
        /// <returns>Коллекция сервисов для цепочки вызовов.</returns>
        public static IServiceCollection AddMatrixOperations<T>(
            this IServiceCollection services,
            BlockSizeOptimizerOptions? options = null)
        {
            // Регистрируем унарные операции для КОНКРЕТНОГО типа T, переданного в AddMatrixOperations<T>
            services.AddMatrixUnaryOperations<T>();

            // Регистрируем контекст операции для КОНКРЕТНОГО типа T
            services.AddSingleton<IMatrixOperationContext<T>, MatrixOperationContext<T>>();
            services.AddSingleton<IHardwareCapabilitiesChecker, HardwareCapabilitiesChecker>();
       

            // Опции оптимизатора размера блока
            var blockSizeOptions = options ?? new BlockSizeOptimizerOptions
            {
                BaseBlockSize = 64,
                MinElementsForParallelism = 4096,
                AdjustForProcessorCount = true,
                Logger = Console.WriteLine // Убедитесь, что Console доступен, или используйте ILogger
            };

            services.AddSingleton(blockSizeOptions);
            BlockSizeOptimizer.SetOptions(blockSizeOptions); // Установка глобальных опций

            return services;
        }

        /// <summary>
        /// Добавляет все стратегии умножения матриц (для всех поддерживаемых типов, таких как double, float)
        /// и связанные селекторы/операторы. Этот метод вызывается один раз.
        /// </summary>
        /// <param name="services">Коллекция сервисов.</param>
        /// <returns>Коллекция сервисов для цепочки вызовов.</returns>
        public static IServiceCollection AddAllMatrixMultiplicationStrategies(this IServiceCollection services)
        {
            // Вызываем ваш метод расширения для регистрации всех стратегий умножения
            // (для double, float и т.д., как определено внутри MultiplicationExtensions)
            services.AddMatrixMultiplicationStrategies();

            return services;
        }
    }
}