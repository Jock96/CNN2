namespace CNN.Extensions
{
    using System;

    /// <summary>
    /// Расширения консоли.
    /// </summary>
    internal static class ConsoleExtensions
    {
        /// <summary>
        /// Написать с цветами.
        /// </summary>
        /// <param name="background">Цвет фона.</param>
        /// <param name="foreground">Цвет шрифта.</param>
        /// <param name="message">Сообщение.</param>
        public static void WriteWithColors(ConsoleColor background, ConsoleColor foreground, string message)
        {
            Console.BackgroundColor = background;
            Console.ForegroundColor = foreground;

            Console.WriteLine(message);

            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
