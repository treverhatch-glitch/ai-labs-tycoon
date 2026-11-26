#!/usr/bin/env bash
# Run all headless Blender early-game asset scripts (props, staff, HUD/wisp).
set -euo pipefail
ROOT="$(cd "$(dirname "$0")/../.." && pwd)"
cd "$ROOT"
BLENDER="${BLENDER_BIN:-blender}"
if ! command -v "$BLENDER" >/dev/null; then
  echo "FAIL: blender not installed" >&2
  exit 1
fi
VER=$("$BLENDER" --version | head -1)
echo "Using $VER"
mapfile -t SCRIPTS < <(
  find tools/art/blender -maxdepth 1 -type f \( \
    -name 'garage-*.py' -o -name 'staff-*.py' -o -name 'hud-*.py' -o -name 'panel-*.py' \
    -o -name 'icon-gpu-hire.py' -o -name 'icon-week-advance.py' -o -name 'tex-*.py' \) \
    ! -name 'tex.py' | sort
)
for script in "${SCRIPTS[@]}"; do
  echo "=== $script ==="
  "$BLENDER" -b --python "$script" || { echo "FAIL $script" >&2; exit 1; }
done
"$BLENDER" -b --python tools/art/blender/blender_render_hud_wisp.py
python3 tools/art/generate_early_hud_icons.py
python3 tools/art/blender/merge_blender_manifests.py
echo "blender early-game OK"