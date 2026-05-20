param(
    [string]$Configuration = "Release",
    [string]$Runtime = "win-x64",
    [string]$Version = "local",
    [string]$OutputRoot = "artifacts"
)

$ErrorActionPreference = "Stop"

$repoRoot = Resolve-Path (Join-Path $PSScriptRoot "..")
$projectPath = Join-Path $repoRoot "src\Lightswitch.Wpf\Lightswitch.Wpf.csproj"
$outputRootPath = Join-Path $repoRoot $OutputRoot
$packageName = "Lightswitch-$Version-$Runtime"
$publishPath = Join-Path $outputRootPath "publish\$packageName"
$zipPath = Join-Path $outputRootPath "$packageName.zip"

if (Test-Path $publishPath) {
    Remove-Item -LiteralPath $publishPath -Recurse -Force
}

if (Test-Path $zipPath) {
    Remove-Item -LiteralPath $zipPath -Force
}

New-Item -ItemType Directory -Path $publishPath -Force | Out-Null
New-Item -ItemType Directory -Path $outputRootPath -Force | Out-Null

dotnet publish $projectPath `
    -c $Configuration `
    -r $Runtime `
    --self-contained true `
    -p:PublishSingleFile=true `
    -p:IncludeNativeLibrariesForSelfExtract=true `
    -p:PublishDir="$publishPath\"

Compress-Archive -Path (Join-Path $publishPath "*") -DestinationPath $zipPath -Force

Write-Host "Created package: $zipPath"
Write-Output $zipPath
