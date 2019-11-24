namespace Core.Models.Layers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Core.Enums;

    /// <summary>
    /// Скрытый слой.
    /// </summary>
    public class HiddenLayer : Layer
    {
        /// <summary>
        /// Тип слоя.
        /// </summary>
        public override LayerType Type => LayerType.Hidden;

        /// <summary>
        /// Список нейронов скрытого слоя.
        /// </summary>
        private List<Neuron> _neurons;

        /// <summary>
        /// Выходные значения.
        /// </summary>
        private List<double> _outputs;

        /// <summary>
        /// Скрытый слой.
        /// </summary>
        /// <param name="neurons">Нейроны.</param>
        public HiddenLayer(List<Neuron> neurons)
        {
            if (!neurons.Any())
                throw new Exception("Скрытому слою не были переданы нейроны! " +
                    "Слой не инициализирован!");

            _neurons = neurons;
            _outputs = new List<double>();

            _neurons.ForEach(neuron => _outputs.Add(neuron.Output));
        }

        /// <summary>
        /// Получить данные слоя.
        /// </summary>
        /// <param name="returnType">Тип возвращаемых значений.</param>
        /// <returns>Возвращаемые значения.</returns>
        public override dynamic GetData(LayerReturnType returnType)
        {
            switch (returnType)
            {
                case LayerReturnType.Neurons:
                    return _outputs;

                case LayerReturnType.Map:
                    //TODO: Доделать.
                    throw new NotImplementedException();

                default:
                    throw new Exception("Неизвестный тип возвращаемого значения!");
            }
        }

        /// <summary>
        /// Запись данных слоя.
        /// </summary>
        public void SetData(List<Neuron> neurons)
        {
            if (!neurons.Count.Equals(_neurons.Count))
                throw new Exception("Количество переданных нейронов не соттвествует количеству нейроно на слое!");

            _neurons = neurons;
        }

        /// <summary>
        /// Инициализация слоя.
        /// </summary>
        /// <param name="type">Тип мода нейронной сети.</param>
        public override void Initialize(NetworkModeType type) 
        {
            // Данный слой инициализируется автоматически,
            // при передаче ему нейронов с предыдущего слоя.
            if (type.Equals(NetworkModeType.Learning))
                return;
            else
            {
                //TODO: Реализовать.
                throw new NotImplementedException();
            }
        }
    }
}
