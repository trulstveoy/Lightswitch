$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$project = Join-Path $repoRoot "src\Lightswitch.App\Lightswitch.App.csproj"

Get-Process -Name "Lightswitch.App" -ErrorAction SilentlyContinue | Stop-Process -Force

dotnet restore $project -p:Platform=x64
dotnet run --project $project -c Debug -p:Platform=x64 --no-restore
