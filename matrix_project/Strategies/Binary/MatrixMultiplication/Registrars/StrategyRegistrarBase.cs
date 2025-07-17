using matrix_project.Enums;
using matrix_project.Interfaces;
using Microsoft.Extensions.DependencyInjection;

public abstract class StrategyRegistrarBase<T> : IStrategyRegistrar
{
    protected abstract Dictionary<MatrixMultiplicationType, Action<IServiceCollection>> Registrations { get; }

    public bool CanRegister(Type type, MatrixMultiplicationType multiplicationType)
    {
        return type == typeof(T) && Registrations.ContainsKey(multiplicationType);
    }

    public void Register(IServiceCollection services)
    {
        foreach (var register in Registrations.Values)
            register(services);
    }

    public void Register(IServiceCollection services, MatrixMultiplicationType multiplicationType)
    {
        if (Registrations.TryGetValue(multiplicationType, out var register))
            register(services);
    }
}
