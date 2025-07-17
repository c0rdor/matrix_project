using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace matrix_project.Enums
{
    /// <summary>
    /// Перечисление, определяющее доступные операции над матрицами.
    /// </summary>
    public enum MatrixOperationType
    {
        /// <summary>
        /// Поворот матрицы на 90 градусов по часовой стрелке.
        /// </summary>
        Rotate90,

        /// <summary>
        /// Поворот матрицы на 180 градусов.
        /// </summary>
        Rotate180,

        /// <summary>
        /// Поворот матрицы на 270 градусов по часовой стрелке.
        /// </summary>
        Rotate270,

        /// <summary>
        /// Транспонирование матрицы.
        /// </summary>
        Transpose,

        Multiplication
    }
}
