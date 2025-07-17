using matrix_project.Interfaces;
using matrix_project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace matrix_project.Utils
{
    public static class MatrixFactory
    {
        /// <summary>
        /// Создаёт пустую матрицу заданного размера.
        /// </summary>
        public static IMatrix<T> CreateEmpty<T>(int rows, int cols)
        {
            return new Matrix<T>(rows, cols);
        }

        /// <summary>
        /// Создаёт матрицу с заданным значением по умолчанию.
        /// </summary>
        public static IMatrix<T> CreateFilled<T>(int rows, int cols, T defaultValue)
        {
            var matrix = new Matrix<T>(rows, cols);

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    matrix[i, j] = defaultValue;

            return matrix;
        }

        /// <summary>
        /// Создаёт матрицу из двумерного массива.
        /// </summary>
        public static IMatrix<T> FromArray<T>(T[,] data)
        {
            int rows = data.GetLength(0);
            int cols = data.GetLength(1);
            var matrix = new Matrix<T>(rows, cols);

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    matrix[i, j] = data[i, j];

            return matrix;
        }
    }

}
