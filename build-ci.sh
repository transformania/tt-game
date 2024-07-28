#!/usr/bin/env bash
set -euox pipefail

export DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1
export DOTNET_CLI_TELEMETRY_OPTOUT=1
export DOTNET_NOLOGO=1

dotnet build
dotnet test --framework net8.0 /p:CollectCoverage=true

dotnet run --project src/TT.Console -- database up "$@"