namespace BL.Helper
{
    using System.Collections.Generic;
    using System.Drawing;

    /// <summary>
    /// Конвертер изображений.
    /// </summary>
    public static class PathToImageConverter
    {
        /// <summary>
        /// Загрузить изображение по указанным путям.
        /// </summary>
        /// <param name="pathes">Пути к изображениям.</param>
        /// <returns>Изображения.</returns>
        public static List<Bitmap> LoadImages(List<string> pathes)
        {
            var images = new List<Bitmap>();
            pathes.ForEach(path => images.Add(new Bitmap(path)));

            return images;
        }
    }
}
