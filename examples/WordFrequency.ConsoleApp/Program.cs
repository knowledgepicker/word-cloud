using KnowledgePicker.WordCloud;
using KnowledgePicker.WordCloud.Coloring;
using KnowledgePicker.WordCloud.Drawing;
using KnowledgePicker.WordCloud.Layouts;
using KnowledgePicker.WordCloud.Primitives;
using KnowledgePicker.WordCloud.Sizers;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace WordFrequency.ConsoleApp
{
    class Program
    {
        static int Main(string[] args)
        {
            var command = new RootCommand
            {
                new Argument<FileInfo>("output",
                    () => new FileInfo(Path.Join(Environment.CurrentDirectory, "output.png")),
                    "Path to the output file, default is `output.png`.")
            };
            command.Description = "Takes words on input and generates word cloud as PNG from them.";

            command.Handler = CommandHandler.Create<FileInfo>(output =>
            {
                // Process words on input.
                var freqs = new Dictionary<string, int>();
                var whitespaces = new Regex(@"\s+");

                while (Console.ReadLine() is string line)
                {
                    foreach (var word in whitespaces.Split(line))
                    {
                        if (!freqs.TryGetValue(word, out var freq))
                        {
                            freq = 0;
                        }
                        freqs[word] = freq + 1;
                    }
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
                var colorizer = new RandomColorizer(); // optional
                var wcg = new WordCloudGenerator<SKBitmap>(wordCloud, engine, layout, colorizer);

                // Draw the bitmap on white background.
                using var final = new SKBitmap(wordCloud.Width, wordCloud.Height);
                using var canvas = new SKCanvas(final);
                canvas.Clear(SKColors.White);
                canvas.DrawBitmap(wcg.Draw(), 0, 0);

                // Save to PNG.
                using var data = final.Encode(SKEncodedImageFormat.Png, 100);
                using var writer = output.Create();
                data.SaveTo(writer);
            });

            return command.Invoke(args);
        }
    }
}
