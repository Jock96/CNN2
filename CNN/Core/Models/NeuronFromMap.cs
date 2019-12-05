namespace Core.Models
{
    using System.Collections.Generic;
    using System.Linq;
    using Core.Enums;

    /// <summary>
    /// Нейрон, преобразованный из карты изображения.
    /// </summary>
    internal class NeuronFromMap : Neuron
    {
        /// <summary>
        /// Нейрон, преобразованный из карты изображения.
        /// </summary>
        /// <param name="inputs">Входные данные.</param>
        /// <param name="weights">Веса.</param>
        /// <param name="cell">Ячейка карты изображений, из которой преобразован нейрон.</param>
        /// <param name="type">Тип ункции активации (сигмоид по-умолчанию).</param>
        public NeuronFromMap(List<double> inputs, List<double> weights,
            ActivationFunctionType type = ActivationFunctionType.Sigmoid) : base(inputs, weights, type)
        {
            Inputs = inputs;
            Weights = weights;
            ActivationFinctionType = type;
        }

        /// <summary>
        /// Значение весов с отношением к позиции в карте предыдущего слоя.
        /// </summary>
        public List<WeightToMapPosition> WeightsToMapPosition { get; set; }

        /// <summary>
        /// Выходное значение.
        /// </summary>
        public new double Output
        {
            get
            {
                var output = 0d;

                if (WeightsToMapPosition != null && WeightsToMapPosition.Any())
                    output = ActivationFunction(Inputs, WeightsToMapPosition.Select(weight => weight.Value).ToList());
                else
                    output = ActivationFunction(Inputs, Weights);

                return output;
            }
        }
    }
}
