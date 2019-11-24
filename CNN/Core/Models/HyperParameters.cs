namespace Core.Models
{
    /// <summary>
    /// Гиперпараметры.
    /// </summary>
    public class HyperParameters
    {
        /// <summary>
        /// Количество эпох.
        /// </summary>
        public int EpochCount { get; set; } = 1;

        /// <summary>
        /// Идеальный результат.
        /// </summary>
        public double IdealResult { get; set; } = 1;

        /// <summary>
        /// Скорость обучения.
        /// </summary>
        public double Epsilon { get; set; } = 0.1;

        /// <summary>
        /// Момент.
        /// </summary>
        public double Alpha { get; set; } = 0.1;
    }
}
