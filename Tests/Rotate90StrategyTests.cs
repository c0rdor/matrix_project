using matrix_project.Enums;
using matrix_project.Interfaces;
using matrix_project.Models;
using matrix_project.Strategies;

namespace Tests
{
    public class Rotate90StrategyTests
    {
        /// <summary>
        /// Тест на успешное вращение квадратной матрицы 90 градусов по часовой стрелке.
        /// </summary>
        [Fact] 
        public async Task ExecuteOperationAsync_SquareMatrix_RotatesCorrectly()
        {
            // Arrange
            // Создаем матрицу 3x3
            IMatrix<double> originalMatrix = new Matrix<double>(3, 3, (r, c) => (r * 3 + c + 1));
            // Original:
            // 1  2  3
            // 4  5  6
            // 7  8  9

            // Ожидаемый результат после поворота на 90 градусов по часовой стрелке
            IMatrix<double> expectedMatrix = new Matrix<double>(3, 3);
            expectedMatrix[0, 0] = 7; expectedMatrix[0, 1] = 4; expectedMatrix[0, 2] = 1;
            expectedMatrix[1, 0] = 8; expectedMatrix[1, 1] = 5; expectedMatrix[1, 2] = 2;
            expectedMatrix[2, 0] = 9; expectedMatrix[2, 1] = 6; expectedMatrix[2, 2] = 3;
            // Expected:
            // 7  4  1
            // 8  5  2
            // 9  6  3

            var strategy = new Rotate90Strategy<double>();

            // Act
            IMatrix<double> resultMatrix = await strategy.ExecuteOperationAsync(originalMatrix);

            // Assert
   
            Assert.Equal(originalMatrix.ColCount, resultMatrix.RowCount); // Проверка количества строк
            Assert.Equal(originalMatrix.RowCount, resultMatrix.ColCount); // Проверка количества столбцов

            for (int r = 0; r < resultMatrix.RowCount; r++)
            {
                for (int c = 0; c < resultMatrix.ColCount; c++)
                {
                    Assert.Equal(expectedMatrix[r, c], resultMatrix[r, c]); // Проверка каждого элемента
                }
            }
        }

        /// <summary>
        /// Тест на то, что операция поворота выбрасывает исключение для неквадратной матрицы.
        /// </summary>
        [Fact] 
        public async Task ExecuteOperationAsync_NonSquareMatrix_ThrowsInvalidOperationException() 
        {
            // Arrange
            // Создаем неквадратную матрицу 2x3
            IMatrix<double> nonSquareMatrix = new Matrix<double>(2, 3, (r, c) => r + c);
            var strategy = new Rotate90Strategy<double>();

       
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await strategy.ExecuteOperationAsync(nonSquareMatrix));
        }

        /// <summary>
        /// Тест на проверку типа операции.
        /// </summary>
        [Fact]
        public void OperationType_ReturnsCorrectEnum()
        {
            // Arrange
            var strategy = new Rotate90Strategy<double>();

            // Act
            MatrixOperation operationType = strategy.OperationType;

            // Assert
            Assert.Equal(MatrixOperation.Rotate90, operationType); // 
        }
    }
}
