#!/usr/bin/env bash
# Remaining early-lane HUD icons + 1024 texture rebakes (9 assets).
set -euo pipefail
ROOT="$(cd "$(dirname "$0")/../.." && pwd)"
cd "$ROOT"
BLENDER="${BLENDER_BIN:-blender}"
SCRIPTS=(
  tools/art/blender/hud-topbar-beige.py
  tools/art/blender/icon-gpu-hire.py
  tools/art/blender/icon-week-advance.py
  tools/art/blender/panel-build-bg.py
  tools/art/blender/panel-hire-bg.py
  tools/art/blender/panel-train-bg.py
  tools/art/blender/tex-garage-trim-orange.py
  tools/art/blender/tex-garage-wall-beige.py
  tools/art/blender/tex-hud-paper-warm.py
)
for script in "${SCRIPTS[@]}"; do
  echo "=== $script ==="
  "$BLENDER" -b --python "$script" || { echo "FAIL $script" >&2; exit 1; }
done
python3 tools/art/blender/sync_migration_inventory.py
echo "early-lane finish OK (9 assets)"