#!/usr/bin/env bash
# CC0 garage hero: fetch Poly Haven packs, restyle in Blender, previews + gates.
set -euo pipefail
export EARLY_ART_PREVIEW_RIG=1
export EARLY_ART_HDRI="${EARLY_ART_HDRI:-1}"
export GARAGE_CC0_IMPORT=1
ROOT="$(cd "$(dirname "$0")/../.." && pwd)"
cd "$ROOT"
BLENDER="${BLENDER_BIN:-blender}"

python3 tools/art/cc0/fetch_polyhaven_garage.py

echo "=== cc0 garage hero (all props) ==="
"$BLENDER" -b --python-exit-code 1 --python tools/art/blender/cc0_garage_asset.py -- ALL

echo "=== garage-scene-preview ==="
"$BLENDER" -b --python-exit-code 1 --python tools/art/blender/garage-scene-preview.py

python3 tools/art/check_garage_quality_previews.py
python3 tools/art/check_garage_mesh_topology.py
python3 tools/art/check_garage_asset_complexity.py
python3 tools/art/check_garage_baked_albedo.py
python3 tools/art/check_garage_silhouette.py
echo "cc0 garage hero OK"