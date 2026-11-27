#!/bin/bash
set -euo pipefail
REPO="$(cd "$(dirname "$0")/.." && pwd)"
DOTNET="${DOTNET:-/home/ubuntu/.dotnet/dotnet}"
export PATH="${PATH:-/home/ubuntu/.dotnet:/usr/bin}"
cd "$REPO/tools/SimTests"
"$DOTNET" build -v q -nologo
export DOTNET_CLI_UI_LANGUAGE=en
export DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1
"$DOTNET" test --no-build -v q --filter "FullyQualifiedName!~GoldenRevert" "$@"