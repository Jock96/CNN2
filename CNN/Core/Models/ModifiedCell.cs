﻿namespace Core.Models
{
    /// <summary>
    /// Модифицируемая ячейка.
    /// </summary>
    internal class ModifiedCell : Cell
    {
        /// <summary>
        /// Модифицируемая ячейка..
        /// </summary>
        /// <param name="xPosition">Позиция X.</param>
        /// <param name="yPosition">Позиция Y.</param>
        /// <param name="value">Значение.</param>
        public ModifiedCell(int xPosition, int yPosition, double value) : base(xPosition, yPosition, value)
        {
            X = xPosition;
            Y = yPosition;
            Value = value;
        }

        /// <summary>
        /// Обновить значение ячейки.
        /// </summary>
        /// <param name="newValue">Новое значение.</param>
        public void UpdatedValue(double newValue) => Value = newValue;

        /// <summary>
        /// Обновить последнее значение дельты.
        /// </summary>
        /// <param name="newValue">Новое значение.</param>
        public void UpdateLastDeltValue(double newValue) => LastValueDelta = newValue;
    }
}
