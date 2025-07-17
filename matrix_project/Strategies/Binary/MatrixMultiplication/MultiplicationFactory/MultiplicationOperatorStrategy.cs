using matrix_project.Enums;
using matrix_project.Interfaces;

public class MultiplicationOperatorStrategy<T> : IMatrixBinaryOperatorStrategy<T>
{
    private readonly IMultiplicationStrategySelector<T> _selector;

    public MultiplicationOperatorStrategy(IMultiplicationStrategySelector<T> selector)
    {
        _selector = selector;
    }

    public MatrixOperationType OperationType => MatrixOperationType.Multiplication;

    public async Task<IMatrix<T>> ExecuteOperationAsync(
        IMatrix<T> matrixA,
        IMatrix<T> matrixB,
        CancellationToken cancellationToken = default,
        int blockSize = 64)
    {
        var strategy = _selector.SelectStrategy(MatrixMultiplicationType.Auto);
        return await strategy.ExecuteOperationAsync(matrixA, matrixB, cancellationToken, blockSize);
    }
}
