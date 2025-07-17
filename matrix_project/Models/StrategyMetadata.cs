using matrix_project.Enums;
using matrix_project.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace matrix_project.Models
{
    // Новый вспомогательный класс для метаданных стратегии
    public class StrategyMetadata<T>
    {
        public IMatrixMultiplicationStrategy<T> Strategy { get; }
        public MatrixMultiplicationType Type { get; }
        public Func<bool> IsSupportedCheck { get; } // Делегат для проверки аппаратной поддержки
        public int Priority { get; } // Приоритет для Auto-выбора (чем выше, тем лучше)
        public Type[] SupportedTypes { get; } // Типы данных, которые поддерживает эта SIMD-стратегия

        public StrategyMetadata(
            IMatrixMultiplicationStrategy<T> strategy,
            MatrixMultiplicationType type,
            Func<bool> isSupportedCheck,
            int priority,
            params Type[] supportedTypes)
        {
            Strategy = strategy;
            Type = type;
            IsSupportedCheck = isSupportedCheck;
            Priority = priority;
            SupportedTypes = supportedTypes;
        }

        public bool IsGloballySupported => IsSupportedCheck();
        public bool IsTypeSupported => SupportedTypes.Contains(typeof(T));
        public bool IsFullySupported => IsGloballySupported && IsTypeSupported;
    }
}
