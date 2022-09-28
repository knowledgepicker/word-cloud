using KnowledgePicker.WordCloud.Drawing;
using KnowledgePicker.WordCloud.Layouts;
using KnowledgePicker.WordCloud.Primitives;
using KnowledgePicker.WordCloud.Sizers;
using SkiaSharp;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace KnowledgePicker.WordCloud.Tests;

public class IntegrationTests
{
    [Fact]
    public void ExampleTest()
    {
        // Arrange.
        var path = Resolve("Assets/example.png");
        var text = "WordCloud for NET is a modern NET Standard 2.0 and fast " +
            "library for arranging and drawing word clouds a.k.a tag clouds " +
            "or wordle It uses Quadtrees for blazing-fast performance It is " +
            "maintained by the KnowledgePicker team How to use Install NuGet " +
            "package KnowledgePicker WordCloud Get collection of " +
            "WordCloudEntrys For example suppose we have dictionary of word " +
            "frequencies Create world cloud configuration We need to create " +
            "drawing engine font sizer and layout Currently we use SkiaSharp " +
            "for fast cross-platform font measuring and drawing We also only " +
            "support logarithmic font sizes and spiral layout All these " +
            "things are implemented in a generic way and can be easily " +
            "extended contributions are welcome Now we can arrange the topic " +
            "cloud And if we are in a Razor view of an ASP.NET Core " +
            "application for example we can generate SVG from items " +
            "Alternatively we can draw the topic cloud see also example " +
            "WordFrequencies ConsoleApp Algorithm The world cloud algorithm " +
            "was initially ported from SourceCodeCloud It uses Quadtrees " +
            "hence it should be reasonably fast It is inspired by " +
            "implementation of Wordle once famous algorithm used on " +
            "now-defunct site wordle.net Examples Simple console application " +
            "which draws word cloud PNG for words given on its standard " +
            "input is WordFrequencies ConsoleApp This library is also used " +
            "in production by KnowledgePicker They use it to draw topic " +
            "clouds for user profiles Contributing As mentioned above only " +
            "subset of functionality is implemented now but all contributions " +
            "are welcome Feel free to open issues and pull requests Creating " +
            "NuGet package Until we have a CI pipeline this is how we release " +
            "new version of the package do n't forget to replace 1.0.0 with " +
            "the correct version";

        // Act.
        var actual = GenerateWordCloud(text);

        // Assert.
        if (File.Exists(path))
        {
            Assert.Equal(File.ReadAllBytes(path), actual);
        }
        else
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            File.WriteAllBytes(path, actual);
        }
    }

    private static string Resolve(string fileName,
        [CallerFilePath] string testFilePath = null!)
    {
        var directoryPath = Path.GetDirectoryName(testFilePath);
        return Path.Join(directoryPath, fileName);
    }

    private static byte[] GenerateWordCloud(string text)
    {
        // Process words on input.
        var freqs = new Dictionary<string, int>();
        var whitespaces = new Regex(@"\s+");
        foreach (var word in whitespaces.Split(text))
        {
            if (!freqs.TryGetValue(word, out var freq))
            {
                freq = 0;
            }
            freqs[word] = freq + 1;
        }

        // Generate topic cloud.
        const int k = 4; // scale
        var wordCloud = new WordCloudInput(
            freqs.Select(p => new WordCloudEntry(p.Key, p.Value)))
        {
            Width = 1024 * k,
            Height = 256 * k,
            MinFontSize = 8 * k,
            MaxFontSize = 32 * k
        };
        var sizer = new LogSizer(wordCloud);
        using var engine = new SkGraphicEngine(sizer, wordCloud);
        var layout = new SpiralLayout(wordCloud);
        var wcg = new WordCloudGenerator<SKBitmap>(wordCloud, engine, layout);

        // Draw the bitmap on white background.
        using var final = new SKBitmap(wordCloud.Width, wordCloud.Height);
        using var canvas = new SKCanvas(final);
        canvas.Clear(SKColors.White);
        canvas.DrawBitmap(wcg.Draw(), 0, 0);

        // Save to PNG.
        using var data = final.Encode(SKEncodedImageFormat.Png, 100);
        using var stream = new MemoryStream();
        data.SaveTo(stream);
        return stream.ToArray();
    }
}