namespace Core.Utils
{
    using Core.Enums;
    using Core.Helpers;
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
        /// Виртуальная матрица макс-пуллинга.
        /// </summary>
        private VirtualMaxPoolingMatrix _virtualMaxPoolingMatrix;

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
        /// <param name="error">Ошибка сети.</param>
        /// <param name="pathToSave">Путь до сохраненого файла.</param>
        /// <param name="isNeedSave">Необходимо ли сохранять оубченную сеть.</param>
        public void Start(int filterMatrixSize, int poolingMatrixSize, out double error, out string pathToSave, bool isNeedSave = true)
        {
            pathToSave = string.Empty;
            error = 0d;

            var errorSummary = 0d;

            var realScheme = InitializeRealScheme(filterMatrixSize, poolingMatrixSize);
            var outputs = realScheme.Last().Value.First().GetData(LayerReturnType.Neurons) as Dictionary<int, Neuron>;

            var epochCount = _hyperParameters.EpochCount;
            var iterationCount = outputs.Count();

            #warning Реализация сделана для дефолтной схемы: вход -> N свёртки -> N пуллинг -> N скрытых -> выход

            for (var epoch = 0; epoch < epochCount; ++epoch)
            {
                var summaryOnEpoch = 0d;

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

                    summaryOnEpoch += Math.Pow(_hyperParameters.IdealResult - currentOutput, 2);

                    RewriteInputs(realScheme, iteration, iterationCount, epoch);
                }

                errorSummary += summaryOnEpoch / iterationCount;
            }

            error = errorSummary / epochCount;

            if (isNeedSave)
                IOUtil.Save(realScheme, out pathToSave);
        }

        /// <summary>
        /// Записать новые входные значения.
        /// </summary>
        /// <param name="realScheme">Топология сети.</param>
        /// <param name="iteration">Текущая итерация.</param>
        /// <param name="iterationCount">Количество итераций.</param>
        /// <param name="epoch">Текущая эпоха.</param>
        private void RewriteInputs(Dictionary<int, List<Layer>> realScheme, int iteration, int iterationCount, int epoch)
        {
            var nextIteration = iteration + 1;

            if (nextIteration == iterationCount)
                nextIteration = 0;

            var numberOfData = epoch;
            var countOfSets = _dataSet.GetDataSetForNumber()[nextIteration].Count;

            if (epoch > countOfSets)
                numberOfData = epoch - countOfSets;

            var data = _dataSet.GetDataSetForNumber()[nextIteration][numberOfData];
            var inputLayer = realScheme.First().Value.ToList().FirstOrDefault() as InputLayer;

            inputLayer.SetData(data);
            var inputMap = inputLayer.GetData(LayerReturnType.Map) as FigureMap;

            var nextKey = realScheme.First().Key + 1;
            var countOfLayers = realScheme[nextKey].Count;

            var lastKey = realScheme.Last().Key;
            var inputs = new List<double>();

            for (var indexOfLayer = 0; indexOfLayer < countOfLayers; ++indexOfLayer)
            {
                var convolutionLayersDictionary = realScheme
                    .Where(layer => layer.Value.First().Type.Equals(LayerType.Convolution))
                    .ToDictionary(x => x.Key, y => y.Value);

                var convolutionLayer = convolutionLayersDictionary.Values.
                    SelectMany(value => value.Select(layer => layer))
                    .ToList()[indexOfLayer] as ConvolutionLayer;

                convolutionLayer.SetData(inputMap);
                var nextData = convolutionLayer.GetData(LayerReturnType.Map) as FigureMap;

                var subsamplingLayersDictionary = realScheme
                    .Where(layer => layer.Value.First().Type.Equals(LayerType.Subsampling))
                    .ToDictionary(x => x.Key, y => y.Value);

                var subsamplingLayer = subsamplingLayersDictionary.Values.
                    SelectMany(value => value.Select(layer => layer))
                    .ToList()[indexOfLayer] as SubsamplingLayer;

                subsamplingLayer.SetData(nextData);
                var temporaryNeurons = subsamplingLayer.GetData(LayerReturnType.Neurons) as List<NeuronFromMap>;

                var hiddenLayersDictionary = realScheme
                        .Where(layer => layer.Value.First().Type.Equals(LayerType.Hidden))
                        .ToDictionary(x => x.Key, y => y.Value);

                var hiddenLayer = hiddenLayersDictionary.Values.
                    SelectMany(value => value.Select(layer => layer))
                    .ToList()[indexOfLayer] as HiddenLayer;

                var hiddenLayerNeurons = hiddenLayer.GetData(LayerReturnType.Neurons) as List<NeuronFromMap>;

                for (var indexOfNeuron = 0; indexOfNeuron < hiddenLayerNeurons.Count; ++indexOfNeuron)
                {
                    var temporaryNeuron = temporaryNeurons[indexOfNeuron];
                    var realNeuron = hiddenLayerNeurons[indexOfNeuron];

                    for (var indexOfInput = 0; indexOfInput < realNeuron.Inputs.Count; ++indexOfInput)
                        realNeuron.Inputs[indexOfInput] = temporaryNeuron.Inputs[indexOfInput];
                }

                hiddenLayerNeurons.ForEach(neuron => inputs.Add(neuron.Output));
            }

            var outputLayer = realScheme.Last().Value.ToList().FirstOrDefault() as OutputLayer;
            outputLayer.SetData(inputs);
        }

        /// <summary>
        /// Применить метод обратного распространения для обучения.
        /// </summary>
        /// <param name="realScheme">Реальна топология сети.</param>
        /// <param name="iteration">Итерация.</param>
        private void DoBackpropagation(Dictionary<int, List<Layer>> realScheme, int iteration)
        {
            var outputLayer = realScheme.Last().Value.ToList().FirstOrDefault() as OutputLayer;
            var outputLayerNeurons = outputLayer.GetData(LayerReturnType.Neurons) as Dictionary<int, Neuron>;

            SetDeltasInOutputLayer(outputLayer, iteration);

            foreach (var outputNeuron in outputLayerNeurons)
            {
                // Output -> hidden
                var hiddenLayersDictionary = realScheme
                .Where(layer => layer.Value.First().Type.Equals(LayerType.Hidden))
                .ToDictionary(x => x.Key, y => y.Value);

                var neurons = hiddenLayersDictionary.Values.SelectMany(pair => pair
                .SelectMany(layer => layer.GetData(LayerReturnType.Neurons) as List<NeuronFromMap>)).ToList();

                SetDeltasInHiddenLayer(outputNeuron, neurons);
                UpdateWeightsHiddenToOutut(outputNeuron, neurons);

                // Hidden -> sub
                var subsamplingLayersDictionary = realScheme
                    .Where(layer => layer.Value.First().Type.Equals(LayerType.Subsampling))
                    .ToDictionary(x => x.Key, y => y.Value);

                var hiddenLayers = hiddenLayersDictionary.Values.
                    SelectMany(value => value.Select(layer => layer)).ToList();

                var subsamplingLayers = subsamplingLayersDictionary.Values.
                    SelectMany(value => value.Select(layer => layer)).ToList();

                var index = 0;
                foreach (var layer in subsamplingLayers)
                {
                    var subsamplingLayer = layer as SubsamplingLayer;
                    var hiddenLayer = hiddenLayers[index];

                    var hiddenLayerNeurons = hiddenLayer.GetData(LayerReturnType.Neurons) as List<NeuronFromMap>;
                    var map = layer.GetData(LayerReturnType.Map) as FigureMap;

                    SetDeltasInSubSamplingLayer(hiddenLayerNeurons, map);
                    UpdateWeightsSubsamplingToHidden(hiddenLayerNeurons, map);

                    _virtualMaxPoolingMatrix.SetDeltaToConvolutionLayer(subsamplingLayer);
                    ++index;
                }

                //sub -> conv
                var convolutionLayersDictionary = realScheme
                    .Where(layer => layer.Value.First().Type.Equals(LayerType.Convolution))
                    .ToDictionary(x => x.Key, y => y.Value);

                var convolutionLayers = convolutionLayersDictionary.Values.
                    SelectMany(value => value.Select(layer => layer)).ToList();

                index = 0;
                foreach (var layer in convolutionLayers)
                {
                    var convolutionLayer = layer as ConvolutionLayer;
                    var subsamplingLayer = subsamplingLayers[index] as SubsamplingLayer;

                    var mapOfDeltas = subsamplingLayer.Map;
                    var mapOfInputs = convolutionLayer.Map;

                    var filterMatrix = convolutionLayer.FilterMatrix;

                    filterMatrix.Cells.ForEach(cell => 
                        cell.UpdateCellByIOMaps(mapOfInputs, mapOfDeltas, filterMatrix.Size, _hyperParameters));

                    ++index;
                }
            }
        }

        /// <summary>
        /// Обновление весов между слоем макс-пуллинга и скрытым.
        /// </summary>
        /// <param name="hiddenLayerNeurons">Нейроны скрытого слоя.</param>
        /// <param name="map">Карта слоя макс-пуллинга.</param>
        private void UpdateWeightsSubsamplingToHidden(List<NeuronFromMap> hiddenLayerNeurons, FigureMap map)
        {
            foreach (var cell in map.Cells)
            {
                foreach (var neuron in hiddenLayerNeurons)
                {
                    var gradient = neuron.Delta * cell.Value;

                    var weight = neuron.WeightsToMapPosition.
                        Find(weightInfo => weightInfo.OwnerCell.X.Equals(cell.X) &&
                        weightInfo.OwnerCell.Y.Equals(cell.Y));

                    var weightDelta = MathUtil.GetWeightsDelta(_hyperParameters, gradient, weight.LastValueDelta);
                    weight.LastValueDelta = weightDelta;
                    weight.Value += weightDelta;
                }
            }
        }

        /// <summary>
        /// Задать дельты слоя макс-пуллинга.
        /// </summary>
        /// <param name="hiddenLayerNeurons">Нейроны скрытого слоя.</param>
        /// <param name="map">Карта слоя макс-пуллинга.</param>
        private void SetDeltasInSubSamplingLayer(List<NeuronFromMap> hiddenLayerNeurons, FigureMap map)
        {
            foreach (var cell in map.Cells)
            {
                var summary = 0d;

                foreach (var neuron in hiddenLayerNeurons)
                {
                    var weightInfo = neuron.WeightsToMapPosition.
                        Find(weight => weight.OwnerCell.X.Equals(cell.X) &&
                        weight.OwnerCell.Y.Equals(cell.Y));

                    summary += weightInfo.Value * neuron.Delta;
                }

                cell.Delta = MathUtil.DerivativeActivationFunction(cell.Value,
                    ActivationFunctionType.Sigmoid) * summary;
            }

            _virtualMaxPoolingMatrix = new VirtualMaxPoolingMatrix(map.Cells);
        }

        /// <summary>
        /// Обновление весов между скрытым и выходным слоем.
        /// </summary>
        /// <param name="outputNeuron">Выходной нейрон.</param>
        /// <param name="neurons">Нейроны скрытого слоя.</param>
        private void UpdateWeightsHiddenToOutut(KeyValuePair<int, Neuron> outputNeuron, List<NeuronFromMap> neurons)
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
        private static void SetDeltasInHiddenLayer(KeyValuePair<int, Neuron> outputNeuron, List<NeuronFromMap> neurons)
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

                                var neurons = previousElement.GetData(LayerReturnType.Neurons) as List<NeuronFromMap>;
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

                                    var data = previousElement.GetData(LayerReturnType.Neurons) as List<NeuronFromMap>;
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
