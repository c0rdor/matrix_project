using matrix_project.Enums;
using System.Collections.Generic; // Убедитесь, что это пространство имен добавлено

namespace matrix_project.Interfaces
{
    /// <summary>
    /// Определяет селектор для выбора стратегии умножения матриц.
    /// </summary>
    /// <typeparam name="T">Тип элементов матрицы.</typeparam>
    public interface IMultiplicationStrategySelector<T>
    {
        /// <summary>
        /// Выбирает подходящую стратегию умножения матриц.
        /// </summary>
        /// <param name="multiplicationType">Желаемый тип умножения. По умолчанию Auto, что позволяет селектору выбрать лучшую доступную стратегию.</param>
        /// <returns>Экземпляр IMatrixMultiplicationStrategy<T>.</returns>
        /// <exception cref="NotSupportedException">Выбрасывается, если запрошенная стратегия не найдена или не поддерживается.</exception>
        IMatrixMultiplicationStrategy<T> SelectStrategy(
            MatrixMultiplicationType multiplicationType = MatrixMultiplicationType.Auto); // <-- НОВОЕ: опциональный параметр
    }
}