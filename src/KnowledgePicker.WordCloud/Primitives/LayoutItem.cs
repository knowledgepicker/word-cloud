namespace KnowledgePicker.WordCloud.Primitives
{
    /// <summary>
    /// Word arranged somewhere in word cloud.
    /// </summary>
    public record LayoutItem
    {
        public LayoutItem(WordCloudEntry entry, PointD location, RectangleD measured)
        {
            Entry = entry;
            Location = location;
            Measured = measured;
        }

        public WordCloudEntry Entry { get; }
        public PointD Location { get; }
        public RectangleD Measured { get; }
        /// <summary>
        /// Bounding box of the word.
        /// </summary>
        public RectangleD Rectangle => new RectangleD(Location, Measured.Size);
    }
}
