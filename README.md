# WordCloud for .NET

[![Nuget](https://img.shields.io/nuget/v/KnowledgePicker.WordCloud?logo=nuget)](https://www.nuget.org/packages/KnowledgePicker.WordCloud/)
[![GitHub](https://img.shields.io/github/last-commit/knowledgepicker/word-cloud/master?logo=github)](https://github.com/knowledgepicker/word-cloud)

`KnowledgePicker.WordCloud` is a modern (.NET Standard 2.0) and fast library for arranging and drawing [word clouds](https://knowledgepicker.com/t/427/tag-word-cloud) (a.k.a. tag clouds or wordle). It uses Quadtrees for blazing-fast performance. It is maintained by the [KnowledgePicker](https://knowledgepicker.com) team.

<!--
  URL needs to be used, so the README works on NuGet.org. See also
  https://learn.microsoft.com/en-us/nuget/nuget-org/package-readme-on-nuget-org#allowed-domains-for-images-and-badges.
-->
![Sample Word Cloud](https://raw.githubusercontent.com/knowledgepicker/word-cloud/master/example.png)

## How to use

1. Install [NuGet package `KnowledgePicker.WordCloud`](https://www.nuget.org/packages/KnowledgePicker.WordCloud/).

   > **Note** There's currently only one drawing engine based on [SkiaSharp](https://github.com/mono/SkiaSharp). On some platforms, additional dependencies need to be installed to make SkiaSharp work correctly. For example, on Linux, add [SkiaSharp.NativeAssets.Linux](https://www.nuget.org/packages/SkiaSharp.NativeAssets.Linux); for Blazor WebAssembly, add [SkiaSharp.Views.Blazor](https://www.nuget.org/packages/SkiaSharp.Views.Blazor).

2. Get collection of `WordCloudEntry`s. For example, suppose we have dictionary of word frequencies:

   ```cs
   var frequencies = new Dictionary<string, int>();
   // ...collect word frequencies somehow...
   IEnumerable<WordCloudEntry> wordEntries = frequencies.Select(p => new WordCloudEntry(p.Key, p.Value));
   ```

3. Create world cloud configuration:

   ```cs
   var wordCloud = new WordCloudInput(wordEntries)
   {
       Width = 1024,
       Height = 256,
       MinFontSize = 8,
       MaxFontSize = 32
   };
   ```

4. We need to create drawing engine, font sizer and layout. Currently, we use [SkiaSharp](https://github.com/mono/SkiaSharp) for fast cross-platform font measuring (and drawing). We also only support logarithmic font sizes and spiral layout. All these things are implemented in a generic way and can be easily extended (contributions are welcome).

   ```cs
   var sizer = new LogSizer(wordCloud);
   using var engine = new SkGraphicEngine(sizer, wordCloud);
   var layout = new SpiralLayout(wordCloud);
   var colorizer = new RandomColorizer(); // optional
   var wcg = new WordCloudGenerator<SKBitmap>(wordCloud, engine, layout, colorizer);
   ```

   You can also use `SpecificColorizer` to colorize specific words with chosen colors:

   ```cs
   var colorizer = new SpecificColorizer(
       new Dictionary<string, Color>
       {
           ["KnowledgePicker"] = Color.FromArgb(0x0f3057),
           ["WordCloud"] = Color.FromArgb(0xe25a5a)
       },
       fallback: new RandomColorizer()); // fallback argument is optional
   ```

   You can also optionally pass a font into the graphic engine:

   ```cs
   var typeface = SKTypeface.FromFamilyName("DejaVu Serif", SKFontStyle.Normal);
   using var engine = new SkGraphicEngine(sizer, wordCloud, typeface);
   ```

5. Now we can *arrange* the topic cloud:

   ```cs
   IEnumerable<(LayoutItem Item, double FontSize)> items = wcg.Arrange();
   ```

   And if we are in a Razor view of an ASP.NET Core application, for example, we can generate SVG from `items`:

   ```cshtml
   <svg viewBox="0,0,@wordCloud.Width,@wordCloud.Height">
   @foreach (var (item, fontSize) in items)
   {
       const string format = "0.##"; // Use at most 2 decimal places.
       var x = (item.Location.X - item.Measured.Left).ToString(format);
       var y = (item.Location.Y - item.Measured.Top).ToString(format);
       var fs = fontSize.ToString(format);
       var color = wcg.GetColorHexString(item);
       <text transform="translate(@x, @y)" font-size="@fs" fill="@color">@item.Entry.Word</text>
   }
   </svg>
   ```

6. Alternatively, we can *draw* the topic cloud (see also [example `WordFrequencies.ConsoleApp`](https://github.com/knowledgepicker/word-cloud/tree/master/examples/WordFrequency.ConsoleApp)):

   ```cs
   using var final = new SKBitmap(wordCloud.Width, wordCloud.Height);
   using var canvas = new SKCanvas(final);

   // Draw on white background.
   canvas.Clear(SKColors.White);
   using var bitmap = wcg.Draw();
   canvas.DrawBitmap(bitmap, 0, 0);

   // Save to PNG.
   using var data = final.Encode(SKEncodedImageFormat.Png, 100);
   using var writer = File.Create("output.png");
   data.SaveTo(writer);
   ```

## Algorithm

The world cloud algorithm was initially ported from [SourceCodeCloud](https://archive.codeplex.com/?p=sourcecodecloud). It uses [Quadtrees](https://en.wikipedia.org/wiki/Quadtree), hence it should be reasonably fast. It is inspired by [implementation of Wordle](https://stackoverflow.com/a/1478314) (once famous algorithm used on
now-defunct site [wordle.net](https://web.archive.org/web/20201206102909/http://www.wordle.net/)).

## Examples

Simple console application which draws word cloud PNG for words given on its standard input is [`WordFrequencies.ConsoleApp`](examples/WordFrequency.ConsoleApp).

This library is also used in production by [KnowledgePicker](https://knowledgepicker.com). They use it to draw [topic clouds for user profiles](https://knowledgepicker.com/profiles).

## Contributing

As mentioned [above](#how-to-use), only subset of functionality is implemented now, but all contributions are welcome. Feel free to open [issues](https://github.com/knowledgepicker/word-cloud/issues) and [pull requests](https://github.com/knowledgepicker/word-cloud/pulls).

### Testing

Tests are currently only supported on Linux, because they are snapshot tests (generating a word cloud image and comparing it byte-by-byte with a snapshot) and more work is needed to ensure this is cross-platform (e.g., use exactly the same font). On Windows, tests can be run in WSL (Visual Studio supports this directly). Tests are also automatically run in GitHub Actions.

### Release process

After pushing a tag, GitHub workflow `release.yml` is triggered which builds and publishes the NuGet package.
