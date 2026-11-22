#!/bin/bash
set -euo pipefail
REPO="$(cd "$(dirname "$0")/.." && pwd)"
cd "$REPO"
step="${1:-all}"
run() { bash "$REPO/tools/golden-revert-batch13-run.sh" "$1"; }
if [ "$step" = "all" ]; then
  for n in 130 131 132 133 134; do run "$n" || exit 1; done
else
  run "$step"
fi