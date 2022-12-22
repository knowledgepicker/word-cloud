using KnowledgePicker.WordCloud.Primitives;
using System.Drawing;

namespace KnowledgePicker.WordCloud.Coloring
{
    public interface IColorizer
    {
        /// <summary>
        /// Gets color for the specified <paramref name="item"/>.
        /// </summary>
        /// <param name="item">The item being colored.</param>
        /// <returns>
        /// Can return <see langword="null"/> to use the default color
        /// (<see cref="WordCloudInput.TextColor"/>).
        /// </returns>
        Color? GetColor(LayoutItem item);
    }
}
