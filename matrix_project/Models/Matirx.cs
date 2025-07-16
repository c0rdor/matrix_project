using matrix_project.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace matrix_project.Models
{
    public class Matrix<T> : IMatrix<T>
    {
        private readonly T[,] _data;

        /// <summary>
        /// Gets the number of rows in the matrix.
        /// </summary>
        public int RowCount { get; }

        /// <summary>
        /// Gets the number of columns in the matrix.
        /// </summary>
        public int ColCount { get; }

        /// <summary>
        /// Initializes a new instance of the Matrix class with specified dimensions.
        /// </summary>
        /// <param name="rows">Number of rows.</param>
        /// <param name="cols">Number of columns.</param>
        public Matrix(int rows, int cols)
        {
            RowCount = rows;
            ColCount = cols;
            _data = new T[rows, cols];
        }

        /// <summary>
        /// Initializes a new instance of the Matrix class with specified dimensions and initializer function.
        /// </summary>
        /// <param name="rows">Number of rows.</param>
        /// <param name="cols">Number of columns.</param>
        /// <param name="initializer">Function to initialize matrix elements.</param>
        public Matrix(int rows, int cols, Func<int, int, T> initializer) : this(rows, cols)
        {
            for (int r = 0; r < rows; r++)
                for (int c = 0; c < cols; c++)
                    _data[r, c] = initializer(r, c);
        }

        /// <summary>
        /// Gets or sets the element at the specified row and column.
        /// </summary>
        /// <param name="row">The row index.</param>
        /// <param name="col">The column index.</param>
        public T this[int row, int col]
        {
            get => _data[row, col];
            set => _data[row, col] = value;
        }
    }

    /// <summary>
    /// Provides methods for transforming matrices in blocks.
    /// </summary>
}
