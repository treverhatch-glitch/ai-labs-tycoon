#!/usr/bin/env bash
set -euo pipefail
ROOT="$(cd "$(dirname "$0")/../.." && pwd)"
cd "$ROOT"
python3 tools/art/generate_campus_kit.py
python3 tools/art/generate_ui_icons.py
python3 tools/art/validate_unity_meta_guids.py
python3 tools/art/check_catalog_manifest.py
echo "art prepush OK"