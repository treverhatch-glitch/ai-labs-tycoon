#!/usr/bin/env bash
# CC0 hero staff lane: founder + researcher, then pause.
set -euo pipefail
export EARLY_ART_PREVIEW_RIG=1
ROOT="$(cd "$(dirname "$0")/../.." && pwd)"
cd "$ROOT"
BLENDER="${BLENDER_BIN:-blender}"

if [[ ! -f tools/art/cc0/incoming/quaternius/character.glb ]]; then
  ./tools/art/cc0/fetch_incoming.sh
fi

for script in tools/art/blender/staff-founder-idle.py tools/art/blender/staff-researcher-idle.py; do
  echo "=== $script ==="
  out=$("$BLENDER" -b --python "$script" 2>&1) || { echo "$out"; echo "FAIL $script" >&2; exit 1; }
  echo "$out"
  echo "$out" | grep -q '^OK staff-' || { echo "FAIL missing OK line: $script" >&2; exit 1; }
done

set +e
out=$("$BLENDER" -b --python tools/art/blender/check_staff_gltf_blender.py -- "$ROOT" 2>&1)
set -e
echo "$out" | grep -E '^(OK|FAIL) staff-(founder|researcher)-idle '
echo "$out" | grep -q '^OK staff-founder-idle ' || { echo "FAIL staff-founder-idle gltf gate"; exit 1; }
echo "$out" | grep -q '^OK staff-researcher-idle ' || { echo "FAIL staff-researcher-idle gltf gate"; exit 1; }
for aid in staff-founder-idle staff-researcher-idle; do
  test -s "docs/art/previews/${aid}.png" || { echo "FAIL missing preview $aid"; exit 1; }
done
python3 tools/art/check_cc0_staff_preview_upright.py
python3 tools/art/check_cc0_staff_material_diversity.py
python3 tools/art/check_cc0_staff_silhouette.py
python3 tools/art/check_cc0_staff_floor_loose.py
set +e
conn_out=$(python3 tools/art/check_staff_connectivity.py 2>&1)
set -e
echo "$conn_out" | grep -E '^(OK|FAIL) staff-(founder|researcher)-idle' || true
echo "$conn_out" | grep -q '^OK staff-founder-idle' || { echo "FAIL connectivity founder"; exit 1; }
echo "$conn_out" | grep -q '^OK staff-researcher-idle' || { echo "FAIL connectivity researcher"; exit 1; }
echo "cc0 staff heroes OK"