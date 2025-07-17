using matrix_project.Enums;
using matrix_project.Interfaces;
using matrix_project.Strategies.Binary.MatrixMultiplication.MatrixMultiplicationStrategy;
using Microsoft.Extensions.DependencyInjection;

public class FloatStrategyRegistrar : StrategyRegistrarBase<float>
{
    protected override Dictionary<MatrixMultiplicationType, Action<IServiceCollection>> Registrations { get; }
        = new()
    {
        { MatrixMultiplicationType.Avx512Float, services =>
            services.AddSingleton<IMatrixMultiplicationStrategy<float>, FloatAvx512MultiplicationStrategy>() },
        { MatrixMultiplicationType.Block, services =>
            services.AddSingleton<IMatrixMultiplicationStrategy<float>, NumberBlockMatrixMultiplicationStrategy<float>>() },
    };
}
