using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace matrix_project.Interfaces
{

    /// <summary>
    /// Interface defining a matrix with generic type elements.
    /// </summary>
    /// <typeparam name="T">The type of elements in the matrix.</typeparam>
    public interface IMatrix<T>
    {
        /// <summary>
        /// Gets the number of rows in the matrix.
        /// </summary>
        int RowCount { get; }

        /// <summary>
        /// Gets the number of columns in the matrix.
        /// </summary>
        int ColCount { get; }

        /// <summary>
        /// Gets or sets the element at the specified row and column.
        /// </summary>
        /// <param name="row">The row index.</param>
        /// <param name="col">The column index.</param>
        T this[int row, int col] { get; set; }
    }
}
