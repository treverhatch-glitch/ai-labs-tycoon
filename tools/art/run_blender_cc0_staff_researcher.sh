#!/usr/bin/env bash
# Researcher Quaternius modular hero — same preview bar as founder #98 (researcher assets only).
set -euo pipefail
export EARLY_ART_PREVIEW_RIG=1
export STAFF_PREVIEW_YAW_DEG=30
export STAFF_PREVIEW_ELEV_DEG=10
export STAFF_PREVIEW_MARGIN=1.44
unset RESEARCHER_STOCK_RECOLOR
ROOT="$(cd "$(dirname "$0")/../.." && pwd)"
cd "$ROOT"
BLENDER="${BLENDER_BIN:-blender}"

if [[ ! -f tools/art/cc0/incoming/quaternius/character.glb ]]; then
  ./tools/art/cc0/fetch_incoming.sh
fi
if [[ ! -f tools/art/cc0/incoming/kenney/animated-characters-3/Model/characterMedium.fbx ]]; then
  ./tools/art/cc0/fetch_incoming.sh
fi

python3 tools/art/check_cc0_staff_founder_stock_kit_glb.py
test -s "docs/art/previews/staff-founder-kenney-stock-check.png" || {
  echo "FAIL missing stock check png (founder #98 frozen — do not rebake on researcher lane)" >&2
  exit 1
}

echo "=== tools/art/blender/render_kenney_researcher_face_diagnostic.py ==="
diag_out=$("$BLENDER" -b --python tools/art/blender/render_kenney_researcher_face_diagnostic.py 2>&1) || { echo "$diag_out"; echo "FAIL face diagnostic" >&2; exit 1; }
echo "$diag_out"
echo "$diag_out" | grep -q '^OK face_diagnostic' || { echo "FAIL face diagnostic OK line" >&2; exit 1; }
test -s "docs/art/previews/staff-researcher-face-diagnostic-0deg.png" || { echo "FAIL missing face diagnostic png"; exit 1; }
python3 tools/art/check_cc0_staff_researcher_diagnostic_eyes.py

echo "=== tools/art/blender/render_kenney_researcher_axis_sheet.py ==="
axis_out=$("$BLENDER" -b --python tools/art/blender/render_kenney_researcher_axis_sheet.py 2>&1) || { echo "$axis_out"; echo "FAIL axis sheet" >&2; exit 1; }
echo "$axis_out"
echo "$axis_out" | grep -q '^OK axis_sheet' || { echo "FAIL axis sheet OK line" >&2; exit 1; }
test -s "docs/art/previews/staff-researcher-axis-sheet.png" || { echo "FAIL missing axis sheet png"; exit 1; }

script=tools/art/blender/staff-researcher-idle.py
echo "=== $script ==="
out=$("$BLENDER" -b --python "$script" 2>&1) || { echo "$out"; echo "FAIL $script" >&2; exit 1; }
echo "$out"
echo "$out" | grep -qE '^OK staff-researcher-idle [0-9]+ ' || { echo "FAIL missing bake OK line: $script" >&2; exit 1; }

set +e
out=$("$BLENDER" -b --python tools/art/blender/check_staff_gltf_blender.py -- "$ROOT" 2>&1)
set -e
echo "$out" | grep -E '^OK staff-researcher-idle '
echo "$out" | grep -q '^OK staff-researcher-idle ' || { echo "FAIL staff-researcher-idle gltf gate"; exit 1; }

python3 tools/art/check_cc0_staff_researcher_eye_glb_world.py
test -s "docs/art/previews/staff-researcher-idle.png" || { echo "FAIL missing researcher preview"; exit 1; }

python3 tools/art/check_cc0_staff_preview_upright.py
python3 tools/art/check_cc0_staff_silhouette.py
python3 tools/art/check_cc0_staff_material_diversity.py
python3 tools/art/check_cc0_staff_floor_loose.py
python3 tools/art/check_cc0_staff_preview_front.py
python3 tools/art/check_cc0_staff_preview_corner_grey.py
python3 tools/art/compose_researcher_stock_preview.py
test -s "docs/art/previews/staff-researcher-vs-stock-check.png" || { echo "FAIL missing vs-stock png"; exit 1; }

python3 tools/art/check_cc0_staff_researcher_eye_blobs.py
python3 tools/art/check_cc0_staff_researcher_eyes_in_head.py
python3 tools/art/check_cc0_staff_researcher_preview_clip.py
python3 tools/art/check_cc0_staff_researcher_idle_visual.py
python3 tools/art/check_cc0_staff_researcher_preview_eyes.py
python3 tools/art/check_cc0_staff_researcher_eye_project.py

set +e
conn_out=$(python3 tools/art/check_staff_connectivity.py 2>&1)
set -e
echo "$conn_out" | grep -E '^OK staff-researcher-idle' || { echo "$conn_out"; exit 1; }

echo "OK cc0 staff researcher hero lane"