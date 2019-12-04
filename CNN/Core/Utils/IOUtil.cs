namespace Core.Utils
{
    using Core.Models.Layers;

    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Xml.Linq;
    using System.Linq;
    using Core.Models;

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
        /// Сохранить топологию сети с данными.
        /// </summary>
        /// <param name="scheme">Схема.</param>
        public static void Save(Dictionary<int, List<Layer>> scheme)
        {
            var currentDirectory = Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName;
            var path = Path.Combine(currentDirectory, SAVE_DIRECTORY);

            var document = new XDocument();
            var networkBaseElement = new XElement("NeuralNetwork");

            var convolutionLayersAttribute = new XAttribute("Type", "Convolution");
            var subsamplingLayersAttribute = new XAttribute("Type", "Subsampling");
            var hiddenLayersAttribute = new XAttribute("Type", "Hidden");
            var outputLayersAttribute = new XAttribute("Type", "Output");

            var dataOfNeuronAttribute = new XAttribute("Type", "Neurons");
            var dataOfMapAttribute = new XAttribute("Type", "Map");

            foreach (var pair in scheme)
            {
                var layer = pair.Value.First();

                if (layer is InputLayer inputLayer)
                {
                    var map = inputLayer.GetData(Enums.LayerReturnType.Map) as FigureMap;
                    var inputDataSize = new XElement("InputDataSize", map.Size);

                    networkBaseElement.Add(inputDataSize);
                }

                if (layer is ConvolutionLayer)
                {
                    var layersElements = new XElement("Layers");
                    var layerCountAttribute = new XAttribute("Count", pair.Value.Count);

                    layersElements.Add(layerCountAttribute);
                    layersElements.Add(convolutionLayersAttribute);

                    var index = 0;
                    foreach (ConvolutionLayer convolutionLayer in pair.Value)
                    {
                        var convolutionLayerElement = new XElement("ConvolutionLayer");
                        var convolutionLayerNumberAttribute = new XAttribute("Number", index);

                        convolutionLayerElement.Add(convolutionLayerNumberAttribute);

                        var filterMatrixElement = new XElement("FilterMatrix");
                        var filterMatrixSizeAttribute = new XAttribute("Size", convolutionLayer.FilterMatrix.Size);

                        filterMatrixElement.Add(filterMatrixSizeAttribute);

                        foreach(var cell in convolutionLayer.FilterMatrix.Cells)
                        {
                            var cellElement = new XElement("Cell", cell.Value);

                            var xAttribute = new XAttribute("X", cell.X);
                            var yAttribute = new XAttribute("Y", cell.Y);

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
                    var layersElements = new XElement("Layers");
                    var layerCountAttribute = new XAttribute("Count", pair.Value.Count);

                    layersElements.Add(layerCountAttribute);
                    layersElements.Add(subsamplingLayersAttribute);

                    var index = 0;
                    foreach (SubsamplingLayer subsamplingLayer in pair.Value)
                    {
                        var subsamplingLayerElement = new XElement("SubsamplingLayer");
                        var subsamplingLayerNumberAttribute = new XAttribute("Number", index);

                        subsamplingLayerElement.Add(subsamplingLayerNumberAttribute);

                        var maxPoolingMatrixElement = new XElement("MaxPoolingMatrix");
                        var maxPoolingMatrixAttribute = new XAttribute("Size", subsamplingLayer.PoolingMatrix.Size);

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
                    var layersElements = new XElement("Layers");
                    var layerCountAttribute = new XAttribute("Count", pair.Value.Count);

                    layersElements.Add(layerCountAttribute);
                    layersElements.Add(hiddenLayersAttribute);

                    var index = 0;
                    foreach (HiddenLayer hiddenLayer in pair.Value)
                    {
                        var hiddenLayerElement = new XElement("HiddenLayer");
                        var hiddenLayerNumberAttribute = new XAttribute("Number", index);

                        hiddenLayerElement.Add(hiddenLayerNumberAttribute);
                        var neurons = hiddenLayer.GetData(Enums.LayerReturnType.Neurons) as List<NeuronFromMap>;

                        var hiddenLayerNeuronsElement = new XElement("Neurons");
                        var hiddenLayerNeuronsAttribute = new XAttribute("Count", neurons.Count);

                        hiddenLayerNeuronsElement.Add(hiddenLayerNeuronsAttribute);

                        var neuronIndex = 0;
                        foreach (var neuron in neurons)
                        {
                            var neuronElement = new XElement("Neuron");
                            var neuronAttribute = new XAttribute("Number", neuronIndex);

                            neuronElement.Add(neuronAttribute);

                            var weightIndex = 0;
                            foreach (var weight in neuron.Weights)
                            {
                                var weightElement = new XElement("Weight", weight);
                                var weighAttribute = new XAttribute("Number", weightIndex);

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
                    var layersElements = new XElement("Layers");
                    var layerCountAttribute = new XAttribute("Count", pair.Value.Count);

                    layersElements.Add(layerCountAttribute);
                    layersElements.Add(outputLayersAttribute);

                    var outputLayerElement = new XElement("OutputLayer");
                    var neurons = outputLayer.GetData(Enums.LayerReturnType.Neurons) as Dictionary<int, Neuron>;

                    var outputLayerNeuronsElement = new XElement("Neurons");
                    var outputLayerNeuronsAttribute = new XAttribute("Count", neurons.Count);

                    outputLayerNeuronsElement.Add(outputLayerNeuronsAttribute);

                    foreach (var neuron in neurons)
                    {
                        var neuronElement = new XElement("Neuron");
                        var neuronAttribute = new XAttribute("Number", neuron.Key);

                        neuronElement.Add(neuronAttribute);

                        var weightIndex = 0;
                        foreach (var weight in neuron.Value.Weights)
                        {
                            var weightElement = new XElement("Weight", weight);
                            var weighAttribute = new XAttribute("Number", weightIndex);

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
            document.Save("NeuralNetworkData.xml");
        }

        public void Load(string path)
        {

        }
    }
}
