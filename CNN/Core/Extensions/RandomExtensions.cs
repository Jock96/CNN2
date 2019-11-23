namespace Core.Extensions
{
    using System;

    /// <summary>
    /// Расширения стандартного класса Random.
    /// </summary>
    public static class RandomExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="random"></param>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        static public double NextDouble(this Random random,
            double minValue = double.MinValue, double maxValue = double.MaxValue)
            => random.NextDouble() * (maxValue - minValue) + minValue;
    }
}
