#!/usr/bin/env bash
# Packet 3–4: HUD glyphs, env props, HUD/garage tile textures (headless Blender).
set -euo pipefail
export EARLY_ART_PREVIEW_RIG=1
ROOT="$(cd "$(dirname "$0")/../.." && pwd)"
cd "$ROOT"
BLENDER="${BLENDER_BIN:-blender}"
mapfile -t SCRIPTS < <(
  printf '%s\n' \
    tools/art/blender/tex-hud-paper-warm.py \
    tools/art/blender/tex-garage-wall-beige.py \
    tools/art/blender/tex-garage-trim-orange.py \
    tools/art/blender/garage-exterior-wall.py \
    tools/art/blender/garage-land-for-sale-sign.py \
    tools/art/blender/garage-lot-curb.py \
    tools/art/blender/garage-window-frame.py \
    tools/art/blender/icon-cash-era0.py \
    tools/art/blender/icon-date-era0.py \
    tools/art/blender/icon-era-bar.py \
    tools/art/blender/icon-build-panel.py \
    tools/art/blender/icon-hire-panel.py \
    tools/art/blender/icon-train-panel.py \
    tools/art/blender/icon-gpu-hire.py \
    tools/art/blender/icon-week-advance.py \
    tools/art/blender/panel-build-bg.py \
    tools/art/blender/panel-hire-bg.py \
    tools/art/blender/panel-train-bg.py \
    tools/art/blender/hud-topbar-beige.py
)
for script in "${SCRIPTS[@]}"; do
  echo "=== $script ==="
  "$BLENDER" -b --python-exit-code 1 --python "$script"
done
python3 tools/art/blender/merge_blender_manifests.py
python3 tools/art/generate_early_hud_icons.py
python3 tools/art/check_env_mesh_topology.py || exit 1
python3 tools/art/check_early_packet34_previews.py || exit 1
echo "packet34 blender OK"