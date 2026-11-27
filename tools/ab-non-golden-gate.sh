#!/bin/bash
# Era prepass stack 1: SimTests except GoldenRevert (serial; flock avoids disk races).
set -euo pipefail
REPO="$(cd "$(dirname "$0")/.." && pwd)"
DOTNET="${DOTNET:-/home/ubuntu/.dotnet/dotnet}"
export PATH="${PATH:-/home/ubuntu/.dotnet:/usr/bin}"
LOG="${ERA_PREPASS_NON_GOLDEN_LOG:-/tmp/ab-non-golden-gate-9b3a50bb.log}"
LOCK="${TMPDIR:-/tmp}/era-prepass-non-golden.lock"
exec 9>"$LOCK"
flock -x -w 7200 9
pkill -f "testhost.dll.*SimTests" 2>/dev/null || true
sleep 1
cd "$REPO/tools/SimTests"
{
  echo "=== ab-non-golden-gate $(date -u +%Y-%m-%dT%H:%M:%SZ) tip=$(git -C "$REPO" rev-parse --short HEAD) ==="
  "$DOTNET" build -v q -nologo
  set +e
  OUT=$("$DOTNET" test --no-build -v q \
    --filter "FullyQualifiedName!~GoldenRevert" \
    -- xUnit.MaxParallelThreads=1 2>&1)
  EC=$?
  set -e
  echo "$OUT"
  echo "$OUT" | grep -E "Passed!|Failed!" | tail -1
  echo "AB_NON_GOLDEN_EXIT $EC"
  exit "$EC"
} 2>&1 | tee "$LOG"