using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace matrix_project.Interfaces
{
    public interface IStrategyRegistrar
    {
        void Register(IServiceCollection services);
    }
}
