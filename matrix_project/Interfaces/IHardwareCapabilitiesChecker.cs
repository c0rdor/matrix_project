using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace matrix_project.Interfaces
{
    public interface IHardwareCapabilitiesChecker
    {
        bool IsAvx512Supported(Type t);
        bool IsAvx2Supported(Type t);
        // Можно расширить для других возможностей
    }
}
