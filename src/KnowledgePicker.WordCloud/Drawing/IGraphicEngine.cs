using KnowledgePicker.WordCloud.Primitives;
using KnowledgePicker.WordCloud.Sizers;
using System;

namespace KnowledgePicker.WordCloud.Drawing
{
    /// <summary>
    /// Can draw and measure texts.
    /// </summary>
    public interface IGraphicEngine : IDisposable
    {
        ISizer Sizer { get; }

        /// <summary>
        /// Measures <paramref name="text"/> with weight proportional to
        /// <paramref name="count"/>.
        /// </summary>
        /// <returns>
        /// Width and height of the <paramref name="text"/> and location of its
        /// baseline (x/y are horizontal/vertical offsets of the leftmost point
        /// of the baseline).
        /// </returns>
        RectangleD Measure(string text, int count);

        /// <summary>
        /// Draws <paramref name="text"/> with weight proportional to
        /// <paramref name="count"/>.
        /// </summary>
        /// <param name="measured">
        /// Result of <see cref="Measure(string, int)"/>.
        /// </param>
        void Draw(PointD location, RectangleD measured, string text, int count, string? colorHex = null);
    }

    public interface IGraphicEngine<TBitmap> : IGraphicEngine
    {
        TBitmap Bitmap { get; }
    }
}
