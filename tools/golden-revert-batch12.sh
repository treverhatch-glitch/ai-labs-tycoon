#!/bin/bash
# Verify batch-12 golden revert steps 125-129. Usage: ./tools/golden-revert-batch12.sh [all|125|...]
set -euo pipefail
DIR="$(cd "$(dirname "$0")" && pwd)"
if [ "${1:-all}" = all ]; then
  for n in 125 126 127 128 129; do "$DIR/golden-revert-batch12-run.sh" "$n"; done
else
  "$DIR/golden-revert-batch12-run.sh" "$1"
fi