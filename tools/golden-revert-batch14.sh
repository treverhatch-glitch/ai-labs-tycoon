#!/bin/bash
set -euo pipefail
REPO="$(cd "$(dirname "$0")/.." && pwd)"
cd "$REPO"
step="${1:-all}"
run() { bash "$REPO/tools/golden-revert-batch14-run.sh" "$1"; }
if [ "$step" = "all" ]; then for n in 135 136 137 138; do run "$n" || exit 1; done; else run "$step"; fi
