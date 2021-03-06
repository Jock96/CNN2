﻿namespace Core.Models.Layers
{
    using System;
    using System.Collections.Generic;

    using Core.Enums;

    /// <summary>
    /// Свёрточный слой.
    /// </summary>
    internal class ConvolutionLayer : Layer
    {
        /// <summary>
        /// Тип слоя.
        /// </summary>
        public override LayerType Type => LayerType.Convolution;

        /// <summary>
        /// Размерность матрицы фильтра.
        /// </summary>
        private int _filterMatrixSize;

        /// <summary>
        /// Карта значений.
        /// </summary>
        public FigureMap Map { get; private set; }

        /// <summary>
        /// Матрица фильтра.
        /// </summary>
        public FilterMatrix FilterMatrix { get; private set; }

        /// <summary>
        /// Слой инициализирован?
        /// </summary>
        private bool _isInitialized;

        /// <summary>
        /// Свёрточный слой.
        /// </summary>
        /// <param name="map">Карта значений.</param>
        /// <param name="filterMatrixSize">Размерность матрицы фильтра.</param>
        public ConvolutionLayer(FigureMap map, int filterMatrixSize)
        {
            Map = map;
            _filterMatrixSize = filterMatrixSize;
        }

        /// <summary>
        /// Свёрточный слой.
        /// </summary>
        /// <param name="map">Карта значений.</param>
        /// <param name="filterMatrix">Матрица фильтра.</param>
        /// <param name="modeType">Тип мода ети (для обучения выдаёт исключение).</param>
        public ConvolutionLayer(FigureMap map, FilterMatrix filterMatrix, NetworkModeType modeType)
        {
            if (!modeType.Equals(NetworkModeType.Recognizing))
                throw new Exception("Данный конструктор не может использоваться для обучения!");

            Map = map;
            FilterMatrix = filterMatrix;
            _isInitialized = true;
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
                FilterMatrix = new FilterMatrix(_filterMatrixSize, type);
                FilterMatrix.Initialize();
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
        /// <param name="returnType">Тип возвращаемого значения.</param>
        /// <returns>Возвращает данные слоя.</returns>
        public override dynamic GetData(LayerReturnType returnType)
        {
            if (!_isInitialized)
                throw new Exception("Слой не инициализирован и не может вернуть значения!");

            switch (returnType)
            {
                case LayerReturnType.Map:
                    return FilterMatrix.DoMapFiltering(Map);

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

            if (!map.Cells.Count.Equals(Map.Cells.Count))
                throw new Exception("Размерность карты значений не совпадает!");

            Map = map;
        }
    }
}
