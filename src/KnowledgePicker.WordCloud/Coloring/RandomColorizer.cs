using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;

namespace KnowledgePicker.WordCloud.Coloring
{
    /// <summary>
    /// Allows random colors for the word cloud text.
    /// </summary>
    public class RandomColorizer : IColorizer
    {
        private readonly Random random;

        public RandomColorizer() : this(Environment.TickCount) { }

        public RandomColorizer(int seed)
        {
            random = new Random(seed);
        }

        /// <summary>
        /// Gets a random color.
        /// </summary>
        [SuppressMessage("Security", "CA5394:Do not use insecure randomness")]
        private Color GetRandomColor()
        {
            return Color.FromArgb(random.Next(0, 255), random.Next(0, 255), random.Next(0, 255));
        }

        /// <summary>
        /// Converts Color to hex string.
        /// </summary>
        private static string ConvertToHexString(Color c)
        {
            return $"#{c.R:X2}{c.G:X2}{c.B:X2}";
        }

        /// <summary>
        /// Gets the randon RGB color as a hex string.
        /// </summary>
        public string GetColorAsHex()
        {
            Color c = GetRandomColor();
            return ConvertToHexString(c);
        }

    }
}
