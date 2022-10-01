Set-StrictMode -version 2.0
$ErrorActionPreference = "Stop"

Write-Output "Working directory: $pwd"

# Load current Git tag.
$tag = $(git describe --tags)
Write-Output "Tag: $tag"

# Parse tag into a three-number version.
$version = $tag.Split('-')[0].TrimStart('v')
Write-Output "Version: $version"

Push-Location src/KnowledgePicker.WordCloud
try {
    # Pack the library.
    dotnet pack -c Release --include-symbols --include-source `
        -p:PackageVersion=$version

    # Push the NuGet package.
    dotnet nuget push `
        "bin/Release/KnowledgePicker.WordCloud.$version.symbols.nupkg" `
        --source https://api.nuget.org/v3/index.json `
        --api-key $env:NUGET_API_KEY
} finally {
    Pop-Location
}
