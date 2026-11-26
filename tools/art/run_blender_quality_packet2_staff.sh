#!/usr/bin/env bash
# Quality pass packet 2: rebake staff silhouettes + wisp moods (#88 bar).
set -euo pipefail
export EARLY_ART_PREVIEW_RIG=1
ROOT="$(cd "$(dirname "$0")/../.." && pwd)"
cd "$ROOT"
BLENDER="${BLENDER_BIN:-blender}"
mapfile -t SCRIPTS < <(find tools/art/blender -maxdepth 1 -name 'staff-*.py' ! -name 'staff_build.py' | sort)
for script in "${SCRIPTS[@]}"; do
  echo "=== $script ==="
  out=$("$BLENDER" -b --python "$script" 2>&1) || { echo "$out"; echo "FAIL $script" >&2; exit 1; }
  echo "$out"
  echo "$out" | grep -q '^OK staff-' || { echo "FAIL missing OK line: $script" >&2; exit 1; }
done
echo "=== wisp ==="
"$BLENDER" -b --python tools/art/blender/blender_render_hud_wisp.py
python3 tools/art/blender/merge_blender_manifests.py
python3 tools/art/generate_wisp_mascot.py
python3 tools/art/check_staff_mesh_topology.py
python3 tools/art/check_staff_gltf_upright.py
python3 tools/art/check_staff_preview_upright.py
python3 tools/art/check_staff_head_above_feet.py
python3 tools/art/check_preview_no_backdrop_plate.py
python3 tools/art/check_wisp_face_present.py
bash tools/art/check_packet2_previews.sh
python3 tools/art/render_early_composites.py
python3 tools/art/check_composite_frame_fill.py
echo "quality packet2 staff OK"