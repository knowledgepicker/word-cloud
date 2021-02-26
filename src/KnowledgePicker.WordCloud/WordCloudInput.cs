using KnowledgePicker.WordCloud.Primitives;
using System.Collections.Generic;

namespace KnowledgePicker.WordCloud
{
    /// <summary>
    /// Input to <see cref="WordCloudGenerator"/>.
    /// </summary>
    public class WordCloudInput
    {
        public const string DefaultTextColor = "#000000";

        public WordCloudInput(IReadOnlyList<WordCloudEntry> entries)
        {
            Entries = entries;
        }

        public IReadOnlyList<WordCloudEntry> Entries { get; }
        public int Width { get; }
        public int Height { get; }
        public int MinFontSize { get; }
        public int MaxFontSize { get; }
        /// <summary>
        /// Margin around each word in the cloud.
        /// </summary>
        public double ItemMargin { get; }
        /// <summary>
        /// Absolute physical path to font for drawing text.
        /// </summary>
        public string? FontPath { get; }
        public string TextColor { get; } = DefaultTextColor;
        /// <summary>
        /// Rectangle where no words shall be placed.
        /// </summary>
        public RectangleD CutOut { get; }
    }
}
