# WordCloud for .NET

`KnowledgePicker.WordCloud` is a modern (.NET Standard 2.0) and fast library for
arranging and drawing [word
clouds](https://knowledgepicker.com/t/427/tag-word-cloud) (a.k.a. tag clouds or
wordle).

## Packaging

Until we create a CI pipeline, this is how we release new version of the package
(don't forget to replace 1.0.0 by the correct version):

```bash
cd src/KnowledgePicker.WordCloud
dotnet pack -c Release --include-symbols --include-source -p:PackageVersion=1.0.0
```
