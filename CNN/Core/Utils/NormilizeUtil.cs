namespace Core.Utils
{
    using System.Collections.Generic;
    using System.Drawing;

    /// <summary>
    /// Инструмент нормализации данных.
    /// </summary>
    public static class NormilizeUtil
    {
        /// <summary>
        /// Изменить размер изображения.
        /// </summary>
        /// <param name="image">Изображение.</param>
        /// <param name="height">Высота.</param>
        /// <param name="width">Ширина.</param>
        /// <returns>Возвращает изображение с изменённой размерностью.</returns>
        public static Bitmap ResizeImage(this Bitmap image, int height, int width)
            => new Bitmap(image, new Size(width, height));

        /// <summary>
        /// Изменить размер изображений.
        /// </summary>
        /// <param name="image">Изображения.</param>
        /// <param name="height">Высота.</param>
        /// <param name="width">Ширина.</param>
        /// <returns>Возвращает изображения с изменённой размерностью.</returns>
        public static List<Bitmap> ResizeImages(this List<Bitmap> images, int height, int width)
        {
            var resizedImages = new List<Bitmap>();

            images.ForEach(image => resizedImages.Add(ResizeImage(image, height, width)));

            return resizedImages;
        }

        /// <summary>
        /// Получить нормализованные матрицы из изображений.
        /// </summary>
        /// <param name="image">Изображения.</param>
        /// <returns>Возвращает нормализованные матрицы.</returns>
        public static List<double[,]> GetNormilizedMatrixesFromImages(this List<Bitmap> images)
        {
            var matrixes = new List<double[,]>();

            images.ForEach(image => matrixes.Add(image.GetNormilizedMatrixFromImage()));

            return matrixes;
        }

        /// <summary>
        /// Получить нормализованную матрицу из изображения.
        /// </summary>
        /// <param name="image">Изображение.</param>
        /// <returns>Возвращает нормализованную матрицу.</returns>
        public static double[,] GetNormilizedMatrixFromImage(this Bitmap image)
        {
            var matrix = new double[image.Width, image.Height];

            for (var width = 0;  width < image.Width; ++width)
                for (var height = 0; height < image.Height; ++height)
                {
                    var pixel = image.GetPixel(width, height);
                    matrix[width, height] = pixel.R / 255;
                }

            return matrix;
        }
    }
}
