using matrix_project.Enums;
using matrix_project.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace matrix_project.Strategies.Binary.MatrixMultiplication.MultiplicationFactory
{
    public class MultiplicationStrategySelector<T> : IMultiplicationStrategySelector<T>
    {
        private readonly IEnumerable<IMatrixMultiplicationStrategy<T>> _availableStrategies;
        private readonly ILogger<MultiplicationStrategySelector<T>> _logger;
        private readonly IHardwareCapabilitiesChecker _hardwareChecker;

        public MultiplicationStrategySelector(
            IEnumerable<IMatrixMultiplicationStrategy<T>> availableStrategies,
            ILogger<MultiplicationStrategySelector<T>> logger,
            IHardwareCapabilitiesChecker hardwareChecker)
        {
            _availableStrategies = availableStrategies ?? throw new ArgumentNullException(nameof(availableStrategies));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _hardwareChecker = hardwareChecker ?? throw new ArgumentNullException(nameof(hardwareChecker));

            LogAvailableStrategies();
        }

        private void LogAvailableStrategies()
        {
            _logger.LogInformation($"MultiplicationStrategySelector<{typeof(T).Name}> initialized. Available strategies:");
            foreach (var strategy in _availableStrategies)
            {
                _logger.LogInformation($"- {strategy.GetType().Name} (Type: {strategy.MultiplicationType})");
            }
        }

        public IMatrixMultiplicationStrategy<T> SelectStrategy(MatrixMultiplicationType multiplicationType = MatrixMultiplicationType.Auto)
        {
            var fallbackStrategy = GetFallbackStrategy();

            if (multiplicationType == MatrixMultiplicationType.Auto)
            {
                var autoStrategy = TrySelectAutoStrategy();
                if (autoStrategy != null)
                {
                    _logger.LogInformation($"Auto-selected strategy: {autoStrategy.GetType().Name}");
                    return autoStrategy;
                }

                _logger.LogInformation($"Auto-selected fallback strategy: {fallbackStrategy.GetType().Name}");
                return fallbackStrategy;
            }
            else
            {
                var requestedStrategy = _availableStrategies.FirstOrDefault(s => s.MultiplicationType == multiplicationType);

                if (requestedStrategy == null || !ValidateRequestedStrategy(requestedStrategy))
                {
                    _logger.LogWarning($"Requested strategy '{multiplicationType}' not available or not supported for type {typeof(T).Name}. Falling back to {fallbackStrategy.GetType().Name}");
                    return fallbackStrategy;
                }

                _logger.LogInformation($"Manually selected strategy: {requestedStrategy.GetType().Name}");
                return requestedStrategy;
            }
        }

        private IMatrixMultiplicationStrategy<T> GetFallbackStrategy()
        {
            return _availableStrategies.FirstOrDefault(s => s.MultiplicationType == MatrixMultiplicationType.Block)
                ?? throw new InvalidOperationException($"Fallback block strategy not registered for type {typeof(T).Name}");
        }

        private IMatrixMultiplicationStrategy<T>? TrySelectAutoStrategy()
        {
            if (_hardwareChecker.IsAvx512Supported(typeof(T)))
            {
                var avx512Strategy = _availableStrategies.FirstOrDefault(s =>
                    s.MultiplicationType == (typeof(T) == typeof(double)
                        ? MatrixMultiplicationType.Avx512Double
                        : MatrixMultiplicationType.Avx512Float));

                if (avx512Strategy != null)
                    return avx512Strategy;
            }

            if (_hardwareChecker.IsAvx2Supported(typeof(T)))
            {
                var avx2Strategy = _availableStrategies.FirstOrDefault(s => s.MultiplicationType == MatrixMultiplicationType.Avx2Double);

                if (avx2Strategy != null)
                    return avx2Strategy;
            }

            return null;
        }

        private bool ValidateRequestedStrategy(IMatrixMultiplicationStrategy<T> strategy)
        {
            var type = strategy.MultiplicationType;

            return type switch
            {
                MatrixMultiplicationType.Avx512Double => _hardwareChecker.IsAvx512Supported(typeof(T)) && typeof(T) == typeof(double),
                MatrixMultiplicationType.Avx512Float => _hardwareChecker.IsAvx512Supported(typeof(T)) && typeof(T) == typeof(float),
                MatrixMultiplicationType.Avx2Double => _hardwareChecker.IsAvx2Supported(typeof(T)) && typeof(T) == typeof(double),
                _ => true,
            };
        }
    }
}
