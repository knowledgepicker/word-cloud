using KnowledgePicker.WordCloud.Drawing;
using KnowledgePicker.WordCloud.Primitives;
using System.Collections.Generic;

namespace KnowledgePicker.WordCloud.Layouts
{
    /// <summary>
    /// Can arrange words into some cloud.
    /// </summary>
    public interface ILayout
    {
        int Arrange(IEnumerable<WordCloudEntry> entries, IGraphicEngine engine);
        IEnumerable<LayoutItem> GetWordsInArea(RectangleD area);
    }
}
