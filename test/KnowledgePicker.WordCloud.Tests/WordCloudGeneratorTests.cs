using KnowledgePicker.WordCloud.Drawing;
using KnowledgePicker.WordCloud.Layouts;
using KnowledgePicker.WordCloud.Primitives;
using KnowledgePicker.WordCloud.Sizers;
using SkiaSharp;

namespace KnowledgePicker.WordCloud.Tests;

public class WordCloudGeneratorTests
{
    [Fact] // https://github.com/knowledgepicker/word-cloud/issues/17
    public void DoesNotShareState()
    {
        // Arrange.
        var wordCloud = new WordCloudInput(new[]
        {
            new WordCloudEntry("a", 1),
            new WordCloudEntry("b", 1),
        })
        {
            Width = 1024,
            Height = 256,
            MinFontSize = 8,
            MaxFontSize = 32
        };
        var sizer = new LogSizer(wordCloud);
        using var engine = new SkGraphicEngine(sizer, wordCloud);
        var layout = new SpiralLayout(wordCloud);
        var wcg = new WordCloudGenerator<SKBitmap>(wordCloud, engine, layout);

        // Act.
        var result1 = wcg.Arrange().ToArray();
        var result2 = wcg.Arrange().ToArray();

        // Assert.
        Assert.Equal(result1.AsEnumerable(), result2);
        Assert.Equal(2, result2.Length);
    }
}
