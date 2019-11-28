namespace Core.Utils
{
    using Core.Enums;
    using Core.Models;

    using System;

    /// <summary>
    /// Инструмент работы с математическими вычислениями.
    /// </summary>
    internal static class MathUtil
    {
        /// <summary>
        /// Производная от функции активации (сигмоид).
        /// </summary>
        /// <param name="neuronOutput">Вывод нейрона.</param>
        /// <returns>Вовзращает результат производной функции активации (сигмоид) нейрона.</returns>
        internal static double DerivativeActivationFunction(double neuronOutput, ActivationFunctionType type)
        {
            switch (type)
            {
                case ActivationFunctionType.Sigmoid:
                    return ((1 - neuronOutput) * neuronOutput);

                case ActivationFunctionType.HyperTan:
                    return (1 - Math.Pow(neuronOutput, 2));

                default:
                    throw new Exception("Неизвестный тип функции активации!");
            }
        }

        /// <summary>
        /// Вычисляет значение функции активации.
        /// </summary>
        /// <param name="type">Тип функции активации.</param>
        /// <param name="summary">Сумма входов.</param>
        /// <returns>Возвращает значение функции активации.</returns>
        internal static double ActivationFunction(ActivationFunctionType type, double summary)
        {
            switch (type)
            {
                case ActivationFunctionType.Sigmoid:
                    return Math.Pow(1 + Math.Exp(-summary), -1);

                case ActivationFunctionType.HyperTan:
                    return (Math.Exp(2 * summary) - 1) / (Math.Exp(2 * summary) + 1);

                default:
                    throw new Exception("Неизвестный тип функции активации!");
            }
        }

        /// <summary>
        /// Нахождение дельты для выходного нейрона.
        /// </summary>
        /// <param name="neuron">Нейрон.</param>
        /// <param name="idealResult">Идеальный результат.</param>
        /// <returns>Возвращает значение дельты.</returns>
        internal static double GetDeltaToOutput(Neuron neuron, double idealResult) => 
            (idealResult - neuron.Output) * DerivativeActivationFunction(neuron.Output, neuron.ActivationFinctionType);

        /// <summary>
        /// Получить дельту веса.
        /// </summary>
        /// <param name="hyperParameters">Гиперпараметры.</param>
        /// <param name="outputNeuron">Выходной связанные нейрон.</param>
        /// <param name="gradient">Градиент.</param>
        /// <param name="indexOfCurrentNeuron">Индекс текущего нейрона.</param>
        /// <returns>Возвращает дельту веса.</returns>
        internal static double GetWeightsDelta(HyperParameters hyperParameters, Neuron outputNeuron,
            double gradient, int indexOfCurrentNeuron) =>
            hyperParameters.Epsilon * gradient +
            hyperParameters.Alpha * 
            outputNeuron.LastWeightsDeltas[indexOfCurrentNeuron];

        /// <summary>
        /// Получить дельту веса.
        /// </summary>
        /// <param name="hyperParameters">Гиперпараметры.</param>
        /// <param name="gradient">Градиент.</param>
        /// <param name="lastDeltaValue">Последние значение дельты веса.</param>
        /// <returns>Возвращает дельту веса.</returns>
        internal static double GetWeightsDelta(HyperParameters hyperParameters,
            double gradient, double lastDeltaValue) =>
            hyperParameters.Epsilon * gradient + hyperParameters.Alpha * lastDeltaValue;
    }
}
