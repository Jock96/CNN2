namespace UI
{
    using BL.Helper;
    using Core.Models;
    using Core.Utils;

    using System;
    using System.IO;
    using System.Linq;
    using System.Collections.Generic;
    using Core.Enums;

    /// <summary>
    /// Класс интерфейса программы.
    /// </summary>
    class Program
    {
        /// <summary>
        /// Точка входа.
        /// </summary>
        /// <param name="args">Аргументы.</param>
        static void Main(string[] args)
        {
            try
            {
                var resources = PathHelper.GetResourcesPath();

                if (!Directory.Exists(resources))
                    throw new Exception("Директория ресурсов не существует!");

                var imagePathes = Directory.GetFiles(resources).ToList();
                var images = PathToImageConverter.LoadImages(imagePathes);

                var resizedImages = NormilizeUtil.ResizeImages(images, 6, 6);
                var normilizedMatrixies = NormilizeUtil.GetNormilizedMatrixesFromImages(resizedImages);

                var hyperParameters = new HyperParameters();
                var topology = new Topology();

                topology.Add(2, 2, LayerType.Convolution);
                topology.Add(3, 2, LayerType.Subsampling);
                topology.Add(4, 2, LayerType.Hidden, true);

                if (!topology.IsClosed)
                    throw new Exception("Не удалось замкнуть топологию.");

                var dataSet = new DataSet(DataSetType.ForNumberRecognizing);

                // Для отладки.
                /**/

                var maxCount = dataSet.MaxCountInDataSet();

                for (var i = 0; i < maxCount; ++i)
                    dataSet.Add(normilizedMatrixies);

                /**/

                var trainUtil = new TrainUtil(dataSet, TrainType.Backpropagation, hyperParameters, topology);
                trainUtil.Start(3, 2, out var error, out var pathToSave);

                var errorString = $"{Math.Round(error * 100, 2)}%";

                Console.WriteLine($"Ошибка: {errorString}");
                Console.WriteLine($"Путь к настройкам: {pathToSave}");

                var recognizeUtil = new RecognizeUtil(pathToSave);

                // Для отладки
                /**/

                var data = normilizedMatrixies.First();

                /**/

                var answer = recognizeUtil.ToRecognizeData(data);
                Console.WriteLine(answer);
            }
            catch (Exception exception)
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.ForegroundColor = ConsoleColor.Black;

                Console.WriteLine(exception.Message);

                Console.ForegroundColor = ConsoleColor.White;
                Console.BackgroundColor = ConsoleColor.Black;
            }

            Console.ReadKey();
        }
    }
}
