using KnowledgePicker.WordCloud.Primitives;
using System;

namespace KnowledgePicker.WordCloud.Layouts
{
    /// <summary>
    /// Arranges words on a spiral starting in center and going outwards.
    /// </summary>
    public class SpiralLayout : BaseLayout
    {
        public SpiralLayout(WordCloudInput wordCloud) : base(wordCloud) { }

        public override bool TryFindFreeRectangle(SizeD size, out RectangleD foundRectangle)
        {
            foundRectangle = RectangleD.Empty;
            double alpha = GetPseudoRandomStartAngle(size);
            const double stepAlpha = Math.PI / 60;

            const double pointsOnSpital = 500;

            for (int pointIndex = 0; pointIndex < pointsOnSpital; pointIndex++)
            {
                double dX = pointIndex / pointsOnSpital * Math.Sin(alpha) * Center.X;
                double dY = pointIndex / pointsOnSpital * Math.Cos(alpha) * Center.Y;
                foundRectangle = new RectangleD(Center.X + dX - size.Width / 2,
                    Center.Y + dY - size.Height / 2, size.Width, size.Height);

                alpha += stepAlpha;
                if (!IsInsideSurface(foundRectangle))
                {
                    return false;
                }

                if (!IsTaken(foundRectangle))
                {
                    return true;
                }
            }

            return false;
        }

        public override ILayout Clone()
        {
            return new SpiralLayout(WordCloud);
        }

        private static double GetPseudoRandomStartAngle(SizeD size)
        {
            return size.Height * size.Width;
        }
    }
}
