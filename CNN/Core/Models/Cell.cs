namespace Core.Models
{
    /// <summary>
    /// Ячейка.
    /// </summary>
    internal class Cell
    {
        /// <summary>
        /// Позиция по X.
        /// </summary>
        public int X { get; protected set; }

        /// <summary>
        /// Позиция по Y.
        /// </summary>
        public int Y { get; protected set; }

        /// <summary>
        /// Дельта ячейки.
        /// </summary>
        public double Delta { get; protected set; }

        /// <summary>
        /// Последняя дельта значения.
        /// </summary>
        public double LastValueDelta { get; protected set; }

        /// <summary>
        /// Значение.
        /// </summary>
        public double Value { get; protected set; }

        /// <summary>
        /// Ячейка.
        /// </summary>
        /// <param name="xPosition">Позиция в карте по X.</param>
        /// <param name="yPosition">Позиция в карте по Y.</param>
        /// <param name="value">Значение.</param>
        public Cell(int xPosition, int yPosition, double value)
        {
            X = xPosition;
            Y = yPosition;
            Value = value;
        }
    }
}
