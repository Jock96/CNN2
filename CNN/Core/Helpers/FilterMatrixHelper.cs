namespace Core.Helpers
{
    using Core.Models;

    /// <summary>
    /// Класс-помощник в вычислениях матрицы фильтра.
    /// </summary>
    internal static class FilterMatrixHelper
    {
        /// <summary>
        /// Обновить ячейку с помощтю карт входа/выхода.
        /// </summary>
        /// <param name="cell">Ячейка.</param>
        /// <param name="inputMap">Входная карта.</param>
        /// <param name="outputMap">Выходная карта.</param>
        /// <param name="filterMatrixSize">Размер матрицы фильтра.</param>
        /// <param name="hyperParameters">Гиперпараметры.</param>
        public static void UpdateCellByIOMaps(this ModifiedCell cell, FigureMap inputMap,
            FigureMap outputMap, int filterMatrixSize, HyperParameters hyperParameters)
        {
            var xEndPoint = cell.X + filterMatrixSize;
            var yEndPoint = cell.Y + filterMatrixSize;

            var gradient = 0d;

            var x = 0;
            for (var xStartPoint = cell.X; xStartPoint < xEndPoint; ++xStartPoint)
            {
                var y = 0;
                for (var yStartPoint = cell.Y; yStartPoint < yEndPoint; ++yStartPoint)
                {
                    var cellFromInputMap = inputMap.Cells
                        .Find(c => c.X.Equals(xStartPoint) && c.Y.Equals(yStartPoint));

                    var cellFromOutputMap = outputMap.Cells.Find(c => c.X.Equals(x) && c.Y.Equals(y));
                    gradient += cellFromInputMap.Value * cellFromOutputMap.Value;

                    ++y;
                }

                ++x;
            }

            var deltaValue = hyperParameters.Epsilon * gradient + hyperParameters.Alpha * cell.LastValueDelta;
            cell.UpdateLastDeltValue(deltaValue);

            var newValue = cell.Value + deltaValue;
            cell.UpdatedValue(newValue);
        }
    }
}
