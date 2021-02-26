using KnowledgePicker.WordCloud.Drawing;
using KnowledgePicker.WordCloud.Internal;
using KnowledgePicker.WordCloud.Layouts;
using KnowledgePicker.WordCloud.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public WordCloudGenerator(WordCloudInput wordCloud,
            IGraphicEngine<TBitmap> engine, ILayout layout)
        {
            this.wordCloud = wordCloud;
            this.engine = engine;
            this.layout = layout;
        }

        private TBitmap Process(Action<IGraphicEngine, IEnumerable<LayoutItem>> handler)
        {
            // Arrange word cloud.
            var size = new SizeD(wordCloud.Width, wordCloud.Height);
            layout.Arrange(wordCloud.Entries, engine);

            // Process results.
            var area = new RectangleD(new PointD(0, 0), size);
            handler(engine, layout.GetWordsInArea(area));

            return engine.Bitmap;
        }

        public IEnumerable<(LayoutItem Item, double FontSize)> Arrange()
        {
            var result = Enumerable.Empty<(LayoutItem, double)>();
            using var _ = Process((engine, items) =>
            {
                // Just transform result.
                result = items.Select(item =>
                   (item, engine.Sizer.GetFontSize(item.Entry.Count)));
            }).AsDisposable();
            return result;
        }

        /// <summary>
        /// Draws the word cloud.
        /// </summary>
        public TBitmap Draw()
        {
            return Process((engine, items) =>
            {
                // Draw words.
                foreach (var item in items)
                    engine.Draw(item.Location, item.Measured, item.Entry.Word, item.Entry.Count);
            });
        }
    }
}
