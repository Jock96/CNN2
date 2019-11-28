namespace Core.Models
{
    using Core.Models.Layers;
    using Core.Utils;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Мнимая матрица макс-пуллинга.
    /// </summary>
    internal class VirtualMaxPoolingMatrix
    {
        /// <summary>
        /// Ячейки.
        /// </summary>
        private List<Cell> _cells = new List<Cell>();

        /// <summary>
        /// Мнимая матрица макс-пуллинга.
        /// </summary>
        /// <param name="cells">Ячейки.</param>
        public VirtualMaxPoolingMatrix(List<Cell> cells)
        {
            cells.ForEach(cell => 
                _cells.Add(new Cell(cell.X, cell.Y, 0) { Delta = cell.Delta }));
        }

        /// <summary>
        /// Задать значение дельт нейронам предыдущего слоя от слоя макс-пуллинга.
        /// </summary>
        /// <param name="sumbsamplinLayer">Слой макс-пуллинга.</param>
        public void SetDeltaToConvolutionLayer(SubsamplingLayer sumbsamplinLayer)
        {
            var map = sumbsamplinLayer.Map;
            var size = sumbsamplinLayer.PoolingMatrix.Size;

            foreach (var cell in _cells)
            {
                for (var x = 0; x <= map.Size - size; x += size)
                    for (var y = 0; y <= map.Size - size; y += size)
                    {
                        var xEndPoint = x + size;
                        var yEndPoint = y + size;

                        var cells = map.Cells.
                            FindAll(cellInMap =>
                            cellInMap.X >= x &&
                            cellInMap.X < xEndPoint &&
                            cellInMap.Y >= y &&
                            cellInMap.Y < yEndPoint);

                        var trueValueCell = cells.First();

                        foreach (var someCell in cells)
                        {
                            if (someCell.Value > trueValueCell.Value)
                                trueValueCell = someCell;
                        }

                        trueValueCell.Delta = MathUtil.DerivativeActivationFunction(trueValueCell.Value,
                            Enums.ActivationFunctionType.Sigmoid) * cell.Delta;

                        foreach (var falseValueCell in cells)
                        {
                            if (falseValueCell.X.Equals(trueValueCell.X) &&
                                falseValueCell.Y.Equals(trueValueCell.Y))
                                continue;

                            falseValueCell.Delta = 0;
                        }
                    }
            }
        }
    }
}
