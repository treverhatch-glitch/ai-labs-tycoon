#!/usr/bin/env bash
set -euo pipefail
ROOT="$(cd "$(dirname "$0")/../../.." && pwd)"
cd "$ROOT"
BLENDER="${BLENDER_BIN:-blender}"

if [[ ! -f tools/art/cc0/incoming/polyhaven/studio_small_09_1k.hdr ]]; then
  ./tools/art/cc0/download_packs.sh
fi

python3 tools/art/cc0/build_compute_heroes.py
python3 tools/art/cc0/check_cc0_compute_manifest.py
echo "compute CC0 heroes OK"