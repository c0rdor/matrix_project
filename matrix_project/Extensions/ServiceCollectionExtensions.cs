using matrix_project.Context;
using matrix_project.Interfaces;
using matrix_project.Strategies;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace matrix_project.Extensions
{

    /// <summary>
    /// Extension methods for registering matrix operation services.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds matrix operation services to the service collection.
        /// </summary>
        /// <typeparam name="T">The type of elements in the matrix.</typeparam>
        /// <param name="services">The service collection to add to.</param>
        /// <returns>The updated service collection.</returns>
        public static IServiceCollection AddMatrixOperations<T>(this IServiceCollection services)
        {
            services.AddSingleton<IMatrixOperationStrategy<T>, Rotate90Strategy<T>>();
            services.AddSingleton<IMatrixOperationStrategy<T>, Rotate180Strategy<T>>();
            services.AddSingleton<IMatrixOperationStrategy<T>, Rotate270Strategy<T>>();
            services.AddSingleton<IMatrixOperationStrategy<T>, TransposeStrategy<T>>();
            services.AddSingleton<IMatrixOperationContext<T>, MatrixOperationContext<T>>();
            return services;
        }
    }
}
