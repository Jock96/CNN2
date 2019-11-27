namespace Core.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Extensions;

    /// <summary>
    /// Карта изображения.
    /// </summary>
    internal class FigureMap
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
        /// Преобразовать в список нейронов.
        /// </summary>
        /// <returns>Возвращает список нейронов.</returns>
        public List<Neuron> ToNeuronList()
        {
            var neurons = new List<Neuron>();
            int countOnLayer;

            if (Cells.Count.Equals(1))
                countOnLayer = 1;
            else
                countOnLayer = Cells.Count / 2;

            for (var index = 0; index < countOnLayer; ++index)
            {
                var inputs = Cells.Select(cell => cell.Value).ToList();

                var lastWeights = new List<double>();
                var weights = new List<double>();

                for (var w = 0; w < Cells.Count; ++w)
                {
                    lastWeights.Add(0);
                    var value = new Random().NextDouble(0.0001, 0.2);
                    System.Threading.Thread.Sleep(20);
                    weights.Add(value);
                }

                neurons.Add(new Neuron(inputs, weights) { LastWeights = lastWeights });
            }

            return neurons;
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
