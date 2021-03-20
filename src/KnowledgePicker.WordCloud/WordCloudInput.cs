using KnowledgePicker.WordCloud.Primitives;
using System.Collections.Generic;

namespace KnowledgePicker.WordCloud
{
    /// <summary>
    /// Input to <see cref="WordCloudGenerator{TBitmap}"/>.
    /// </summary>
    public class WordCloudInput
    {
        public const string DefaultTextColor = "#000000";

        public WordCloudInput(IEnumerable<WordCloudEntry> entries)
        {
            Entries = entries;
        }

        public IEnumerable<WordCloudEntry> Entries { get; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int MinFontSize { get; set; }
        public int MaxFontSize { get; set; }
        /// <summary>
        /// Margin around each word in the cloud.
        /// </summary>
        public double ItemMargin { get; set; }
        public string TextColor { get; set; } = DefaultTextColor;
        /// <summary>
        /// Rectangle where no words shall be placed.
        /// </summary>
        public RectangleD CutOut { get; set; }
    }
}
