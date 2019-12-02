namespace Core.Models.Layers
{
    using System;
    using System.Collections.Generic;

    using Core.Enums;
    using Core.Extensions;

    /// <summary>
    /// Выходной слой.
    /// </summary>
    internal class OutputLayer : Layer
    {
        /// <summary>
        /// Тип слоя.
        /// </summary>
        public override LayerType Type => LayerType.Output;

        /// <summary>
        /// Входные данные.
        /// </summary>
        private List<double> _inputs;

        /// <summary>
        /// Тип выходного слоя.
        /// </summary>
        private OutputLayerType _type;

        /// <summary>
        /// Количество нейроновна слое.
        /// </summary>
        private int _countOfNeurons;

        /// <summary>
        /// Слой инициализирован?
        /// </summary>
        private bool _isInitialized;

        /// <summary>
        /// Нейроны слоя.
        /// </summary>
        private List<Neuron> _layerNeurons;

        /// <summary>
        /// Выходной слой.
        /// </summary>
        /// <param name="inputs">Входные значения.</param>
        /// <param name="type">Тип выходного слоя (по-умолчанию распознавание чисел).</param>
        /// <param name="countOfNeurons">Количество нейронов.</param>
        public OutputLayer(List<double> inputs, OutputLayerType type = OutputLayerType.NumberRecognizing,
            int countOfNeurons = 10)
        {
            _inputs = inputs;
            _type = type;
            _countOfNeurons = countOfNeurons;
        }

        /// <summary>
        /// Записать значения на слоя.
        /// </summary>
        /// <param name="inputs">Входные значения.</param>
        public void SetData(List<double> inputs)
        {
            if (!_isInitialized)
                throw new Exception("Перед внесением данных в слой необходимо его проинициализировать!");

            _layerNeurons.ForEach(neuron => neuron.Inputs = inputs);
        }

        /// <summary>
        /// Вывести данные слоя (возвращает словарь индекс/нейрон).
        /// </summary>
        public override dynamic GetData(LayerReturnType returnType)
        {
            if (!_isInitialized)
                throw new Exception("Слой не инициализирован и не может вернуть значения!");

            // Независимо от типа возвращаем словарь нейронов
            // Ключ - номер нейрона, значение - сам нейрон.

            var indexToNeuronDictionary = new Dictionary<int, Neuron>();
            var index = 0;

            foreach (var neuron in _layerNeurons)
            {
                indexToNeuronDictionary.Add(index, neuron);
                ++index;
            }

            return indexToNeuronDictionary;
        }

        /// <summary>
        /// Инициализация слоя.
        /// </summary>
        /// <param name="type">Тип мода нейронной сети.</param>
        public override void Initialize(NetworkModeType type)
        {
            if (_isInitialized)
                throw new Exception("Невозможно инициализировать уже инициализированную модель!");

            switch (type)
            {
                case NetworkModeType.Learning:
                    _layerNeurons = new List<Neuron>();
                    CreateNeurons();
                    break;

                case NetworkModeType.Recognizing:
                    //TODO: Доделать.
                    throw new NotImplementedException();

                default:
                    throw new Exception("Неизвестный тип мода нейронной сети.");
            }

            _isInitialized = true;
        }

        /// <summary>
        /// Создать нейроны.
        /// </summary>
        private void CreateNeurons()
        {
            for (var index = 0; index < _countOfNeurons; ++index)
            {
                var weights = new List<double>();
                var lastWeights = new List<double>();

                foreach (var input in _inputs)
                {
                    lastWeights.Add(0);
                    var value = new Random().NextDouble(0.0001, 0.2);
                    System.Threading.Thread.Sleep(20);
                    weights.Add(value);
                }

                _layerNeurons.Add(new Neuron(_inputs, weights) { LastWeightsDeltas = lastWeights });
            }
        }
    }
}
