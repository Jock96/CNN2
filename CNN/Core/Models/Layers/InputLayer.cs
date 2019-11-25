namespace Core.Models.Layers
{
    using System;

    using Enums;

    /// <summary>
    /// Входной слой.
    /// </summary>
    internal class InputLayer : Layer
    {
        /// <summary>
        /// Тип слоя.
        /// </summary>
        public override LayerType Type => LayerType.Input;

        /// <summary>
        /// Данные.
        /// </summary>
        private double[,] _data;

        /// <summary>
        /// Слой инициализирован?
        /// </summary>
        private bool _isInitialized;

        /// <summary>
        /// Данные слоя.
        /// </summary>
        private FigureMap _layerData;

        /// <summary>
        /// Входной слой.
        /// </summary>
        /// <param name="data">Данные.</param>
        public InputLayer(double[,] data)
        {
            _data = data;
        }

        /// <summary>
        /// Инициализация.
        /// </summary>
        /// <param name="type">Тип мода нейронной сети.</param>
        public override void Initialize(NetworkModeType type)
        {
            if (_isInitialized)
                throw new Exception("Невозможно инициализировать уже инициализированную модель!");

            var size = GetDataSize();

            if (type.Equals(NetworkModeType.Learning))
                _layerData = new FigureMap(size, _data);
            else
            {
                //TODO: Реализовать.
                throw new NotImplementedException();
            }

            _isInitialized = true;
        }

        /// <summary>
        /// Получить размерность данных.
        /// </summary>
        /// <remarks>Если матрица не симметричена - выбрасывает исключение.</remarks>
        /// <returns>Возвращает размерность данных</returns>
        private int GetDataSize()
        {
            var lengthByX = _data.GetLength(0);
            var lengthByY = _data.GetLength(1);

            if (!lengthByX.Equals(lengthByY))
                throw new Exception("Размерность матрицы по X не соответствует размерность по Y." +
                    " Необходимо нормализовать размерность матрицы.");

            return lengthByX;
        }

        /// <summary>
        /// Получить размерность данных.
        /// </summary>
        /// <remarks>Если матрица не симметричена - выбрасывает исключение.</remarks>
        /// <param name="data">Матрица.</param>
        /// <returns>Возвращает размерность данных</returns>
        private int GetDataSize(double[,] data)
        {
            var lengthByX = _data.GetLength(0);
            var lengthByY = _data.GetLength(1);

            if (!lengthByX.Equals(lengthByY))
                throw new Exception("Размерность матрицы по X не соответствует размерность по Y." +
                    " Необходимо нормализовать размерность матрицы.");

            return lengthByX;
        }

        /// <summary>
        /// Запись данных слоя.
        /// </summary>
        /// <param name="data">Данные.</param>
        public void SetData(double[,] data)
        {
            if (!_isInitialized)
                throw new Exception("Перед внесением данных в слой необходимо его проинициализировать!");

            var inputDataSize = GetDataSize(data);
            var includedDataSize = GetDataSize();

            if (!inputDataSize.Equals(includedDataSize))
                throw new Exception("Переданная матрица не соответствует размерностью" +
                    " с уже содержащейся в слое матрицей.");

            _data = data;
            _layerData = new FigureMap(inputDataSize, data);
        }

        /// <summary>
        /// Получить данные слоя.
        /// </summary>
        /// <param name="returnType">Какой тип необходимо вернуть.</param>
        /// <returns>Возвращает данные слоя.</returns>
        public override dynamic GetData(LayerReturnType returnType)
        {
            if (!_isInitialized)
                throw new Exception("Слой не инициализирован и не может вернуть значения!");

            switch (returnType)
            {
                case LayerReturnType.Map:
                    return _layerData;

                case LayerReturnType.Neurons:
                    // TODO: Реализовать возврат нейронов.
                    throw new NotImplementedException();

                default:
                    throw new Exception("Неизвестный тип возвращаемого значения!");
            }
        }
    }
}
