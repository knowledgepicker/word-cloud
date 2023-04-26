using KnowledgePicker.WordCloud.Coloring;
using KnowledgePicker.WordCloud.Drawing;
using KnowledgePicker.WordCloud.Layouts;
using KnowledgePicker.WordCloud.Primitives;
using KnowledgePicker.WordCloud.Sizers;
using KnowledgePicker.WordCloud.Utilities;
using SkiaSharp;
using System.Drawing;

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

    [Fact] // https://github.com/knowledgepicker/word-cloud/issues/23
    public void CanGetColor()
    {
        // Arrange.
        var wordCloud = new WordCloudInput(new[]
        {
            new WordCloudEntry("a", 1),
            new WordCloudEntry("b", 1),
            new WordCloudEntry("c", 1),
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
        var color1 = Color.FromArgb(0x0f3057);
        var color2 = Color.FromArgb(0xe25a5a);
        var colorizer = new SpecificColorizer(
            new Dictionary<string, Color>
            {
                ["a"] = color1,
                ["b"] = color2
            });
        var wcg = new WordCloudGenerator<SKBitmap>(wordCloud, engine, layout, colorizer);

        // Act.
        foreach (var (item, fontSize) in wcg.Arrange())
        {
            var actualColor = wcg.GetColorHexString(item);
            var actualColorOrDefault = wcg.GetColorHexStringOrDefault(item);

            // Assert.
            var expectedColor = item.Entry.Word switch
            {
                "a" => color1.ToHexString(),
                "b" => color2.ToHexString(),
                _ => null
            };
            var expectedColorOrDefault = expectedColor ?? WordCloudInput.DefaultTextColor;
            Assert.Equal(expectedColor, actualColor);
            Assert.Equal(expectedColorOrDefault, actualColorOrDefault);
        }
    }
}
