namespace Core.Models
{
    using Core.Enums;
    using Core.Utils;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Класс нейрона.
    /// </summary>
    internal class Neuron
    {
        /// <summary>
        /// Тип функции активации.
        /// </summary>
        public ActivationFunctionType ActivationFinctionType { get; protected set; }

        /// <summary>
        /// Класс нейрона.
        /// </summary>
        /// <param name="inputs">Входные данные.</param>
        /// <param name="weights">Веса.</param>
        /// <param name="type">Тип ункции активации (сигмоид по-умолчанию).</param>
        public Neuron(List<double> inputs, List<double> weights,
            ActivationFunctionType type = ActivationFunctionType.Sigmoid)
        {
            _inputs = inputs;
            _weights = weights;
        }

        #region Данные для обучения.

        /// <summary>
        /// Значение дельты веса.
        /// </summary>
        public double Delta { get; set; }

        /// <summary>
        /// Прошлое значение дельт весов.
        /// </summary>
        public List<double> LastWeightsDeltas { get; set; }

        #endregion

        #region Работа с данными

        /// <summary>
        /// Входные данные.
        /// </summary>
        private List<double> _inputs;

        /// <summary>
        /// Веса.
        /// </summary>
        private List<double> _weights;

        /// <summary>
        /// Входные данные.
        /// </summary>
        public List<double> Inputs
        {
            get => _inputs;
            set => _inputs = value;
        }

        /// <summary>
        /// Веса.
        /// </summary>
        public List<double> Weights
        {
            get => _weights;
            set => _weights = value;
        }

        #endregion

        /// <summary>
        /// Выходное значение.
        /// </summary>
        public double Output { get => ActivationFunction(_inputs, _weights); }

        /// <summary>
        /// Функция активации.
        /// </summary>
        /// <param name="inputs">Входные данные.</param>
        /// <param name="weights">Веса.</param>
        /// <returns>Возвращает нормализованное выходное значение.</returns>
        private double ActivationFunction(List<double> inputs, List<double> weights)
        {
            var summary = 0d;

            for (int index = 0; index < inputs.Count; ++index)
                summary += inputs[index] * weights[index];

            return MathUtil.ActivationFunction(ActivationFinctionType, summary);
        }

    }
}
