#!/usr/bin/env bash
# Hero-only CC0 garage lane (one asset; pause for Hatch review).
set -euo pipefail
ROOT="$(cd "$(dirname "$0")/../.." && pwd)"
cd "$ROOT"
BLENDER="${BLENDER_BIN:-blender}"
command -v "$BLENDER" >/dev/null || { echo "FAIL: blender not installed" >&2; exit 1; }
if [[ ! -d tools/art/cc0/incoming/kenney_building-kit ]]; then
  echo "=== downloading CC0 packs ==="
  bash tools/art/cc0/download_cc0_packs.sh
fi
echo "=== garage-interior-room-kit (CC0 hero) ==="
"$BLENDER" -b --python tools/art/blender/garage-interior-room-kit.py
python3 tools/art/check_cc0_garage_hero.py
python3 tools/art/check_cc0_garage_preview.py
echo "cc0 garage hero OK"