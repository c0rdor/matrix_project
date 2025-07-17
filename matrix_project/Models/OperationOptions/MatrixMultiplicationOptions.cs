using matrix_project.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace matrix_project.Models.OperationOptions
{
    public class MatrixMultiplicationOptions : MatrixBinaryOperationOptions
    {
        public MatrixMultiplicationType MultiplicationType { get; set; } = MatrixMultiplicationType.Auto;
    }
}
