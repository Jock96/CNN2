﻿namespace Core.Models
{
    using Core.Enums;

    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Класс нейрона.
    /// </summary>
    public class Neuron
    {
        /// <summary>
        /// Тип функции активации.
        /// </summary>
        protected ActivationFunctionType _type;

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
        /// Прошлое значение весов.
        /// </summary>
        public List<double> LastWeights { get; set; }

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

            switch (_type)
            {
                case ActivationFunctionType.Sigmoid:
                    return Math.Pow(1 + Math.Exp(-summary), -1);

                case ActivationFunctionType.HyperTan:
                    return (Math.Exp(2 * summary) - 1) / (Math.Exp(2 * summary) + 1);

                default:
                    throw new Exception("Неизвестный тип функции активации!");
            }
        }

    }
}
