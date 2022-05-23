using System.Diagnostics.CodeAnalysis;

namespace KnowledgePicker.WordCloud.Primitives
{
    [SuppressMessage("Performance",
        "CA1815: Override equals and operator equals on value types")]
    public readonly struct SizeD
    {
        public SizeD(double width, double height)
        {
            Width = width;
            Height = height;
        }

        public double Width { get; }
        public double Height { get; }

        public override string ToString()
        {
            return $"[Width={Width}, Height={Height}]";
        }
    }
}
