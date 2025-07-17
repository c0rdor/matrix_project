using matrix_project.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace matrix_project.Interfaces
{
    public interface IMatrixMultiplicationStrategy<T> : IMatrixBinaryOperatorStrategy<T> 
    {
        MatrixMultiplicationType MultiplicationType { get; }
    }
}
