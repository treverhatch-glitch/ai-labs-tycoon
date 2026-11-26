#!/usr/bin/env bash
# Packet 3–4: env props (3D bpy), HUD (2D SVG sprites), tile textures.
set -euo pipefail
export EARLY_ART_PREVIEW_RIG=1
ROOT="$(cd "$(dirname "$0")/../.." && pwd)"
cd "$ROOT"
BLENDER="${BLENDER_BIN:-blender}"

ENV_SCRIPTS=(
  tools/art/blender/garage-exterior-wall.py
  tools/art/blender/garage-land-for-sale-sign.py
  tools/art/blender/garage-lot-curb.py
  tools/art/blender/garage-window-frame.py
)
TEX_SCRIPTS=(
  tools/art/blender/tex-hud-paper-warm.py
  tools/art/blender/tex-garage-wall-beige.py
  tools/art/blender/tex-garage-trim-orange.py
)

for script in "${ENV_SCRIPTS[@]}" "${TEX_SCRIPTS[@]}"; do
  echo "=== $script ==="
  "$BLENDER" -b --python-exit-code 1 --python "$script"
done
"$BLENDER" -b --python-exit-code 1 --python tools/art/blender/render_packet34_env_previews.py
python3 tools/art/rasterize_early_hud_previews.py
python3 tools/art/blender/merge_blender_manifests.py
python3 tools/art/check_env_mesh_topology.py || exit 1
python3 tools/art/check_early_packet34_previews.py || exit 1
echo "packet34 blender OK"