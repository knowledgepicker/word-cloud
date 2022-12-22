using KnowledgePicker.WordCloud.Primitives;
using System;
using System.Drawing;

namespace KnowledgePicker.WordCloud.Coloring
{
    /// <summary>
    /// Allows random colors for the word cloud text.
    /// </summary>
    public class RandomColorizer : IColorizer
    {
        private readonly Random random;

        public RandomColorizer()
        {
            random = new Random();
        }

        public RandomColorizer(int seed)
        {
            random = new Random(seed);
        }

        public Color? GetColor(LayoutItem item)
        {
            return Color.FromArgb(
                random.Next(0, 255),
                random.Next(0, 255),
                random.Next(0, 255));
        }
    }
}
