namespace Core.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// Карта изображения.
    /// </summary>
    public class FigureMap
    {
        /// <summary>
        /// Размер.
        /// </summary>
        public int Size { get; private set; }

        /// <summary>
        /// Ячейки карты.
        /// </summary>
        public List<Cell> Cells { get; private set; } = new List<Cell>();

        /// <summary>
        /// Карта изображения.
        /// </summary>
        /// <param name="size"></param>
        /// <param name="data"></param>
        public FigureMap(int size, double[,] data)
        {
            Size = size;
            CreateCells(data);
        }

        /// <summary>
        /// Создание ячеек.
        /// </summary>
        /// <param name="data">Значения.</param>
        private void CreateCells(double[,] data)
        {
            for (var x = 0; x < Size; ++x)
                for (var y = 0; y < Size; ++y)
                    Cells.Add(new Cell(x, y, data[x, y]));
        }
    }
}
