#!/usr/bin/env bash
# CC0 hero staff lane: full founder + researcher pipelines (incl. rig pivot + walk strips).
set -euo pipefail
ROOT="$(cd "$(dirname "$0")/../.." && pwd)"
cd "$ROOT"

./tools/art/run_blender_cc0_staff_founder.sh
./tools/art/run_blender_cc0_staff_researcher.sh

echo "cc0 staff heroes OK"