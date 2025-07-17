using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace matrix_project.Enums
{
    public enum MatrixMultiplicationType
    {
        Auto,
        /// <summary>Блочное умножение для любых числовых типов (GenericBlockMultiplicationStrategy).</summary>
        Block,

        /// <summary>Умножение с использованием AVX-512 для double (DoubleAvx512MultiplicationStrategy).</summary>
        Avx512Double,

        /// <summary>Умножение с использованием AVX-512 для float (FloatAvx512MultiplicationStrategy).</summary>
        Avx512Float,

        Avx2Double,
  
    }
}
