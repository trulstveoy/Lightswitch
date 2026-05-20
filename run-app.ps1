$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$project = Join-Path $repoRoot "src\Lightswitch.Wpf\Lightswitch.Wpf.csproj"

Get-Process -Name "Lightswitch.Wpf" -ErrorAction SilentlyContinue | Stop-Process -Force

dotnet restore $project
dotnet run --project $project -c Debug --no-restore
