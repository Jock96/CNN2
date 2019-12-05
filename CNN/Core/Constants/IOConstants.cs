namespace Core.Constants
{
    /// <summary>
    /// Класс констант ввода/вывода.
    /// </summary>
    internal static class IOConstants
    {
        /// <summary>
        /// Имя базового элемента файла настроек.
        /// </summary>
        public const string NETWORK_BASE_ELEMENT_NAME = "NeuralNetwork";

        /// <summary>
        /// Имя элемента размера входных значений.
        /// </summary>
        public const string INPUT_DATA_SIZE_ELEMENT_NAME = "InputDataSize";

        /// <summary>
        /// Имя элемента, обозначающий слои.
        /// </summary>
        public const string LAYERS_ELEMENT_NAME = "Layers";

        /// <summary>
        /// Имя элемента матрицы макс-пуллинга.
        /// </summary>
        public const string MAX_POOLING_MATRIX_ELEMENT_NAME = "MaxPoolingMatrix";

        /// <summary>
        /// Имя элемента матрицы фильтра.
        /// </summary>
        public const string FILTER_MATRIX_ELEMENT_NAME = "FilterMatrix";

        /// <summary>
        /// Имя элемента "нейроны".
        /// </summary>
        public const string NEURONS_ELEMENT_NAME = "Neurons";

        /// <summary>
        /// Имя элемента "нейрон".
        /// </summary>
        public const string NEURON_ELEMENT_NAME = "Neuron";

        /// <summary>
        /// Имя элемента ячейки.
        /// </summary>
        public const string CELL_ELEMENT_NAME = "Cell";

        /// <summary>
        /// Имя элемента "вес".
        /// </summary>
        public const string WEIGHT_ELEMENT_NAME = "Weight";

        /// <summary>
        /// Имя элемента свёрточного слоя.
        /// </summary>
        public const string CONVOLUTION_LAYER_ELEMENT_NAME = "ConvolutionLayer";

        /// <summary>
        /// Имя элемента слоя макс-пуллинга.
        /// </summary>
        public const string SUBSAMPLING_LAYER_ELEMENT_NAME = "SubsamplingLayer";

        /// <summary>
        /// Имя элемента скрытого слоя.
        /// </summary>
        public const string HIDDEN_LAYER_ELEMENT_NAME = "HiddenLayer";

        /// <summary>
        /// Имя элемента выходного слоя.
        /// </summary>
        public const string OUTPUT_LAYER_ELEMENT_NAME = "OutputLayer";

        /// <summary>
        /// Имя аттрибута позиции по X.
        /// </summary>
        public const string X_ATTRIBUTE_NAME = "X";

        /// <summary>
        /// Имя аттрибута позиции по Y.
        /// </summary>
        public const string Y_ATTRIBUTE_NAME = "Y";

        /// <summary>
        /// Имя аттрибута "тип".
        /// </summary>
        public const string TYPE_ATTRIBUTE_NAME = "Type";

        /// <summary>
        /// Имя аттрибута "количество".
        /// </summary>
        public const string COUNT_ATTRIBUTE_NAME = "Count";

        /// <summary>
        /// Имя аттрибута "Размер".
        /// </summary>
        public const string SIZE_ATTRIBUTE_NAME = "Size";

        /// <summary>
        /// Имя аттрибута "номер".
        /// </summary>
        public const string NUMBER_ATTRIBUTE_NAME = "Number";

        /// <summary>
        /// Тип свёрточного слоя.
        /// </summary>
        public const string CONVOLUTION_LAYER_TYPE = "Convolution";

        /// <summary>
        /// Тип слоя макс-пуллинга.
        /// </summary>
        public const string SUBSAMPLING_LAYER_TYPE = "Subsampling";

        /// <summary>
        /// Тип скрытого слоя.
        /// </summary>
        public const string HIDDEN_LAYER_TYPE = "Hidden";

        /// <summary>
        /// Тип выходного слоя.
        /// </summary>
        public const string OUTPUT_LAYER_TYPE = "Output";

        /// <summary>
        /// Тип нейронов.
        /// </summary>
        public const string NEURONS_TYPE = "Neurons";

        /// <summary>
        /// Тип карты.
        /// </summary>
        public const string MAP_TYPE = "Map";
    }
}
