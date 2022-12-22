using KnowledgePicker.WordCloud.Primitives;
using KnowledgePicker.WordCloud.Sizers;
using SkiaSharp;

namespace KnowledgePicker.WordCloud.Drawing
{
    /// <summary>
    /// <see cref="IGraphicEngine"/> that uses <see cref="SKCanvas"/>.
    /// </summary>
    public sealed class SkGraphicEngine : IGraphicEngine<SKBitmap>
    {
        private readonly SKCanvas canvas;
        private readonly SKColor defaultColor;
        private readonly SKPaint textPaint;
        private readonly WordCloudInput wordCloud;
        private bool bitmapExtracted;

        private SkGraphicEngine(ISizer sizer, WordCloudInput wordCloud,
            SKPaint textPaint)
        {
            Sizer = sizer;
            this.wordCloud = wordCloud;
            defaultColor = textPaint.Color;
            this.textPaint = textPaint;
            Bitmap = new SKBitmap(wordCloud.Width, wordCloud.Height);
            canvas = new SKCanvas(Bitmap);
        }

        public SkGraphicEngine(ISizer sizer, WordCloudInput wordCloud,
            SKTypeface? font = null, bool antialias = true)
        {
            Sizer = sizer;
            Bitmap = new SKBitmap(wordCloud.Width, wordCloud.Height);
            canvas = new SKCanvas(Bitmap);
            defaultColor = SKColor.Parse(wordCloud.TextColor);
            textPaint = new SKPaint
            {
                Color = defaultColor,
                Typeface = font,
                IsAntialias = antialias
            };
            this.wordCloud = wordCloud;
        }

        public ISizer Sizer { get; }

        public SKBitmap Bitmap { get; }

        public RectangleD Measure(string text, int count)
        {
            textPaint.TextSize = (float)Sizer.GetFontSize(count);
            SKRect rect = new SKRect();
            textPaint.MeasureText(text, ref rect);
            var m = wordCloud.ItemMargin;
            return new RectangleD(rect.Left + m, rect.Top + m, rect.Width + 2 * m, rect.Height + 2 * m);
        }

        public void Draw(PointD location, RectangleD measured, string text, int count, string? colorHex = null)
        {
            // For computation explanation, see
            // https://docs.microsoft.com/en-us/xamarin/xamarin-forms/user-interface/graphics/skiasharp/basics/text.
            textPaint.TextSize = (float)Sizer.GetFontSize(count);
            if (colorHex != null)
            {
                textPaint.Color = SKColor.Parse(colorHex);
            }
            else
            {
                textPaint.Color = defaultColor;
            }
            canvas.DrawText(text, (float)(location.X - measured.Left),
                (float)(location.Y - measured.Top), textPaint);
        }

        public IGraphicEngine<SKBitmap> Clone()
        {
            var clonedTextPaint = textPaint.Clone();
            clonedTextPaint.Color = defaultColor;
            return new SkGraphicEngine(Sizer, wordCloud, clonedTextPaint);
        }

        public SKBitmap ExtractBitmap()
        {
            bitmapExtracted = true;
            return Bitmap;
        }

        public void Dispose()
        {
            textPaint.Dispose();
            canvas.Dispose();
            if (!bitmapExtracted)
            {
                Bitmap.Dispose();
            }
        }
    }
}
