using matrix_project.Interfaces;
using matrix_project.Strategies.Unary;
using Microsoft.Extensions.DependencyInjection;

namespace matrix_project.Extensions
{
    public static class UnaryOperationExtensions
    {
        public static IServiceCollection AddMatrixUnaryOperations<T>(this IServiceCollection services)
        {
            services.AddSingleton<IMatrixUnaryOperatorStrategy<T>, Rotate90Strategy<T>>();
            services.AddSingleton<IMatrixUnaryOperatorStrategy<T>, Rotate180Strategy<T>>();
            services.AddSingleton<IMatrixUnaryOperatorStrategy<T>, Rotate270Strategy<T>>();
            services.AddSingleton<IMatrixUnaryOperatorStrategy<T>, TransposeStrategy<T>>();
            return services;
        }
    }
}
