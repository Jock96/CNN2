namespace Core.Models
{
    using Core.Enums;

    using Extensions;

    using System;
    using System.Linq;
    using System.Collections.Generic;
    using Core.Constants;

    /// <summary>
    /// Матрица фильтра.
    /// </summary>
    internal class FilterMatrix
    {
        /// <summary>
        /// Размер.
        /// </summary>
        public int Size { get; private set; }

        /// <summary>
        /// Инициализирована ли матрица фильтра?
        /// </summary>
        private bool _isInitialized;

        /// <summary>
        /// Типа мода сети.
        /// </summary>
        private NetworkModeType _type;

        /// <summary>
        /// Ячейки матрицы фильтра.
        /// </summary>
        public List<ModifiedCell> Cells { get; private set; } = new List<ModifiedCell>();

        /// <summary>
        /// Матрица фильтра.
        /// </summary>
        /// <param name="size">Размер.</param>
        public FilterMatrix(int size, NetworkModeType type)
        {
            if (!type.Equals(NetworkModeType.Learning))
                throw new Exception("Данный конструктор может использоваться только для обучения.");

            Size = size;
            _type = type;
        }

        /// <summary>
        /// Матрица фильтра.
        /// </summary>
        /// <param name="size">Размер.</param>
        /// <param name="type">Тип мода сети.</param>
        /// <param name="cells">Ячейки.</param>
        public FilterMatrix(int size, NetworkModeType type, List<ModifiedCell> cells)
        {
            if (!type.Equals(NetworkModeType.Recognizing))
                throw new Exception("Данный конструктор может использоваться только для распознавания.");

            Cells = cells;
            Size = size;
            _type = type;

            _isInitialized = true;
        }

        /// <summary>
        /// Инициализация.
        /// </summary>
        public void Initialize()
        {
            if (_isInitialized)
                throw new Exception("Невозможно инициализировать уже инициализированную модель!");

            if (_type.Equals(NetworkModeType.Learning))
            {
                for (var x = 0; x < Size; ++x)
                    for (var y = 0; y < Size; ++y)
                    {
                        var value = new Random().NextDouble(
                            RandomConstants.NEURON_WEIGHT_START_MIN_VALUE,
                            RandomConstants.NEURON_WEIGHT_START_MAX_VALUE);

                        System.Threading.Thread.Sleep(20);
                        Cells.Add(new ModifiedCell(x, y, value));
                    }
            }
            else
            {
                ///TODO : Реализовать.
                throw new NotImplementedException();
            }

            _isInitialized = true;
        }

        /// <summary>
        /// Обновить матрицу фильтра.
        /// </summary>
        /// <param name="cells">Новые ячейки.</param>
        public void UpdateFilterMatrix(List<Cell> cells)
        {
            foreach(var cell in Cells)
            {
                var cellToGetValue = cells.FirstOrDefault(equalsCell => equalsCell.X.Equals(cell.X) && equalsCell.Y.Equals(cell.Y));

                if (cellToGetValue == null)
                    throw new Exception("Не удалось найти ячейку с нужным расположением!");

                cell.UpdatedValue(cellToGetValue.Value);
            }
        }

        /// <summary>
        /// Сделать фильтрацию карты изображения.
        /// </summary>
        /// <param name="map">Карта изображения.</param>
        /// <returns>Новая карта изображений.</returns>
        public FigureMap DoMapFiltering(FigureMap map)
        {
            if (!_isInitialized)
                throw new Exception("Невозможно использовать фильтр до его инициализации!");

            var step = map.Size - this.Size;
            var newSize = step + 1;
            double[,] data = new double[newSize, newSize];

            for (var xStartIndex = 0; xStartIndex <= step; ++xStartIndex)
                for (var yStartIndex = 0; yStartIndex <= step; ++yStartIndex)
                {
                    var xEndIndex = xStartIndex + this.Size;
                    var yEndIndex = yStartIndex + this.Size;

                    var choosenCells = map.Cells.FindAll(
                        cell => cell.X >= xStartIndex &&
                        cell.X < xEndIndex &&
                        cell.Y >= yStartIndex &&
                        cell.Y < yEndIndex);

                    var cellsToRewrite = new List<Cell>();
                    
                    choosenCells.ForEach(cell => 
                    cellsToRewrite.Add(new Cell(cell.X - xStartIndex, cell.Y - yStartIndex, cell.Value)));

                    data[xStartIndex, xStartIndex] = ToConvoluteData(cellsToRewrite, Cells);
                }

            return new FigureMap(newSize, data);
        }

        /// <summary>
        /// Свернуть данные.
        /// </summary>
        /// <param name="firstCells">Список первых ячеек.</param>
        /// <param name="secondCells">Список вторых ячеек.</param>
        /// <returns>Значение.</returns>
        private double ToConvoluteData(List<Cell> firstCells, List<ModifiedCell> secondCells)
        {
            var value = 0d;

            foreach (var cell in firstCells)
            {
                var equalsCell = secondCells.Find(
                    equalCell => equalCell.X.Equals(cell.X) && equalCell.Y.Equals(cell.Y));

                value += cell.Value * equalsCell.Value;
            }

            return value;
        }
    }
}
