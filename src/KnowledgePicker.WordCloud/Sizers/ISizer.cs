namespace KnowledgePicker.WordCloud.Sizers
{
    /// <summary>
    /// Converter from count to font size.
    /// </summary>
    public interface ISizer
    {
        double GetFontSize(int count);
    }
}
