namespace Core.Models
{
    /// <summary>
    /// Вес с отношением к позиции в карте предыдущего слоя.
    /// </summary>
    internal class WeightToMapPosition
    {
        /// <summary>
        /// Значение.
        /// </summary>
        public double Value { get; set; }

        /// <summary>
        /// Последние значение дельты веса.
        /// </summary>
        public double LastValueDelta { get; set; }

        /// <summary>
        /// Родительская ячейка.
        /// </summary>
        public Cell OwnerCell { get; set; }
    }
}
