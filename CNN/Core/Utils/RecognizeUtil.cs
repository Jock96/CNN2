namespace Core.Utils
{
    using Core.Models;
    using Core.Models.Layers;

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    /// <summary>
    /// Инструмент распознавания.
    /// </summary>
    public class RecognizeUtil
    {
        /// <summary>
        /// Путь к файлу настроек.
        /// </summary>
        private string _settingsPath;

        /// <summary>
        /// Инструмент распознавания.
        /// </summary>
        /// <param name="settingsPath">Путь к файлу настроек.</param>
        public RecognizeUtil(string settingsPath)
        {
            if (!File.Exists(settingsPath))
                throw new Exception("Файл с настройками не найден по указанному пути!");

            _settingsPath = settingsPath;
        }

        /// <summary>
        /// Рапознать входные данные.
        /// </summary>
        /// <param name="inputData">Входные данные.</param>
        /// <returns>Возвращает ответ нейронной сети.</returns>
        public string ToRecognizeData(double [,] inputData)
        {
            var scheme = IOUtil.LoadAndInitialize(_settingsPath, inputData);

            var outputsDictionary = (scheme.Last().Value.First() as OutputLayer)
                .GetData(Enums.LayerReturnType.Neurons) as Dictionary<int, Neuron>;

            var outputString = "\n";
            var defaultOut = outputsDictionary.First();

            var maxValue = defaultOut.Value.Output;
            var maxKey = defaultOut.Key;

            foreach (var outputPair in outputsDictionary)
            {
                var percents = Math.Round(outputPair.Value.Output * 100, 2);
                outputString += $"Вероятность, что это цифра {outputPair.Key}: {percents}% \n";

                if (outputPair.Value.Output > maxValue)
                {
                    maxValue = outputPair.Value.Output;
                    maxKey = outputPair.Key;
                }
            }

            outputString += $"\nВероятнее всего это цифра {maxKey} " +
                $"(вероятность {Math.Round(maxValue * 100, 2)}%)";

            return outputString;
        }
    }
}
