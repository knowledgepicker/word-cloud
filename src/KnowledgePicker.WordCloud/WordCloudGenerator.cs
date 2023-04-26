using KnowledgePicker.WordCloud.Coloring;
using KnowledgePicker.WordCloud.Drawing;
using KnowledgePicker.WordCloud.Layouts;
using KnowledgePicker.WordCloud.Primitives;
using KnowledgePicker.WordCloud.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

[assembly: CLSCompliant(false)]

namespace KnowledgePicker.WordCloud
{
    /// <summary>
    /// Service for arranging and drawing word clouds.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Inspired by https://archive.codeplex.com/?p=sourcecodecloud.
    /// </para>
    /// <para>
    /// This class is thread-safe.
    /// </para>
    /// </remarks>
    public class WordCloudGenerator<TBitmap>
    {
        private readonly WordCloudInput wordCloud;
        private readonly IGraphicEngine<TBitmap> engine;
        private readonly ILayout layout;
        private readonly IColorizer? colorizer;

        public WordCloudGenerator(WordCloudInput wordCloud,
            IGraphicEngine<TBitmap> engine, ILayout layout,
            IColorizer? colorizer = null)
        {
            this.wordCloud = wordCloud;
            this.engine = engine;
            this.layout = layout;
            this.colorizer = colorizer;
        }

        private T Process<T>(
            Func<IGraphicEngine<TBitmap>, IEnumerable<LayoutItem>, T> handler)
        {
            // Ensure state is not shared.
            // TODO: We should instead use factory pattern.
            // But that would be a big change in usage of this class.
            using var localEngine = engine.Clone();
            var localLayout = layout.Clone();

            // Arrange word cloud.
            var size = new SizeD(wordCloud.Width, wordCloud.Height);
            localLayout.Arrange(wordCloud.Entries, localEngine);

            // Process results.
            var area = new RectangleD(new PointD(0, 0), size);
            return handler(localEngine, localLayout.GetWordsInArea(area));
        }

        public IEnumerable<(LayoutItem Item, double FontSize)> Arrange()
        {
            return Process((engine, items) =>
            {
                // Just transform result.
                return items.Select(item =>
                   (item, engine.Sizer.GetFontSize(item.Entry.Count)));
            });
        }

        /// <summary>
        /// Draws the word cloud into <see
        /// cref="IGraphicEngine{TBitmap}.Bitmap"/>.
        /// </summary>
        public TBitmap Draw()
        {
            return Process((engine, items) =>
            {
                // Draw words.
                foreach (var item in items)
                {
                    engine.Draw(
                        item.Location,
                        item.Measured,
                        item.Entry.Word,
                        item.Entry.Count,
                        colorizer?.GetColor(item)?.ToHexString());
                }
                return engine.ExtractBitmap();
            });
        }

        public string? GetColorHexString(LayoutItem item)
        {
            return colorizer?.GetColor(item)?.ToHexString();
        }

        public string GetColorHexStringOrDefault(LayoutItem item)
        {
            return GetColorHexString(item) ?? wordCloud.TextColor;
        }
    }
}
