using System;
using System.Linq;

namespace KnowledgePicker.WordCloud.Sizers
{
    /// <summary>
    /// Converts counts to font sizes using <see cref="Math.Log(double)"/>.
    /// </summary>
    /// <remarks>
    /// Inspired by Wowchemy's Tag Cloud (commit
    /// <c>b647d7e81a5ac59326fe8e5a1424df6a8bff7146</c>, file
    /// <c>wowchemy/layouts/partials/widgets/tag_cloud.html</c>).
    /// </remarks>
    public class LogSizer : ISizer
    {
        private readonly int fontDelta, minFontSize;
        private readonly double minLog, divisor;

        public LogSizer(WordCloudInput wordCloud)
        {
            fontDelta = wordCloud.MaxFontSize - wordCloud.MinFontSize;
            minFontSize = wordCloud.MinFontSize;

            var wordCounts = wordCloud.Entries.Select(e => e.Count).DefaultIfEmpty(0);
            var minCount = wordCounts.Min();
            var maxCount = wordCounts.Max();

            minLog = Math.Log(minCount);
            divisor = Math.Log(maxCount) - minLog;
        }

        public double GetFontSize(int count)
        {
            var weight = divisor == 0 ? 1 : (Math.Log(count) - minLog) / divisor;
            return minFontSize + fontDelta * weight;
        }
    }
}
