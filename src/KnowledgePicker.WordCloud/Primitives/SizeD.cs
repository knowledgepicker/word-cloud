namespace KnowledgePicker.WordCloud.Primitives
{
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
