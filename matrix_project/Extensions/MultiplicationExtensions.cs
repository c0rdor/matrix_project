using matrix_project.Enums;
using matrix_project.Interfaces;
using matrix_project.Strategies.Binary.MatrixMultiplication.MultiplicationFactory;
using matrix_project.Strategies.Binary.MatrixMultiplication.MatrixMultiplicationStrategy;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace matrix_project.Extensions
{
    public static class MultiplicationExtensions
    {
        /// <summary>
        /// Добавляет все необходимые стратегии умножения матриц и селекторы в IServiceCollection.
        /// </summary>
        /// <param name="services">Коллекция сервисов, в которую будут добавлены сервисы.</param>
        /// <returns>Коллекция сервисов для цепочки вызовов.</returns>
        public static IServiceCollection AddMatrixMultiplicationStrategies(this IServiceCollection services)
        {
            // Регистрация IStrategyRegistrar для double
            services.AddSingleton<IStrategyRegistrar, DoubleStrategyRegistrar>();

            // Регистрация IStrategyRegistrar для float
            services.AddSingleton<IStrategyRegistrar, FloatStrategyRegistrar>();

            // Регистрация всех стратегий через регистраторы
            // Получаем все зарегистрированные IStrategyRegistrar и вызываем их метод Register
            foreach (var registrar in services.BuildServiceProvider().GetServices<IStrategyRegistrar>())
            {
                registrar.Register(services);
            }

            // Регистрация селекторов стратегий
            // Это позволит получать IMultiplicationStrategySelector<double> или IMultiplicationStrategySelector<float>
            // в зависимости от запрашиваемого типа T
            services.AddSingleton(typeof(IMultiplicationStrategySelector<>), typeof(MultiplicationStrategySelector<>));

            // Регистрация стратегии оператора умножения, которая использует селектор
            // Это позволит получать IMatrixBinaryOperatorStrategy<double> или IMatrixBinaryOperatorStrategy<float>
            // в зависимости от запрашиваемого типа T
            services.AddSingleton(typeof(IMatrixBinaryOperatorStrategy<>), typeof(MultiplicationOperatorStrategy<>));
            

            return services;
        }

        /// <summary>
        /// Добавляет конкретную стратегию умножения для указанного типа T и типа умножения.
        /// Этот метод может быть использован для добавления новых стратегий без изменения основного метода AddMatrixMultiplicationStrategies.
        /// </summary>
        /// <typeparam name="T">Тип элементов матрицы (например, double, float).</typeparam>
        /// <typeparam name="TStrategy">Тип стратегии, реализующий IMatrixMultiplicationStrategy<T>.</typeparam>
        /// <param name="services">Коллекция сервисов, в которую будет добавлена стратегия.</param>
        /// <returns>Коллекция сервисов для цепочки вызовов.</returns>
        public static IServiceCollection AddMatrixMultiplicationStrategy<T, TStrategy>(this IServiceCollection services)
            where TStrategy : class, IMatrixMultiplicationStrategy<T>
        {
            services.AddSingleton<IMatrixMultiplicationStrategy<T>, TStrategy>();
            return services;
        }
    }
}