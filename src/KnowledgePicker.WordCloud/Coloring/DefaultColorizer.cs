namespace KnowledgePicker.WordCloud.Coloring
{
    /// <summary>
    /// Gets the default color set in the WordCloudInput
    /// </summary>
    public class DefaultColorizer : IColorizer
    {
        /// <summary>
        /// Gets the hex string color for text
        /// </summary>
        /// <returns>Hex string color</returns>
        public string GetColorAsHex()
        {
            return WordCloudInput.DefaultTextColor;
        }
    }
}
