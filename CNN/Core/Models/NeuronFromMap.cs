namespace Core.Models
{
    using System.Collections.Generic;

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
        public NeuronFromMap(List<double> inputs, List<double> weights, Cell cell,
            ActivationFunctionType type = ActivationFunctionType.Sigmoid) : base(inputs, weights, type)
        {
            Inputs = inputs;
            Weights = weights;
            _type = type;
            X = cell.X;
            Y = cell.Y;
        }

        /// <summary>
        /// Позиция в карте по X.
        /// </summary>
        public int X { get; private set; }

        /// <summary>
        /// Позиция в карте по Y.
        /// </summary>
        public int Y { get; private set; }
    }
}
