using matrix_project.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;

namespace matrix_project.Infrastructure.Hardware
{
    public class HardwareCapabilitiesChecker : IHardwareCapabilitiesChecker
    {
        public bool IsAvx512Supported(Type t)
        {
            if (!Avx512F.IsSupported) return false;
            return t == typeof(double) || t == typeof(float);
        }

        public bool IsAvx2Supported(Type t)
        {
            return Avx2.IsSupported && Fma.IsSupported && t == typeof(double);
        }
    }
}
