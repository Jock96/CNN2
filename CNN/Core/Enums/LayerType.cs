namespace Core.Enums
{
    /// <summary>
    /// Типы слоёв.
    /// </summary>
    public enum LayerType
    {
        /// <summary>
        /// Входной.
        /// </summary>
        Input,

        /// <summary>
        /// Свёрточный.
        /// </summary>
        Convolution,

        /// <summary>
        /// Пуллинговый. 
        /// </summary>
        Subsampling,

        /// <summary>
        /// Скрытый.
        /// </summary>
        Hidden,

        /// <summary>
        /// Выходной.
        /// </summary>
        Output
    }
}
