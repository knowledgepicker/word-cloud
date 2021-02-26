using KnowledgePicker.WordCloud.Collections;
using KnowledgePicker.WordCloud.Drawing;
using KnowledgePicker.WordCloud.Primitives;
using System.Collections.Generic;

namespace KnowledgePicker.WordCloud.Layouts
{
    public abstract class BaseLayout : ILayout
    {
        protected BaseLayout(WordCloudInput wordCloud)
        {
            var size = new SizeD(wordCloud.Width, wordCloud.Height);
            Surface = new RectangleD(new PointD(0, 0), size);
            QuadTree = new QuadTree<LayoutItem>(Surface);
            Center = new PointD(Surface.X + size.Width / 2, Surface.Y + size.Height / 2);
            WordCloud = wordCloud;
        }

        protected QuadTree<LayoutItem> QuadTree { get; }
        protected PointD Center { get; }
        protected RectangleD Surface { get; }
        protected WordCloudInput WordCloud { get; }

        public int Arrange(IEnumerable<WordCloudEntry> entries, IGraphicEngine engine)
        {
            foreach (var entry in entries)
            {
                RectangleD measured = engine.Measure(entry.Word, entry.Count);
                if (!TryFindFreeRectangle(measured.Size, out var freeRectangle)) break;
                QuadTree.Insert(new LayoutItem(entry, freeRectangle.Location, measured));
            }
            return QuadTree.Count;
        }

        public abstract bool TryFindFreeRectangle(SizeD size, out RectangleD foundRectangle);

        public IEnumerable<LayoutItem> GetWordsInArea(RectangleD area)
        {
            return QuadTree.Query(area);
        }

        protected bool IsInsideSurface(RectangleD targetRectangle)
        {
            return IsInside(Surface, targetRectangle);
        }

        protected bool IsTaken(RectangleD targetRectangle)
        {
            return QuadTree.HasContent(targetRectangle) ||
                WordCloud.CutOut.IntersectsWith(targetRectangle);
        }

        private static bool IsInside(RectangleD outer, RectangleD inner)
        {
            return
                inner.X >= outer.X &&
                inner.Y >= outer.Y &&
                inner.Bottom <= outer.Bottom &&
                inner.Right <= outer.Right;
        }
    }
}
