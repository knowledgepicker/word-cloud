using System.Diagnostics.CodeAnalysis;

namespace KnowledgePicker.WordCloud.Primitives
{
    [SuppressMessage("Performance",
        "CA1815: Override equals and operator equals on value types")]
    public readonly struct PointD
    {
        public PointD(double x, double y)
        {
            X = x;
            Y = y;
        }

        public double Y { get; }
        public double X { get; }

        public override string ToString()
        {
            return $"[X={X}, Y={Y}]";
        }
    }
}
