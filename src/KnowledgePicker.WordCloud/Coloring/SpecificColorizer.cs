using KnowledgePicker.WordCloud.Primitives;
using System.Collections.Generic;
using System.Drawing;

namespace KnowledgePicker.WordCloud.Coloring
{
    /// <summary>
    /// Colors specific words with provided colors.
    /// </summary>
    public class SpecificColorizer : IColorizer
    {
        private readonly IReadOnlyDictionary<string, Color> mapping;
        private readonly IColorizer? fallback;

        public SpecificColorizer(
            IReadOnlyDictionary<string, Color> mapping,
            IColorizer? fallback = null)
        {
            this.mapping = mapping;
            this.fallback = fallback;
        }

        public Color? GetColor(LayoutItem item)
        {
            if (mapping.TryGetValue(item.Entry.Word, out var color))
            {
                return color;
            }
            return fallback?.GetColor(item);
        }
    }
}
