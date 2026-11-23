#!/bin/bash
# Refresh GOLDEN[...] entries after tip digest moves (run from repo root; does not commit).
set -euo pipefail
REPO="$(cd "$(dirname "$0")/.." && pwd)"
cd "$REPO"
export PATH="${PATH:-/home/ubuntu/.dotnet:/usr/bin}"

update_golden() {
  local script="$1" schema="$2" digest="$3"
  sed -i "s/^GOLDEN\\[$schema\\]=.*/GOLDEN[$schema]=$digest/" "$script"
}

run_one() {
  local script="$1" schema="$2"
  echo "=== $script schema $schema ==="
  local out
  out=$(bash "$script" "$schema" 2>&1) || true
  echo "$out" | tail -5
  if echo "$out" | grep -q "schema $schema revert: Passed!"; then
    echo "OK (already pinned)"
    return 0
  fi
  local new
  new=$(echo "$out" | sed -n 's/.*digest: \([0-9a-f]*\).*/\1/p' | tail -1)
  if [ -z "$new" ]; then
    new=$(echo "$out" | sed -n 's/.*New digest: \([0-9a-f]*\).*/\1/p' | tail -1)
  fi
  tip=$(sed -n 's/.*const string Golden = "\([0-9a-f]*\)".*/\1/p' "$REPO/tools/SimTests/SaveAndDeterminismTests.cs" | head -1)
  if [ -n "$new" ] && [ -n "$tip" ] && [ "$new" = "$tip" ]; then
    echo "SKIP digest equals tip (revert did not apply)"
    return 1
  fi
  if [ -z "$new" ]; then
    echo "FAILED: no digest"
    return 1
  fi
  update_golden "$script" "$schema" "$new"
  bash "$REPO/tools/restore-golden-tip.sh" >/dev/null
  out=$(bash "$script" "$schema" 2>&1) || true
  echo "$out" | tail -3
  echo "$out" | grep -q "schema $schema revert: Passed!"
}

for s in 126 127 128 129; do run_one "$REPO/tools/golden-revert-batch12-run.sh" "$s"; done
for s in 130 131 132 133 134; do run_one "$REPO/tools/golden-revert-batch13-run.sh" "$s"; done
for s in 135 136 137 138; do run_one "$REPO/tools/golden-revert-batch14-run.sh" "$s"; done
for s in 139 140 141 142 143; do run_one "$REPO/tools/golden-revert-batch15-run.sh" "$s"; done
for s in 144 145 146 147 148; do run_one "$REPO/tools/golden-revert-batch16-run.sh" "$s"; done
for s in 149 150 151 152 153; do run_one "$REPO/tools/golden-revert-batch17-run.sh" "$s"; done
for s in 154 155; do run_one "$REPO/tools/golden-revert-batch18-run.sh" "$s"; done
for s in 156 157; do run_one "$REPO/tools/golden-revert-batch19-run.sh" "$s"; done
for s in 158 159 160; do run_one "$REPO/tools/golden-revert-batch20-run.sh" "$s"; done
for s in 161 162 163; do run_one "$REPO/tools/golden-revert-batch21-run.sh" "$s"; done

echo "Done repin."