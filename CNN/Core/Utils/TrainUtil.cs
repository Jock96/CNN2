namespace Core.Utils
{
    using Core.Enums;
    using Core.Models;
    using Core.Models.Layers;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Инструмент обучения.
    /// </summary>
    public class TrainUtil
    {
        /// <summary>
        /// Обучающая выборки.
        /// </summary>
        private DataSet _dataSet;

        /// <summary>
        /// Тип обучения.
        /// </summary>
        private TrainType _trainType;

        /// <summary>
        /// Гиперпараметры.
        /// </summary>
        private HyperParameters _hyperParameters;

        /// <summary>
        /// Топология сети.
        /// </summary>
        private Topology _topology;

        /// <summary>
        /// Инструмент обучения.
        /// </summary>
        /// <param name="dataSet">Обучающая выборка.</param>
        /// <param name="trainType">Тип обучения.</param>
        /// <param name="hyperParameters">Гиперпараметры.</param>
        /// <param name="topology">Топология сети.</param>
        public TrainUtil(DataSet dataSet, TrainType trainType,
            HyperParameters hyperParameters, Topology topology)
        {
            if (!trainType.Equals(TrainType.Backpropagation))
                throw new NotImplementedException();

            if (!topology.IsClosed)
                throw new Exception("Топология сети не замкнута! Обучение невозможно!");

            _trainType = trainType;
            _dataSet = dataSet;
            _hyperParameters = hyperParameters;
            _topology = topology;
        }

        /// <summary>
        /// Начать обучение.
        /// </summary>
        /// <param name="filterMatrixSize">Размер матриц фильтра.</param>
        /// <param name="poolingMatrixSize">Размер пуллинговых матриц.</param>
        /// <param name="isNeedSave">Необходимо ли сохранять оубченную сеть.</param>
        public void Start(int filterMatrixSize, int poolingMatrixSize, bool isNeedSave = true)
        {
            var realScheme = InitializeRealScheme(filterMatrixSize, poolingMatrixSize);
            var outputs = realScheme.Last().Value.First().GetData(LayerReturnType.Neurons) as Dictionary<int, Neuron>;

            var epochCount = _hyperParameters.EpochCount;
            var iterationCount = outputs.Count();

            for (var epoch = 0; epoch < epochCount; ++epoch)
                for (var iteration = 0; iteration < iterationCount; ++iteration)
                {
                    var currentOutput = outputs[iteration].Output;

                    switch (_trainType)
                    {
                        case TrainType.Backpropagation:
                            DoBackpropagation(realScheme, iteration);
                            break;

                        case TrainType.Neuroevolution:
                            throw new NotImplementedException();

                        default:
                            throw new Exception("Неизвестный тип обучения!");
                    }

                    // TODO: Обновление значений.
                }

            // TODO:Сохранение значений.
        }


        private void DoBackpropagation(Dictionary<int, List<Layer>> realScheme, int iteration)
        {
            var outputLayer = realScheme.Last().Value.ToList().FirstOrDefault() as OutputLayer;
            var outputLayerNeurons = outputLayer.GetData(LayerReturnType.Neurons) as Dictionary<int, Neuron>;

            SetDeltasInOutputLayer(outputLayer, iteration);

            foreach (var outputNeuron in outputLayerNeurons)
            {
                // Output hidden
                var hiddenLayersDictionary = realScheme
                .Where(layer => layer.Value.First().Type.Equals(LayerType.Hidden))
                .ToDictionary(x => x.Key, y => y.Value);

                var neurons = hiddenLayersDictionary.Values.SelectMany(pair => pair
                .SelectMany(layer => layer.GetData(LayerReturnType.Neurons) as List<Neuron>)).ToList();

                SetDeltasInHiddenLayer(outputNeuron, neurons);
                UpdateWeightsInHiddenLayer(outputNeuron, neurons);

                var subsamplingLayersDictionary = realScheme
                    .Where(layer => layer.Value.First().Type.Equals(LayerType.Subsampling))
                    .ToDictionary(x => x.Key, y => y.Value);

                var convolutionLayersDictionary = realScheme
                    .Where(layer => layer.Value.First().Type.Equals(LayerType.Convolution))
                    .ToDictionary(x => x.Key, y => y.Value);

                // Hidden sub
                // Sub conv
                // Conv in
            }
        }

        private void UpdateWeightsInHiddenLayer(KeyValuePair<int, Neuron> outputNeuron, List<Neuron> neurons)
        {
            var deltaWeightsList = new List<double>();

            var indexer = 0;
            foreach (var neuron in neurons)
            {
                var gradient = neuron.Delta * neuron.Output;
                var deltaWeight = MathUtil.GetWeightsDelta(_hyperParameters, outputNeuron.Value, gradient, indexer);

                deltaWeightsList.Add(deltaWeight);
                ++indexer;
            }

            outputNeuron.Value.LastWeightsDeltas = deltaWeightsList;

            for (var index = 0; index < outputNeuron.Value.Weights.Count; ++index)
                outputNeuron.Value.Weights[index] += deltaWeightsList[index];
        }

        /// <summary>
        /// Обновить дельты скрытого слоя.
        /// </summary>
        /// <param name="outputNeuron">Нейрон выходного слоя.</param>
        /// <param name="neurons">Нейроны скрытого слоя.</param>
        private static void SetDeltasInHiddenLayer(KeyValuePair<int, Neuron> outputNeuron, List<Neuron> neurons)
        {
            foreach (var neuron in neurons)
            {
                var summary = 0d;

                foreach (var inputWeight in outputNeuron.Value.Weights)
                    summary += inputWeight * outputNeuron.Value.Delta;

                neuron.Delta = neuron.Output * summary;
            }
        }

        /// <summary>
        /// Установить значение дельт в выходном слое.
        /// </summary>
        /// <param name="outputLayer">Выходной слой.</param>
        /// <param name="iteration">Итерация.</param>
        private static void SetDeltasInOutputLayer(OutputLayer outputLayer, int iteration)
        {
            if (outputLayer is null)
                throw new Exception("Выходной слой оказался Null!");

            var outputNeurons = outputLayer.GetData(LayerReturnType.Neurons) as Dictionary<int, Neuron>;

            var resultToAccept = 1;
            var resultToDenied = 0;

            var idealNeuron = outputNeurons[iteration];
            idealNeuron.Delta = MathUtil.GetDeltaToOutput(idealNeuron, resultToAccept);

            var otherNeurons = new List<Neuron>();

            foreach (var pair in outputNeurons)
            {
                if (pair.Key.Equals(iteration))
                    continue;

                otherNeurons.Add(pair.Value);
            }

            otherNeurons.ForEach(neuron => neuron.Delta =
            MathUtil.GetDeltaToOutput(neuron, resultToDenied));
        }

        /// <summary>
        /// Инициализация реальной схемы.
        /// </summary>
        /// <param name="filterMatrixSize">Размер матриц фильтра.</param>
        /// <param name="poolingMatrixSize">Размер пуллинговых матриц.</param>
        private Dictionary<int, List<Layer>> InitializeRealScheme(int filterMatrixSize, int poolingMatrixSize)
        {
            var realScheme = new Dictionary<int, List<Layer>>();
            var virtualScheme = _topology.GetScheme();

            var firstValue = _dataSet.GetDataSetForNumber().First().Value.First();

            foreach (var pair in virtualScheme)
            {
                var virtualElements = pair.Value;
                var realElements = new List<Layer>();

                var index = 0;
                foreach (var layerType in virtualElements)
                {
                    Layer element = null;

                    switch (layerType)
                    {
                        case LayerType.Input:
                            element = new InputLayer(firstValue);
                            element.Initialize(NetworkModeType.Learning);
                            break;

                        case LayerType.Convolution:
                            var previousKey = pair.Key - 1;

                            var previousElements = realScheme[previousKey];
                            var previousType = virtualScheme[previousKey].First();

                            if (previousElements.Count != virtualElements.Count &&
                                previousType.Equals(LayerType.Input))
                            {
                                var previousElement = previousElements.FirstOrDefault() as InputLayer;

                                if (previousElement is null)
                                    throw new Exception("Предыдущий слой оказался Null!");

                                var map = previousElement.GetData(LayerReturnType.Map);
                                element = new ConvolutionLayer(map, filterMatrixSize);
                                element.Initialize(NetworkModeType.Learning);
                            }
                            else
                                throw new NotImplementedException();
                            break;

                        case LayerType.Subsampling:
                            previousKey = pair.Key - 1;

                            previousElements = realScheme[previousKey];
                            previousType = virtualScheme[previousKey][index];

                            if (previousElements.Count == virtualElements.Count &&
                                previousType.Equals(LayerType.Convolution))
                            {
                                var previousElement = previousElements[index] as ConvolutionLayer;

                                if (previousElement is null)
                                    throw new Exception("Предыдущий слой оказался Null!");

                                var map = previousElement.GetData(LayerReturnType.Map);
                                element = new SubsamplingLayer(map, poolingMatrixSize);
                                element.Initialize(NetworkModeType.Learning);
                            }
                            else
                                throw new NotImplementedException();
                            break;

                        case LayerType.Hidden:
                            previousKey = pair.Key - 1;

                            previousElements = realScheme[previousKey];
                            previousType = virtualScheme[previousKey][index];

                            if (previousElements.Count == virtualElements.Count &&
                                previousType.Equals(LayerType.Subsampling))
                            {
                                var previousElement = previousElements[index] as SubsamplingLayer;

                                if (previousElement is null)
                                    throw new Exception("Предыдущий слой оказался Null!");

                                var neurons = previousElement.GetData(LayerReturnType.Neurons);
                                element = new HiddenLayer(neurons);
                                element.Initialize(NetworkModeType.Learning);
                            }
                            else
                                throw new NotImplementedException();
                            break;

                        case LayerType.Output:
                            previousKey = pair.Key - 1;

                            previousElements = realScheme[previousKey];
                            previousType = virtualScheme[previousKey][index];

                            if (previousElements.Count > virtualElements.Count &&
                                previousType.Equals(LayerType.Hidden))
                            {
                                var neurons = new List<Neuron>();

                                foreach (var elementInLastLayer in previousElements)
                                {
                                    var previousElement = elementInLastLayer as HiddenLayer;

                                    if (previousElement is null)
                                        throw new Exception("Предыдущий слой оказался Null!");

                                    var data = previousElement.GetData(LayerReturnType.Neurons) as List<Neuron>;
                                    neurons.AddRange(data);
                                }

                                var outputs = neurons.Select(neuron => neuron.Output).ToList();

                                element = new OutputLayer(outputs);
                                element.Initialize(NetworkModeType.Learning);
                            }
                            else
                                throw new NotImplementedException();
                            break;

                        default:
                            throw new Exception("Неизвестный тип слоя!");
                    }

                    realElements.Add(element);
                    ++index;
                }

                realScheme.Add(pair.Key, realElements);
            }

            return realScheme;
        }
    }
}
