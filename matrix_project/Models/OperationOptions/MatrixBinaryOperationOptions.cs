using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace matrix_project.Models.OperationOptions
{
    public abstract class MatrixBinaryOperationOptions
    {
        public CancellationToken CancellationToken { get; set; } = default;
        public int BlockSize { get; set; } = 64; // Значение по умолчанию
    }
}
