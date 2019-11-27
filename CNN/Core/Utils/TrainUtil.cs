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

            var x = realScheme.Last().Value.First().GetData(LayerReturnType.Neurons);

            // TODO: Обратное распространение.
            // Обновление значений
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
                                var allData = new List<double>();

                                foreach (var elementInLastLayer in previousElements)
                                {
                                    var previousElement = elementInLastLayer as HiddenLayer;

                                    if (previousElement is null)
                                        throw new Exception("Предыдущий слой оказался Null!");

                                    var data = previousElement.GetData(LayerReturnType.Neurons) as List<double>;
                                    allData.AddRange(data);
                                }

                                element = new OutputLayer(allData);
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
