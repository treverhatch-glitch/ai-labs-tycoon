#!/usr/bin/env bash
# Quality pass packet 1: rebake + studio previews for 15 garage props.
set -euo pipefail
export EARLY_ART_PREVIEW_RIG=1
ROOT="$(cd "$(dirname "$0")/../.." && pwd)"
cd "$ROOT"
BLENDER="${BLENDER_BIN:-blender}"
mapfile -t SCRIPTS < <(find tools/art/blender -maxdepth 1 -name 'garage-*.py' | sort)
for script in "${SCRIPTS[@]}"; do
  echo "=== $script ==="
  "$BLENDER" -b --python "$script" || { echo "FAIL $script" >&2; exit 1; }
done
python3 tools/art/blender/merge_blender_manifests.py
python3 tools/art/check_garage_quality_previews.py
echo "quality packet1 garage OK"