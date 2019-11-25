namespace Core.Models.Layers
{
    using Enums;

    /// <summary>
    /// Слой.
    /// </summary>
    internal abstract class Layer
    {
        /// <summary>
        /// Тип слоя.
        /// </summary>
        public abstract LayerType Type { get; }

        /// <summary>
        /// Инициализация.
        /// </summary>
        /// <param name="type">Тип мода нейронной сети.</param>
        public abstract void Initialize(NetworkModeType type);

        /// <summary>
        /// Вернуть данные слоя.
        /// </summary>
        /// <param name="returnType">Возвращаемый тип.</param>
        /// <returns>Возвращает данные слоя.</returns>
        public abstract dynamic GetData(LayerReturnType returnType);
    }
}
