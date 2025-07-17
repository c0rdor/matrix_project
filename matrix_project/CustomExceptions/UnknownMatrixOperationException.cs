using matrix_project.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace matrix_project.CustomExceptions
{
    public class UnknownMatrixOperationException : Exception
    {
        public UnknownMatrixOperationException(MatrixOperationType operationType)
            : base($"Unknown matrix operation: {operationType}")
        {
        }
    }
}
