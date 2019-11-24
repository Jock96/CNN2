namespace Core.Utils
{
    using Core.Enums;
    using Core.Models;

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Инструмент обучения.
    /// </summary>
    public class TrainUtil
    {
        /// <summary>
        /// Обучающая выборка.
        /// </summary>
        private double[,] _dataSet;

        /// <summary>
        /// Тип обучения.
        /// </summary>
        private TrainType _trainType;

        /// <summary>
        /// Гиперпараметры.
        /// </summary>
        private HyperParameters _hyperParameters;

        /// <summary>
        /// Инструмент обучения.
        /// </summary>
        /// <param name="dataSet">Обучающая выборка.</param>
        /// <param name="trainType">Тип обучения.</param>
        /// <param name="hyperParameters">Гиперпараметры.</param>
        /// <param name="topology">Топология сети.</param>
        public TrainUtil(double[,] dataSet, TrainType trainType,
            HyperParameters hyperParameters, Topology topology)
        {
            if (!trainType.Equals(TrainType.Backpropagation))
                throw new NotImplementedException();

            if (!topology.IsClosed)
                throw new Exception("Топология сетия не замкнута! Обучение невозможно!");

            _trainType = trainType;
            _dataSet = dataSet;
            _hyperParameters = hyperParameters;
        }

        /// <summary>
        /// Начать обучение.
        /// </summary>
        /// <param name="isNeedSave">Необходимость сохранять обученную сеть.</param>
        public void Start(bool isNeedSave = true)
        {

        }
    }
}
