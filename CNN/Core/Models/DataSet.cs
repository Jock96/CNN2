namespace Core.Models
{
    using Core.Enums;

    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Обучающая выборка.
    /// </summary>
    public class DataSet
    {
        /// <summary>
        /// Тип выборки.
        /// </summary>
        public DataSetType Type { get; private set; }

        /// <summary>
        /// Выборка для чисел.
        /// </summary>
        private Dictionary<int, List<double[,]>> _dataSetForNumbers { get; set; } 
            = new Dictionary<int, List<double[,]>>();

        /// <summary>
        /// Максимальное количество добавляемых элементов в выборку для чисел
        /// </summary>
        private const int MAX_ADDED_NUMBERS = 10;

        /// <summary>
        /// Число с которого начинается выборка для обучения чисел.
        /// </summary>
        private const int START_NUMBER = 1;

        /// <summary>
        /// Текущее число.
        /// </summary>
        private int _currentNumber = START_NUMBER;

        /// <summary>
        /// Заполнено ли?
        /// </summary>
        private bool _isFull;

        /// <summary>
        /// Обучающая выборка.
        /// </summary>
        /// <param name="type">Тип.</param>
        /// <param name="data">Данные.</param>
        public DataSet(DataSetType type)
        {
            if (!type.Equals(DataSetType.ForNumberRecognizing))
                throw new NotImplementedException();

            Type = type;
        }

        /// <summary>
        /// Добавить данные в выборку.
        /// </summary>
        /// <param name="data">Данные.</param>
        public void Add(dynamic data)
        {
            if (!Type.Equals(DataSetType.ForNumberRecognizing))
                throw new NotImplementedException();

            if (_isFull)
                throw new Exception("Элементы не могут быть добавлены в заполненную выборку!");

            var numericData = data as List<double[,]>;

            _dataSetForNumbers.Add(_currentNumber, numericData);
            _currentNumber++;

            if (_currentNumber > MAX_ADDED_NUMBERS)
                _isFull = true;
        }

        /// <summary>
        /// Максимальное число элементов в выборке.
        /// </summary>
        /// <returns>Возвращает максимальное число элементов в выборке.</returns>
        public int MaxCountInDataSet()
        {
            if (!Type.Equals(DataSetType.ForNumberRecognizing))
                throw new NotImplementedException();

            return MAX_ADDED_NUMBERS;
        }

        /// <summary>
        /// Получить данные выборки.
        /// </summary>
        /// <returns></returns>
        internal Dictionary<int, List<double[,]>> GetDataSetForNumber()
        {
            if (!_isFull)
                throw new Exception("Выборка не была заполнена до конца!");

            return _dataSetForNumbers;
        }
    }
}
