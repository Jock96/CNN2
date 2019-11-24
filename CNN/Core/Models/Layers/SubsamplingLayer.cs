namespace Core.Models.Layers
{
    using System;

    using Core.Enums;

    /// <summary>
    /// Слой пуллинга.
    /// </summary>
    public class SubsamplingLayer : Layer
    {
        /// <summary>
        /// Тип слоя.
        /// </summary>
        public override LayerType Type => LayerType.Subsampling;

        /// <summary>
        /// Карта изображения.
        /// </summary>
        private FigureMap _map;

        /// <summary>
        /// Размер матрицы макс-пуллинга.
        /// </summary>
        private int _poolingMatrixSize;

        /// <summary>
        /// Слой инициализирован?
        /// </summary>
        private bool _isInitialized;

        /// <summary>
        /// Матрица макс-пуллинга.
        /// </summary>
        private PoolingMatrix _poolingMatrix;

        /// <summary>
        /// Слой пуллинга.
        /// </summary>
        /// <param name="map">Карта изображения.</param>
        /// <param name="poolingMatrixSize">Размер матрицы макс-пуллинга.</param>
        public SubsamplingLayer(FigureMap map, int poolingMatrixSize)
        {
            _map = map;
            _poolingMatrixSize = poolingMatrixSize;
        }

        /// <summary>
        /// Инициализация.
        /// </summary>
        /// <param name="type">Тип мода нейронной сети.</param>
        public override void Initialize(NetworkModeType type)
        {
            if (_isInitialized)
                throw new Exception("Невозможно инициализировать уже инициализированную модель!");

            if (type.Equals(NetworkModeType.Learning))
            {
                _poolingMatrix = new PoolingMatrix(_poolingMatrixSize);
            }
            else
            {
                //TODO: Реализовать.
                throw new NotImplementedException();
            }

            _isInitialized = true;
        }

        /// <summary>
        /// Получить данные слоя.
        /// </summary>
        /// <param name="returnType">Тип возвращаемых значений.</param>
        /// <returns>Возвращаемые значения.</returns>
        public override dynamic GetData(LayerReturnType returnType)
        {
            if (!_isInitialized)
                throw new Exception("Слой не инициализирован и не может вернуть значения!");

            var figureMap = _poolingMatrix.DoMaxPooling(_map);

            switch (returnType)
            {
                case LayerReturnType.Map:
                    return figureMap;

                case LayerReturnType.Neurons:
                    // TODO: Реализовать возврат нейронов.
                    throw new NotImplementedException();

                default:
                    throw new Exception("Неизвестный тип возвращаемого значения!");
            }
        }

        /// <summary>
        /// Запись данных слоя.
        /// </summary>
        /// <param name="map">Карта данных.</param>
        public void SetData(FigureMap map)
        {
            if (!_isInitialized)
                throw new Exception("Перед внесением данных в слой необходимо его проинициализировать!");

            if (!map.Cells.Count.Equals(_map.Cells.Count))
                throw new Exception("Размерность карты значений не совпадает!");

            _map = map;
        }
    }
}
