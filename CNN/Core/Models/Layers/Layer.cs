namespace Core.Models.Layers
{
    using Enums;

    /// <summary>
    /// Слой.
    /// </summary>
    public abstract class Layer
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
    }
}
