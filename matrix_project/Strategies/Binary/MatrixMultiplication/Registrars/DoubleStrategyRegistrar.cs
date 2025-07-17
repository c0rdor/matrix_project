using matrix_project.Enums;
using matrix_project.Interfaces;
using matrix_project.Strategies.Binary.MatrixMultiplication.MatrixMultiplicationStrategy;
using Microsoft.Extensions.DependencyInjection;

public class DoubleStrategyRegistrar : StrategyRegistrarBase<double>
{
    protected override Dictionary<MatrixMultiplicationType, Action<IServiceCollection>> Registrations { get; }
        = new()
    {
        { MatrixMultiplicationType.Avx512Double, services =>
            services.AddSingleton<IMatrixMultiplicationStrategy<double>, DoubleAvx512MultiplicationStrategy>() },
        { MatrixMultiplicationType.Block, services =>
            services.AddSingleton<IMatrixMultiplicationStrategy<double>, NumberBlockMatrixMultiplicationStrategy<double>>() },
         { MatrixMultiplicationType.Avx2Double, services =>
            services.AddSingleton<IMatrixMultiplicationStrategy<double>, DoubleAvx2MultiplicationStrategy>() },


    };
}
