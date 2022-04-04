using KnowledgePicker.WordCloud.Drawing;
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
        private readonly IColorizer colorizer;

        public WordCloudGenerator(WordCloudInput wordCloud,
            IGraphicEngine<TBitmap> engine, ILayout layout, IColorizer colorizer)
        {
            this.wordCloud = wordCloud;
            this.engine = engine;
            this.layout = layout;
            this.colorizer = colorizer;
        }

        private T Process<T>(
            Func<IGraphicEngine<TBitmap>, IEnumerable<LayoutItem>, T> handler)
        {
            // Arrange word cloud.
            var size = new SizeD(wordCloud.Width, wordCloud.Height);
            layout.Arrange(wordCloud.Entries, engine);

            // Process results.
            var area = new RectangleD(new PointD(0, 0), size);
            return handler(engine, layout.GetWordsInArea(area));
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
                    engine.Draw(item.Location, item.Measured, item.Entry.Word, item.Entry.Count, colorizer.GetColorAsHex());
                return engine.Bitmap;
            });
        }
    }
}
