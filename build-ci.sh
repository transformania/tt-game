#!/usr/bin/env bash
set -xeuo pipefail

export DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1
export DOTNET_CLI_TELEMETRY_OPTOUT=1
export DOTNET_NOLOGO=1

dotnet tool restore
dotnet cake "$@"
dotnet run --project src/TT.Console database recreate -Y
dotnet test --collect:"XPlat Code Coverage"
dotnet reportgenerator -reports:src/TT.Tests/TestResults/**/coverage.cobertura.xml -targetdir:./coverage -reporttypes:"Html;TeamCitySummary" -verbosity:Info
