#!/usr/bin/env bash
# Quality pass packet 1: rebake + studio previews for 15 garage props.
set -euo pipefail
export EARLY_ART_PREVIEW_RIG=1
ROOT="$(cd "$(dirname "$0")/../.." && pwd)"
cd "$ROOT"
BLENDER="${BLENDER_BIN:-blender}"
mapfile -t SCRIPTS < <(find tools/art/blender -maxdepth 1 -name 'garage-*.py' ! -name 'garage-scene-preview.py' | sort)
for script in "${SCRIPTS[@]}"; do
  echo "=== $script ==="
  "$BLENDER" -b --python-exit-code 1 --python "$script" || { echo "FAIL $script" >&2; exit 1; }
done
python3 tools/art/blender/merge_blender_manifests.py
for tex in tools/art/blender/tex-garage-trim-orange.py tools/art/blender/tex-garage-wall-beige.py; do
  echo "=== $tex ==="
  "$BLENDER" -b --python-exit-code 1 --python "$tex" || { echo "FAIL $tex" >&2; exit 1; }
done
echo "=== garage-scene-preview ==="
"$BLENDER" -b --python-exit-code 1 --python tools/art/blender/garage-scene-preview.py
python3 tools/art/check_garage_quality_previews.py
python3 tools/art/check_garage_preview_assembly.py
python3 tools/art/check_garage_mesh_topology.py
python3 tools/art/check_garage_asset_complexity.py
python3 tools/art/check_garage_baked_albedo.py
python3 tools/art/check_garage_silhouette.py
python3 tools/art/check_garage_part_bbox_gap.py
python3 tools/art/check_garage_hero_palette.py
echo "quality packet1 garage OK"