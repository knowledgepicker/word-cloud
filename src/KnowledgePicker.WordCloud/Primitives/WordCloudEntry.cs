namespace KnowledgePicker.WordCloud.Primitives
{
    public class WordCloudEntry
    {
        public WordCloudEntry(string word, int count)
        {
            Word = word;
            Count = count;
        }

        public string Word { get; }
        public int Count { get; }
    }
}
