namespace KnowledgePicker.WordCloud.Coloring
{
    public interface IColorizer
    {
        /// <summary>
        /// Gets the hex string color for text.
        /// </summary>
        /// <returns>Hex string color in format #RRGGBB.</returns>
        string GetColorAsHex();
    }
}
