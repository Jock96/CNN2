namespace Core.Models
{
    using Core.Enums;

    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Топология сети.
    /// </summary>
    public class Topology
    {
        /// <summary>
        /// Схема топологии.
        /// </summary>
        private Dictionary<int, List<LayerType>> _scheme;

        /// <summary>
        /// Первое число.
        /// </summary>
        internal const int FIRST_NUMBER = 1;

        /// <summary>
        /// Последнее число.
        /// </summary>
        private int _lastNumber;

        /// <summary>
        /// Замкнута ли топология.
        /// </summary>
        public bool IsClosed { get; private set; }

        /// <summary>
        /// Зарезервированные числа.
        /// </summary>
        private List<int> _reservedNumbers = new List<int> { FIRST_NUMBER };

        /// <summary>
        /// Зарезервированные типы.
        /// </summary>
        private static List<LayerType> _reservedTypes { get; } = 
            new List<LayerType> { LayerType.Input, LayerType.Output };

        /// <summary>
        /// Топология сети.
        /// </summary>
        public Topology()
        {
            _scheme = new Dictionary<int, List<LayerType>>
            {
                { FIRST_NUMBER, new List<LayerType> { LayerType.Input } }
            };

            _lastNumber = FIRST_NUMBER;
        }

        /// <summary>
        /// Добавить объект в топологию.
        /// </summary>
        /// <param name="number">Номер в сети.</param>
        /// <param name="count">Количество элементов (несколько объектов располагаются параллельно).</param>
        /// <param name="type">Тип располагаемого элемента.</param>
        /// <param name="isLast">Последний ли элемент в списке?</param>
        public void Add(int number, int count, LayerType type, bool isLast = false)
        {
            if (IsClosed)
                throw new Exception("Невозможно добавить элемент в замкнутую топологию!");

            Check(number, count, type);
            var elements = new List<LayerType>();

            for (var index = 0; index < count; ++index)
                elements.Add(type);

            _scheme.Add(number, elements);

            if (isLast)
            {
                _scheme.Add(number + 1, 
                    new List<LayerType> { LayerType.Output });

                IsClosed = true;
                return;
            }

            _reservedNumbers.Add(number);
            _lastNumber = number;
        }

        /// <summary>
        /// Получить схему замкнутой топологии.
        /// </summary>
        /// <returns>Возвращает схему замкнутой топологии.</returns>
        public Dictionary<int, List<LayerType>> GetScheme()
        {
            if (!IsClosed)
                throw new Exception("Топология не замкнута и не может вернуть свою схему!");

            return _scheme;
        }

        /// <summary>
        /// Проверка возожности добавления.
        /// </summary>
        /// <param name="number">Номер в сети.</param>
        /// <param name="count">Количество элементов (несколько объектов располагаются параллельно).</param>
        /// <param name="type">Тип располагаемого элемента.</param>
        private void Check(int number, int count, LayerType type)
        {
            if (number < FIRST_NUMBER)
                throw new Exception("Элемент в сети не может находиться раньше первого элемента!");

            if (_lastNumber + 1 != number)
                throw new Exception("Элементы в сети должны идти последовательно!");

            if (_reservedNumbers.Contains(number))
                throw new Exception("Данный номер уже зарезервирован в сети!");

            if (_reservedTypes.Contains(type))
                throw new Exception("Нельзя использовать тип открывающий/замыкающий нейронную сеть!");

            if (number - 1 != FIRST_NUMBER && _scheme[number - 1].Count != count)
                throw new Exception("Количество последующих элементов должно быть равно количеству предыдущих!");
        }
    }
}
