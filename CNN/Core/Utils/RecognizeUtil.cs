namespace Core.Utils
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

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

        public string ToRecognizeData(double [,] inputData)
        {
            var scheme = IOUtil.LoadAndInitialize(_settingsPath, inputData);

            throw new NotImplementedException();
        }
    }
}
