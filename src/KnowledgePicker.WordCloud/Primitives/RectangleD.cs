using System.Diagnostics.CodeAnalysis;

namespace KnowledgePicker.WordCloud.Primitives
{
    [SuppressMessage("Performance",
        "CA1815: Override equals and operator equals on value types")]
    public readonly struct RectangleD
    {
        public static readonly RectangleD Empty;

        public RectangleD(double x, double y, double width, double height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public RectangleD(PointD location, SizeD size)
        {
            X = location.X;
            Y = location.Y;
            Width = size.Width;
            Height = size.Height;
        }

        public double X { get; }
        public double Y { get; }
        public double Width { get; }
        public double Height { get; }

        public PointD Location => new PointD(X, Y);

        public double Left => X;

        public double Top => Y;

        public double Right => X + Width;

        public double Bottom => Y + Height;

        public SizeD Size => new SizeD(Width, Height);

        public bool IsEmpty => Width <= 0 || Height <= 0;

        public override string ToString()
        {
            return $"[X={X}, Y={Y}, Width={Width}, Height={Height}]";
        }

        public bool Contains(RectangleD rectangle)
        {
            return (X <= rectangle.X) && ((rectangle.X + rectangle.Width) <= (X + Width)) &&
                (Y <= rectangle.Y) && ((rectangle.Y + rectangle.Height) <= (Y + Height));
        }

        public bool IntersectsWith(RectangleD rect)
        {
            return (rect.X < (X + Width)) && (X < (rect.X + rect.Width)) &&
                (rect.Y < (Y + Height)) && (Y < (rect.Y + rect.Height));
        }
    }
}
