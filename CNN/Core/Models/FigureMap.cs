namespace Core.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core.Constants;
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
        public List<NeuronFromMap> ToNeuronList(bool isNeedActivate)
        {
            //if (isNeedActivate)

            var neurons = new List<NeuronFromMap>();
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
                var weightsToMapPosition = new List<WeightToMapPosition>();

                for (var w = 0; w < Cells.Count; ++w)
                {
                    lastWeights.Add(0);

                    var value = new Random().NextDouble(
                        RandomConstants.NEURON_WEIGHT_START_MIN_VALUE,
                        RandomConstants.NEURON_WEIGHT_START_MAX_VALUE);

                    System.Threading.Thread.Sleep(20);

                    weights.Add(value);

                    var weightToMapPosition = new WeightToMapPosition()
                    {
                        LastValueDelta = 0,
                        OwnerCell = Cells[w],
                        Value = value
                    };

                    weightsToMapPosition.Add(weightToMapPosition);
                }

                var neuron = new NeuronFromMap(inputs, weights)
                {
                    LastWeightsDeltas = lastWeights,
                    WeightsToMapPosition = weightsToMapPosition
                };

                neurons.Add(neuron);
            }

            return neurons;

            // TODO: Реализовать!
            //throw new NotImplementedException();
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
