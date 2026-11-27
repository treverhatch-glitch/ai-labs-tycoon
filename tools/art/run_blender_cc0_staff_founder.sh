#!/usr/bin/env bash
# Founder hero only — pause lane (researcher unchanged).
set -euo pipefail
export EARLY_ART_PREVIEW_RIG=1
export STAFF_PREVIEW_YAW_DEG=32
export STAFF_PREVIEW_ELEV_DEG=8
export STAFF_PREVIEW_MARGIN=1.52
ROOT="$(cd "$(dirname "$0")/../.." && pwd)"
cd "$ROOT"
BLENDER="${BLENDER_BIN:-blender}"

if [[ ! -f tools/art/cc0/incoming/kenney/animated-characters-3/Model/characterMedium.fbx ]]; then
  ./tools/art/cc0/fetch_incoming.sh
fi

echo "=== tools/art/blender/render_kenney_founder_stock_check.py ==="
stock_out=$("$BLENDER" -b --python tools/art/blender/render_kenney_founder_stock_check.py 2>&1) || { echo "$stock_out"; echo "FAIL stock check" >&2; exit 1; }
echo "$stock_out"
echo "$stock_out" | grep -q '^OK stock_check' || { echo "FAIL stock check OK line" >&2; exit 1; }
test -s "docs/art/previews/staff-founder-kenney-stock-check.png" || { echo "FAIL missing stock check png"; exit 1; }

script=tools/art/blender/staff-founder-idle.py
echo "=== $script ==="
out=$("$BLENDER" -b --python "$script" 2>&1) || { echo "$out"; echo "FAIL $script" >&2; exit 1; }
echo "$out"
echo "$out" | grep -qE '^OK staff-founder-idle [0-9]+ ' || { echo "FAIL missing bake OK line: $script" >&2; exit 1; }

set +e
out=$("$BLENDER" -b --python tools/art/blender/check_staff_gltf_blender.py -- "$ROOT" 2>&1)
set -e
echo "$out" | grep -E '^OK staff-founder-idle '
echo "$out" | grep -q '^OK staff-founder-idle ' || { echo "FAIL staff-founder-idle gltf gate"; exit 1; }

test -s "docs/art/previews/staff-founder-idle.png" || { echo "FAIL missing founder preview"; exit 1; }

python3 tools/art/check_cc0_staff_founder_regression.py
python3 tools/art/check_cc0_staff_founder_preview_upright.py
python3 tools/art/check_cc0_staff_founder_preview_silhouette.py
python3 tools/art/check_cc0_staff_founder_face_read.py
python3 tools/art/check_cc0_staff_founder_torso_hue.py
python3 tools/art/check_cc0_staff_founder_arm_symmetry.py
python3 tools/art/check_cc0_staff_founder_framing_width.py
python3 tools/art/check_cc0_staff_founder_head_highlight.py
python3 tools/art/check_cc0_staff_founder_floor_tile.py
python3 tools/art/check_cc0_staff_founder_preview_orphans.py
python3 tools/art/check_cc0_staff_founder_standing.py
python3 tools/art/check_cc0_staff_founder_screen_up.py
python3 tools/art/check_cc0_staff_material_diversity.py
python3 tools/art/check_cc0_staff_silhouette.py
python3 tools/art/check_cc0_staff_floor_loose.py
python3 tools/art/check_cc0_staff_preview_front.py
python3 tools/art/check_cc0_staff_preview_corner_grey.py
python3 tools/art/check_cc0_staff_founder_pose.py
python3 tools/art/check_cc0_staff_founder_arm_pose.py
python3 tools/art/check_cc0_staff_face_visible.py
python3 tools/art/check_cc0_staff_founder_eye_blobs.py
python3 tools/art/check_cc0_staff_founder_preview_eyes.py
python3 tools/art/check_cc0_staff_founder_preview_centered.py
python3 tools/art/check_cc0_staff_founder_glb_connected.py
python3 tools/art/check_cc0_staff_founder_pixel_hues.py
python3 tools/art/check_cc0_staff_founder_assembly.py
python3 tools/art/check_cc0_staff_founder_head_mesh.py
python3 tools/art/check_cc0_staff_founder_chunky_bake.py

set +e
conn_out=$(python3 tools/art/check_staff_connectivity.py 2>&1)
set -e
echo "$conn_out" | grep -E '^OK staff-founder-idle' || true
echo "$conn_out" | grep -q '^OK staff-founder-idle' || { echo "FAIL connectivity founder"; exit 1; }

echo "cc0 staff founder OK"