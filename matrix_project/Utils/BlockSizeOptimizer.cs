using System;

public static class BlockSizeOptimizer
{
    public class BlockSizeOptimizerOptions
    {
        public int BaseBlockSize { get; set; } = 64;
        public long MinElementsForParallelism { get; set; } = 4096;
        public long LargeMatrixThreshold1 { get; set; } = 1_000_000;
        public int LargeMatrixBlockSize1 { get; set; } = 128;
        public long LargeMatrixThreshold2 { get; set; } = 10_000_000;
        public int LargeMatrixBlockSize2 { get; set; } = 256;
        public int MaxAllowedBlockSize { get; set; } = 512;
        public bool AdjustForProcessorCount { get; set; } = false;

        /// <summary>
        /// Необязательный логгер для отладки.
        /// </summary>
        public Action<string>? Logger { get; set; }
    }

    private static BlockSizeOptimizerOptions _options = new BlockSizeOptimizerOptions();

    public static void SetOptions(BlockSizeOptimizerOptions options)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    public static int GetOptimalBlockSize(int matrixRowCount, int matrixColCount)
    {
        if (matrixRowCount <= 0 || matrixColCount <= 0)
            return 1;

        long totalElements = (long)matrixRowCount * matrixColCount;

        if (totalElements < _options.MinElementsForParallelism)
        {
            int result = Math.Max(1, Math.Min(matrixRowCount, matrixColCount));
            _options.Logger?.Invoke($"Matrix too small for parallelism. Using block size: {result}");
            return result;
        }

        int optimalSize = GetAdjustedBlockSizeByThresholds(totalElements);

        if (_options.AdjustForProcessorCount)
        {
            int processorCount = Environment.ProcessorCount;
            if (processorCount > 4)
            {
                optimalSize = Math.Max(1, optimalSize / 2);
                _options.Logger?.Invoke($"Processor count adjusted block size: {optimalSize}");
            }
        }

        // Учитываем размеры матрицы
        optimalSize = Math.Min(optimalSize, Math.Min(matrixRowCount, matrixColCount));
        // Ограничиваем максимум
        optimalSize = Math.Min(optimalSize, _options.MaxAllowedBlockSize);

        _options.Logger?.Invoke($"Final optimal block size: {optimalSize}");
        return Math.Max(1, optimalSize);
    }

    /// <summary>
    /// Определяет размер блока на основе порогов для больших матриц.
    /// </summary>
    private static int GetAdjustedBlockSizeByThresholds(long totalElements)
    {
        if (totalElements > _options.LargeMatrixThreshold2)
        {
            _options.Logger?.Invoke($"Matrix exceeds threshold 2. Using block size: {_options.LargeMatrixBlockSize2}");
            return _options.LargeMatrixBlockSize2;
        }

        if (totalElements > _options.LargeMatrixThreshold1)
        {
            _options.Logger?.Invoke($"Matrix exceeds threshold 1. Using block size: {_options.LargeMatrixBlockSize1}");
            return _options.LargeMatrixBlockSize1;
        }

        _options.Logger?.Invoke($"Using base block size: {_options.BaseBlockSize}");
        return _options.BaseBlockSize;
    }
}
