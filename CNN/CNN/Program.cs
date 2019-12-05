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
    using CNN.Extensions;

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
            ConsoleExtensions.WriteWithColors(ConsoleColor.White, ConsoleColor.Black, "CNN v2 by Jock.");

            var input = string.Empty;
            var pathToSettings = string.Empty;

            while (true)
            {
                while (true)
                {
                    Console.WriteLine("\nДля обучения сети введите: 0.\n" +
                    "Для распознавания изображения введите: 1.\n" +
                    "Для выхода введите: exit.");

                    input = Console.ReadLine();

                    if (input.Equals("0") || input.Equals("1"))
                        break;

                    if (input.Equals("exit"))
                        Environment.Exit(0);
                }

                try
                {
                    if (input.Equals("0"))
                        DoTrain(out pathToSettings);
                    else
                        DoRecognize(pathToSettings);
                }
                catch (Exception exception)
                {
                    ConsoleExtensions.WriteWithColors(ConsoleColor.Red,
                        ConsoleColor.Black, $"\n{exception.Message}");
                }
            }

                var pathToSave = "";

                var recognizeUtil = new RecognizeUtil(pathToSave);

                // Для отладки
                /**/

                dynamic data = null;

                /**/

                var answer = recognizeUtil.ToRecognizeData(data);
                Console.WriteLine(answer);
        }

        private static void DoRecognize(string pathToSettings)
        {
            Console.Clear();
            var path = string.Empty;

            ConsoleExtensions.WriteWithColors(ConsoleColor.Black, ConsoleColor.Green,
                "Вас приветствует распознавание!");

            if (pathToSettings.Equals(string.Empty))
            {
                ConsoleExtensions.WriteWithColors(ConsoleColor.Black, ConsoleColor.Green,
                    "\nВведите путь до файла настроек:");

                path = Console.ReadLine();
            }
            else
            {
                ConsoleExtensions.WriteWithColors(ConsoleColor.Black, ConsoleColor.Blue,
                    $"\nНайден последний сохранённый файл настроек.\nДиректория: {pathToSettings}" +
                    $"\nНажмите enter, чтобы использовать данный файл, либо введите путь до файла настроек:");

                var input = Console.ReadLine();

                if (input.Equals(string.Empty))
                    path = pathToSettings;
                else
                    path = input;
            }

            var recognizeUtil = new RecognizeUtil(path);

            while (true)
            {
                ConsoleExtensions.WriteWithColors(ConsoleColor.Black, ConsoleColor.Green,
                    "\nВведите путь до файла с изображением, чтобы распознать его:");

                var pathToImage = Console.ReadLine();

                if (!File.Exists(pathToImage))
                    throw new Exception($"Не удалось найти файл по указанному пути!\nДиректория: {pathToImage}");

                var image = PathToImageConverter.LoadImages(new List<string> { pathToImage }).First();
                var resizedImage = NormilizeUtil.ResizeImage(image, 6, 6);

                var normilizedMatrix = NormilizeUtil.GetNormilizedMatrixFromImage(resizedImage);
                var answer = recognizeUtil.ToRecognizeData(normilizedMatrix);

                ConsoleExtensions.WriteWithColors(ConsoleColor.Black, ConsoleColor.Cyan, answer);
                Console.WriteLine("Введите 0, чтобы распознать другое изображение.\n" +
                    "Введите 1, чтобы выйти в меню.");

                var input = Console.ReadLine();

                if (input.Equals("1"))
                    break;
            }
        }

        /// <summary>
        /// Выполнить обучение.
        /// </summary>
        /// <param name="pathToSettings">Путь до сохранённого файла настроек.</param>
        private static void DoTrain(out string pathToSettings)
        {
            Console.Clear();
            ConsoleExtensions.WriteWithColors(ConsoleColor.Black, ConsoleColor.Green,
                "Вас приветствует обучение!\nУкажите директорию обучающей выборки " +
                "(enter для директории по-умолчанию):");

            var input = Console.ReadLine();

            if (input.Equals(string.Empty))
                input = PathHelper.GetResourcesPath();

            if (!Directory.Exists(input))
                throw new Exception($"Указанная директория не существует!\nДиректория: {input}");

            var directories = Directory.GetDirectories(input).ToList();
            var matrixDictionary = new Dictionary<int, List<double[,]>>();

            var key = 0;
            foreach (var directory in directories)
            {
                var files = Directory.GetFiles(directory).ToList();

                if (!files.Any())
                    throw new Exception($"Файлы не найдены!\nДиректория: {directory}");

                var images = PathToImageConverter.LoadImages(files);
                var resizedImages = NormilizeUtil.ResizeImages(images, 6, 6);

                var normilizedMatrixies = NormilizeUtil.GetNormilizedMatrixesFromImages(resizedImages);
                matrixDictionary.Add(key, normilizedMatrixies);

                ++key;
            }

            var hyperParameters = new HyperParameters();
            var topology = new Topology();

            topology.Add(2, 2, LayerType.Convolution);
            topology.Add(3, 2, LayerType.Subsampling);
            topology.Add(4, 2, LayerType.Hidden, true);

            if (!topology.IsClosed)
                throw new Exception("Не удалось замкнуть топологию.");

            var dataSet = new DataSet(DataSetType.ForNumberRecognizing);

            if (!matrixDictionary.Count.Equals(dataSet.MaxCountInDataSet()))
                throw new Exception("Не соответсвие количества выборок для распознавания чисел!");

            foreach (var pair in matrixDictionary)
                dataSet.Add(pair.Value);

            var trainUtil = new TrainUtil(dataSet, TrainType.Backpropagation, hyperParameters, topology);
            trainUtil.Start(3, 2, out var error, out pathToSettings);

            var errorString = $"{Math.Round(error * 100, 2)}%";

            ConsoleExtensions.WriteWithColors(ConsoleColor.Black, ConsoleColor.Yellow,
                $"\nОшибка: {errorString}");

            if (pathToSettings.Equals(string.Empty))
                ConsoleExtensions.WriteWithColors(ConsoleColor.Black, ConsoleColor.Red,
                    "\nДанные не были сохранены!");
            else
                ConsoleExtensions.WriteWithColors(ConsoleColor.Black, ConsoleColor.Blue,
                    $"\nДанные сохранены!\nДиректория: {pathToSettings}");
        }
    }
}
