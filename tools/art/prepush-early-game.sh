#!/usr/bin/env bash
# Early-game art lane: campus checks + garage/staff/hud/wisp generators.
set -euo pipefail
ROOT="$(cd "$(dirname "$0")/../.." && pwd)"
cd "$ROOT"
ART_PREPUSH_UPSTREAM="${ART_PREPUSH_UPSTREAM:-origin/cursor/early-game-art-bootstrap}"
export ART_PREPUSH_UPSTREAM
python3 tools/art/generate_garage_kit.py
python3 tools/art/generate_early_staff.py
python3 tools/art/generate_early_hud_icons.py
python3 tools/art/generate_wisp_mascot.py
python3 tools/art/generate_early_textures.py
./tools/art/prepush.sh
python3 tools/art/check_garage_manifest.py
python3 tools/art/check_early_hud_manifest.py
echo "early-game art prepush OK"