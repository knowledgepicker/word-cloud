using System.Drawing;

namespace KnowledgePicker.WordCloud.Utilities
{
    internal static class ColorUtil
    {
        /// <summary>
        /// Converts <see cref="Color"/> to hex string.
        /// </summary>
        public static string ToHexString(this Color c)
        {
            return $"#{c.R:X2}{c.G:X2}{c.B:X2}";
        }
    }
}
