using System;
using System.Drawing;

namespace KnowledgePicker.WordCloud.Drawing
{
    /// <summary>
    /// Allows random colors for the word cloud text
    /// </summary>
    public class RandomColorizer : IColorizer
    {
        /// <summary>
        /// Used to select random colors.
        /// </summary>
        private Random Random { get; set; } = new Random(Environment.TickCount);

        /// <summary>
        /// Gets a random color.
        /// </summary>
        /// <returns>Color</returns>
        private Color GetRandomColor()
        {
#pragma warning disable CA5394 // Do not use insecure randomness
            return Color.FromArgb(Random.Next(0, 255), Random.Next(0, 255), Random.Next(0, 255));
#pragma warning restore CA5394 // Do not use insecure randomness
        }

        /// <summary>
        /// Converts Color to Hext string
        /// </summary>
        /// <returns>Color</returns>
        private static string ConvertToHexString(Color c)
        {
            return $"#{c.R:X2}{c.G:X2}{c.B:X2}";
        }

        /// <summary>
        /// Gets the randon RGB color and
        /// returns a hex string
        /// </summary>
        /// <returns>Hexstring</returns>
        public string GetColorAsHex()
        {
            Color c = GetRandomColor();
            return ConvertToHexString(c);
        }

    }
}
