#!/usr/bin/env bash
# Batch 01 on cursor/unity-hud-backup: verify recovered runners present; no sprint replay yet.
set -euo pipefail
ROOT="$(cd "$(dirname "$0")/../.." && pwd)"
cd "$ROOT"
COUNT="$(find studio/hud-ledger/recovered -name '*.py' | wc -l)"
echo "recovered runners: $COUNT"
test "$COUNT" -ge 90
if command -v dotnet >/dev/null 2>&1; then
  dotnet test tools/SimTests/SimTests.csproj --filter "FullyQualifiedName~golden" -v q
else
  echo "dotnet not installed; skip golden (install SDK 8+ before sprint replay)"
fi