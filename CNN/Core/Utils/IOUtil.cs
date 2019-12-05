namespace Core.Utils
{
    using Core.Models.Layers;
    using Core.Models;

    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Xml.Linq;
    using System.Linq;
    using System;
    using Core.Constants;

    /// <summary>
    /// Инструмент ввода/вывода данных.
    /// </summary>
    internal static class IOUtil
    {
        /// <summary>
        /// Директория сохранения.
        /// </summary>
        private const string SAVE_DIRECTORY = "Scheme";

        /// <summary>
        /// Имя файла по умолчанию.
        /// </summary>
        private const string DEFAULT_NAME = "NeuralNetworkData.xml";

        /// <summary>
        /// Сохранить топологию сети с данными.
        /// </summary>
        /// <param name="scheme">Схема.</param>
        /// <param name="pathToSave">Путь к файлу сохранения.</param>
        public static void Save(Dictionary<int, List<Layer>> scheme, out string pathToSave)
        {
            var currentDirectory = Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName;
            var path = Path.Combine(currentDirectory, SAVE_DIRECTORY);

            var document = new XDocument();
            var networkBaseElement = new XElement(IOConstants.NETWORK_BASE_ELEMENT_NAME);

            var convolutionLayersAttribute = new XAttribute(IOConstants.TYPE_ATTRIBUTE_NAME,
                IOConstants.CONVOLUTION_LAYER_TYPE);

            var subsamplingLayersAttribute = new XAttribute(IOConstants.TYPE_ATTRIBUTE_NAME,
                IOConstants.SUBSAMPLING_LAYER_TYPE);

            var hiddenLayersAttribute = new XAttribute(IOConstants.TYPE_ATTRIBUTE_NAME,
                IOConstants.HIDDEN_LAYER_TYPE);

            var outputLayersAttribute = new XAttribute(IOConstants.TYPE_ATTRIBUTE_NAME,
                IOConstants.OUTPUT_LAYER_TYPE);

            var dataOfNeuronAttribute = new XAttribute(IOConstants.TYPE_ATTRIBUTE_NAME,
                IOConstants.NEURONS_TYPE);

            var dataOfMapAttribute = new XAttribute(IOConstants.TYPE_ATTRIBUTE_NAME,
                IOConstants.MAP_TYPE);

            foreach (var pair in scheme)
            {
                var layer = pair.Value.First();

                if (layer is InputLayer inputLayer)
                {
                    var map = inputLayer.GetData(Enums.LayerReturnType.Map) as FigureMap;

                    var inputDataSize = new XElement(IOConstants.INPUT_DATA_SIZE_ELEMENT_NAME,
                        map.Size);

                    networkBaseElement.Add(inputDataSize);
                }

                if (layer is ConvolutionLayer)
                {
                    var layersElements = new XElement(IOConstants.LAYERS_ELEMENT_NAME);

                    var layerCountAttribute = new XAttribute(IOConstants.COUNT_ATTRIBUTE_NAME,
                        pair.Value.Count);

                    layersElements.Add(layerCountAttribute);
                    layersElements.Add(convolutionLayersAttribute);

                    var index = 0;
                    foreach (ConvolutionLayer convolutionLayer in pair.Value)
                    {
                        var convolutionLayerElement = new XElement(IOConstants.CONVOLUTION_LAYER_ELEMENT_NAME);

                        var convolutionLayerNumberAttribute = 
                            new XAttribute(IOConstants.NUMBER_ATTRIBUTE_NAME, index);

                        convolutionLayerElement.Add(convolutionLayerNumberAttribute);

                        var filterMatrixElement = new XElement(IOConstants.FILTER_MATRIX_ELEMENT_NAME);

                        var filterMatrixSizeAttribute = 
                            new XAttribute(IOConstants.SIZE_ATTRIBUTE_NAME, convolutionLayer.FilterMatrix.Size);

                        filterMatrixElement.Add(filterMatrixSizeAttribute);

                        foreach(var cell in convolutionLayer.FilterMatrix.Cells)
                        {
                            var cellElement = new XElement(IOConstants.CELL_ELEMENT_NAME, cell.Value);

                            var xAttribute = new XAttribute(IOConstants.X_ATTRIBUTE_NAME, cell.X);
                            var yAttribute = new XAttribute(IOConstants.Y_ATTRIBUTE_NAME, cell.Y);

                            cellElement.Add(xAttribute);
                            cellElement.Add(yAttribute);

                            filterMatrixElement.Add(cellElement);
                        }

                        convolutionLayerElement.Add(filterMatrixElement);
                        layersElements.Add(convolutionLayerElement);

                        ++index;
                    }

                    networkBaseElement.Add(layersElements);
                    continue;
                }

                if (layer is SubsamplingLayer)
                {
                    var layersElements = new XElement(IOConstants.LAYERS_ELEMENT_NAME);

                    var layerCountAttribute = new XAttribute(IOConstants.COUNT_ATTRIBUTE_NAME,
                        pair.Value.Count);

                    layersElements.Add(layerCountAttribute);
                    layersElements.Add(subsamplingLayersAttribute);

                    var index = 0;
                    foreach (SubsamplingLayer subsamplingLayer in pair.Value)
                    {
                        var subsamplingLayerElement = 
                            new XElement(IOConstants.SUBSAMPLING_LAYER_ELEMENT_NAME);

                        var subsamplingLayerNumberAttribute = 
                            new XAttribute(IOConstants.NUMBER_ATTRIBUTE_NAME, index);

                        subsamplingLayerElement.Add(subsamplingLayerNumberAttribute);

                        var maxPoolingMatrixElement = 
                            new XElement(IOConstants.MAX_POOLING_MATRIX_ELEMENT_NAME);

                        var maxPoolingMatrixAttribute = 
                            new XAttribute(IOConstants.SIZE_ATTRIBUTE_NAME,
                            subsamplingLayer.PoolingMatrix.Size);

                        maxPoolingMatrixElement.Add(maxPoolingMatrixAttribute);
                        subsamplingLayerElement.Add(maxPoolingMatrixElement);
                        layersElements.Add(subsamplingLayerElement);

                        ++index;
                    }

                    networkBaseElement.Add(layersElements);
                    continue;
                }

                if (layer is HiddenLayer)
                {
                    var layersElements = new XElement(IOConstants.LAYERS_ELEMENT_NAME);

                    var layerCountAttribute = new XAttribute(IOConstants.COUNT_ATTRIBUTE_NAME,
                        pair.Value.Count);

                    layersElements.Add(layerCountAttribute);
                    layersElements.Add(hiddenLayersAttribute);

                    var index = 0;
                    foreach (HiddenLayer hiddenLayer in pair.Value)
                    {
                        var hiddenLayerElement = new XElement(IOConstants.HIDDEN_LAYER_ELEMENT_NAME);

                        var hiddenLayerNumberAttribute = new XAttribute(IOConstants.NUMBER_ATTRIBUTE_NAME,
                            index);

                        hiddenLayerElement.Add(hiddenLayerNumberAttribute);
                        var neurons = hiddenLayer.GetData(Enums.LayerReturnType.Neurons) as List<NeuronFromMap>;

                        var hiddenLayerNeuronsElement = new XElement(IOConstants.NEURONS_ELEMENT_NAME);

                        var hiddenLayerNeuronsAttribute = 
                            new XAttribute(IOConstants.COUNT_ATTRIBUTE_NAME, neurons.Count);

                        hiddenLayerNeuronsElement.Add(hiddenLayerNeuronsAttribute);

                        var neuronIndex = 0;
                        foreach (var neuron in neurons)
                        {
                            var neuronElement = new XElement(IOConstants.NEURON_ELEMENT_NAME);

                            var neuronAttribute = new XAttribute(IOConstants.NUMBER_ATTRIBUTE_NAME,
                                neuronIndex);

                            neuronElement.Add(neuronAttribute);

                            var weightIndex = 0;
                            foreach (var weight in neuron.Weights)
                            {
                                var weightElement = new XElement(IOConstants.WEIGHT_ELEMENT_NAME, weight);

                                var weighAttribute = new XAttribute(IOConstants.NUMBER_ATTRIBUTE_NAME,
                                    weightIndex);

                                weightElement.Add(weighAttribute);
                                neuronElement.Add(weightElement);

                                ++weightIndex;
                            }

                            hiddenLayerNeuronsElement.Add(neuronElement);
                            ++neuronIndex;
                        }

                        hiddenLayerElement.Add(hiddenLayerNeuronsElement);
                        layersElements.Add(hiddenLayerElement);

                        ++index;
                    }

                    networkBaseElement.Add(layersElements);
                    continue;
                }

                if (layer is OutputLayer outputLayer)
                {
                    var layersElements = new XElement(IOConstants.LAYERS_ELEMENT_NAME);

                    var layerCountAttribute = new XAttribute(IOConstants.COUNT_ATTRIBUTE_NAME,
                        pair.Value.Count);

                    layersElements.Add(layerCountAttribute);
                    layersElements.Add(outputLayersAttribute);

                    var outputLayerElement = new XElement(IOConstants.OUTPUT_LAYER_ELEMENT_NAME);

                    var neurons = outputLayer.GetData(Enums.LayerReturnType.Neurons) 
                        as Dictionary<int, Neuron>;

                    var outputLayerNeuronsElement = new XElement(IOConstants.NEURONS_ELEMENT_NAME);

                    var outputLayerNeuronsAttribute = new XAttribute(IOConstants.COUNT_ATTRIBUTE_NAME,
                        neurons.Count);

                    outputLayerNeuronsElement.Add(outputLayerNeuronsAttribute);

                    foreach (var neuron in neurons)
                    {
                        var neuronElement = new XElement(IOConstants.NEURON_ELEMENT_NAME);

                        var neuronAttribute = new XAttribute(IOConstants.NUMBER_ATTRIBUTE_NAME,
                            neuron.Key);

                        neuronElement.Add(neuronAttribute);

                        var weightIndex = 0;
                        foreach (var weight in neuron.Value.Weights)
                        {
                            var weightElement = new XElement(IOConstants.WEIGHT_ELEMENT_NAME, weight);

                            var weighAttribute = new XAttribute(IOConstants.NUMBER_ATTRIBUTE_NAME,
                                weightIndex);

                            weightElement.Add(weighAttribute);
                            neuronElement.Add(weightElement);

                            ++weightIndex;
                        }

                        outputLayerNeuronsElement.Add(neuronElement);
                    }

                    outputLayerElement.Add(outputLayerNeuronsElement);
                    layersElements.Add(outputLayerElement);

                    networkBaseElement.Add(layersElements);
                    continue;
                }
            }

            document.Add(networkBaseElement);
            var fullPath = Path.Combine(path, DEFAULT_NAME);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            document.Save(fullPath);

            if (File.Exists(fullPath))
                pathToSave = fullPath;
            else
            {
                pathToSave = string.Empty;
                throw new Exception("Файл натроек не был сохранён!");
            }
        }

        /// <summary>
        /// Загрузить файл настроек и инициализировать сеть.
        /// </summary>
        /// <param name="path">Путь к файлу настроек.</param>
        /// <param name="inputData">Входные данные.</param>
        /// <returns>Возвращает сеть с данными.</returns>
        public static Dictionary<int, List<Layer>> LoadAndInitialize(string path, double[,] inputData)
        {
            var document = new XDocument(path);

        }
    }
}
