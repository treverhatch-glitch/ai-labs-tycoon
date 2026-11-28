#!/bin/bash
# Era prepass stack 2: GoldenRevert + golden determinism (serial; exclusive lock).
set -euo pipefail
REPO="$(cd "$(dirname "$0")/.." && pwd)"
DOTNET="${DOTNET:-/home/ubuntu/.dotnet/dotnet}"
export PATH="${PATH:-/home/ubuntu/.dotnet:/usr/bin}"
LOG="${ERA_PREPASS_GOLDEN_LOG:-/tmp/era-prepass-golden-serial-9b3a50bb.log}"
LOCK="${TMPDIR:-/tmp}/era-prepass-golden.lock}"
exec 9>"$LOCK"
flock -x -w 14400 9
pkill -f "testhost.dll.*SimTests" 2>/dev/null || true
sleep 1
cd "$REPO/tools/SimTests"
{
  echo "=== era-prepass-golden-serial $(date -u +%Y-%m-%dT%H:%M:%SZ) tip=$(git -C "$REPO" rev-parse --short HEAD) ==="
  "$DOTNET" build -v q -nologo
  set +e
  OUT=$("$DOTNET" test --no-build -v q \
    --filter "FullyQualifiedName~GoldenRevertBatchTwentyFive|FullyQualifiedName~GoldenRevertBatchTwentySix|FullyQualifiedName~GoldenRevertBatchTwentySeven|FullyQualifiedName~GoldenRevertBatchTwentyEight|FullyQualifiedName~GoldenRevertBatchTwentyNine|FullyQualifiedName~The_golden_run" \
    -- xUnit.MaxParallelThreads=1 2>&1)
  EC=$?
  set -e
  echo "$OUT"
  echo "$OUT" | grep -E "Passed!|Failed!" | tail -1
  echo "ERA_PREPASS_GOLDEN_EXIT $EC"
  exit "$EC"
} 2>&1 | tee "$LOG"