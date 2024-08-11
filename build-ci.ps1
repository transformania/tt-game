$ErrorActionPreference = 'Stop'

Set-Location -LiteralPath $PSScriptRoot

$env:DOTNET_SKIP_FIRST_TIME_EXPERIENCE = '1'
$env:DOTNET_CLI_TELEMETRY_OPTOUT = '1'
$env:DOTNET_NOLOGO = '1'

dotnet tool restore
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

dotnet cake @args
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

dotnet run --project src\TT.Console database recreate -Y
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

dotnet test --collect:"XPlat Code Coverage" --logger:junit
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

dotnet reportgenerator -reports:src\TT.Tests\TestResults\**\coverage.cobertura.xml -targetdir:.\coverage -reporttypes:"Html;TeamCitySummary" -verbosity:Info
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }