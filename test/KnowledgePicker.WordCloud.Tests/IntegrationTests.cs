using KnowledgePicker.WordCloud.Coloring;
using KnowledgePicker.WordCloud.Drawing;
using KnowledgePicker.WordCloud.Layouts;
using KnowledgePicker.WordCloud.Primitives;
using KnowledgePicker.WordCloud.Sizers;
using KnowledgePicker.WordCloud.Utilities;
using SkiaSharp;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Xunit.Sdk;

namespace KnowledgePicker.WordCloud.Tests;

public class IntegrationTests
{
    const string sampleText =
        "WordCloud for NET is a modern NET Standard 2.0 and fast " +
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

    [Fact]
    public void ExampleTest()
    {
        // Act.
        var actual = GenerateWordCloud(sampleText);

        // Assert.
        AssertSnapshot("Assets/example.png", actual);
    }

    [Fact]
    public void RandomColorizer()
    {
        // Arrange.
        var colorizer = new RandomColorizer(seed: 42);

        // Act.
        var actual = GenerateWordCloud(sampleText, colorizer);

        // Assert.
        AssertSnapshot("Assets/random-colorizer.png", actual);
    }

    [Fact]
    public void SpecificColorizer()
    {
        // Arrange.
        var colorizer = new SpecificColorizer(new Dictionary<string, Color>
        {
            ["KnowledgePicker"] = Color.FromArgb(0x0f3057),
            ["WordCloud"] = Color.FromArgb(0xe25a5a)
        });

        // Act.
        var actual = GenerateWordCloud(sampleText, colorizer);

        // Assert.
        AssertSnapshot("Assets/specific-colorizer.png", actual);
    }

    [Fact]
    public void SpecificColorizer_PurpleFallback()
    {
        // Arrange.
        var colorizer = new SpecificColorizer(new Dictionary<string, Color>
        {
            ["KnowledgePicker"] = Color.FromArgb(0x0f3057),
            ["WordCloud"] = Color.FromArgb(0xe25a5a)
        });

        // Act.
        var actual = GenerateWordCloud(sampleText, colorizer, static (input) =>
        {
            input.TextColor = Color.Purple.ToHexString();
        });

        // Assert.
        AssertSnapshot("Assets/specific-colorizer-purple-fallback.png", actual);
    }

    [Fact]
    public void SpecificColorizer_RandomFallback()
    {
        // Arrange.
        var fallback = new RandomColorizer(seed: 42);
        var colorizer = new SpecificColorizer(
            new Dictionary<string, Color>
            {
                ["KnowledgePicker"] = Color.FromArgb(0x0f3057),
                ["WordCloud"] = Color.FromArgb(0xe25a5a)
            },
            fallback);

        // Act.
        var actual = GenerateWordCloud(sampleText, colorizer);

        // Assert.
        AssertSnapshot("Assets/specific-colorizer-random-fallback.png", actual);
    }

    private static string Resolve(string fileName,
        [CallerFilePath] string testFilePath = null!)
    {
        var directoryPath = Path.GetDirectoryName(testFilePath);
        return Path.Join(directoryPath, fileName);
    }

    private static byte[] GenerateWordCloud(
        string text,
        IColorizer? colorizer = null,
        Action<WordCloudInput>? configureInput = null)
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

        // Load font.
        var typeface = SKTypeface.FromFamilyName("DejaVu Serif",
            SKFontStyle.Normal);

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
        configureInput?.Invoke(wordCloud);
        var sizer = new LogSizer(wordCloud);
        using var engine = new SkGraphicEngine(sizer, wordCloud, typeface);
        var layout = new SpiralLayout(wordCloud);
        var wcg = new WordCloudGenerator<SKBitmap>(wordCloud, engine, layout, colorizer);

        // Draw the bitmap on white background.
        using var final = new SKBitmap(wordCloud.Width, wordCloud.Height);
        using var canvas = new SKCanvas(final);
        canvas.Clear(SKColors.White);
        using var bitmap = wcg.Draw();
        canvas.DrawBitmap(bitmap, 0, 0);

        // Save to PNG.
        using var data = final.Encode(SKEncodedImageFormat.Png, 100);
        using var stream = new MemoryStream();
        data.SaveTo(stream);
        return stream.ToArray();
    }

    private static void AssertSnapshot(string path, byte[] actual)
    {
        path = Resolve(path);
        if (File.Exists(path))
        {
            try
            {
                Assert.Equal(File.ReadAllBytes(path), actual);
            }
            catch (EqualException)
            {
                File.WriteAllBytes(path, actual);
                throw;
            }
        }
        else
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            File.WriteAllBytes(path, actual);
        }
    }
}
