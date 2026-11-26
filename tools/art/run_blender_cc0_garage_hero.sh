#!/usr/bin/env bash
# Hero-only CC0 garage lane (one asset; pause for Hatch review).
set -euo pipefail
ROOT="$(cd "$(dirname "$0")/../.." && pwd)"
cd "$ROOT"
BLENDER="${BLENDER_BIN:-blender}"
command -v "$BLENDER" >/dev/null || { echo "FAIL: blender not installed" >&2; exit 1; }
if [[ ! -f unity/Assets/Art/EarlyGame/garage-kit/garage-floor-tile.glb ]]; then
  echo "FAIL: missing garage-kit GLBs; run tools/art/generate_garage_kit.py" >&2
  exit 1
fi
if [[ ! -d tools/art/cc0/incoming/polyhaven ]] || [[ ! -f tools/art/cc0/incoming/polyhaven/studio_small_08_1k.hdr ]]; then
  echo "=== downloading CC0 packs (HDRI) ==="
  bash tools/art/cc0/download_cc0_packs.sh
fi
echo "=== garage-interior-room-kit (CC0 hero) ==="
"$BLENDER" -b --python tools/art/blender/garage-interior-room-kit.py
python3 tools/art/check_no_tracked_pyc.py
python3 tools/art/check_cc0_garage_hero.py
python3 tools/art/check_cc0_garage_scene.py
git add docs/art/previews/garage-interior-room-kit.png \
  docs/art/previews/garage-interior-room-kit-ortho-top.png \
  docs/art/previews/garage-interior-room-kit-ortho-front.png \
  docs/art/previews/garage-interior-room-kit-scene-gates.json
for f in docs/art/previews/garage-interior-room-kit-ortho-*.png.gate.json; do
  [ -f "$f" ] && git add "$f"
done
python3 tools/art/check_cc0_garage_preview.py
echo "cc0 garage hero OK"