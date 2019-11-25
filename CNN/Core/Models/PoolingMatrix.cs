namespace Core.Models
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Матрица макс-пулинга.
    /// </summary>
    internal class PoolingMatrix
    {
        /// <summary>
        /// Размер.
        /// </summary>
        public int Size { get; private set; }

        /// <summary>
        /// Матрица макс-пулинга.
        /// </summary>
        /// <param name="size">Размер.</param>
        public PoolingMatrix(int size)
        {
            Size = size;
        }

        /// <summary>
        /// Сделать макс-пуллинг.
        /// </summary>
        /// <param name="map">Карта изображения.</param>
        /// <returns>Возвращаёт обработанную карту.</returns>
        public FigureMap DoMaxPooling(FigureMap map)
        {
            if (map.Size <= Size)
                throw new Exception("Размер карты изображения не может " +
                    "быть меньше или равен размеру матрицы макс-пуллинга.");

            if (map.Size % Size > 0)
                throw new Exception("Размеры карты изображения и матрицы " +
                    "макс-пуллинга должны быть кратны.");

            var sectorsInLine = map.Size / Size;
            var matrix = new double[sectorsInLine, sectorsInLine];

            for (var xStartIndex = 0; xStartIndex < sectorsInLine; xStartIndex += Size)
                for (var yStartIndex = 0; xStartIndex < sectorsInLine; yStartIndex += Size)
                {
                    var xEndIndex = xStartIndex + Size;
                    var yEndIndex = yStartIndex + Size;

                    var elements = map.Cells.FindAll(
                        cell => cell.X >= xStartIndex &&
                        cell.X <= xEndIndex &&
                        cell.Y >= yStartIndex &&
                        cell.Y <= yEndIndex);

                    matrix[xStartIndex / Size, yStartIndex / Size] = GetMaxValue(elements);
                }

            return new FigureMap(sectorsInLine, matrix);
        }

        /// <summary>
        /// Получает максимальное значение из списка.
        /// </summary>
        /// <param name="cells">Ячейки.</param>
        /// <returns>Возвращает максимальное значение.</returns>
        private double GetMaxValue(List<Cell> cells)
        {
            var maxValue = 0d;

            foreach(var cell in cells)
            {
                if (cell.Value > maxValue)
                    maxValue = cell.Value;
            }

            return maxValue;
        }
    }
}
