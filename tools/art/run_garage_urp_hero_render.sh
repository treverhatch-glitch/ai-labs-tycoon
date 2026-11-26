#!/usr/bin/env bash
# Blender fallback hero when Unity batchmode is unavailable.
set -euo pipefail
ROOT="$(cd "$(dirname "$0")/../.." && pwd)"
cd "$ROOT"
./tools/art/cc0/fetch_polyhaven_hdri.sh
chmod +x ./tools/art/cc0/fetch_kenney_cc0.sh 2>/dev/null || true
./tools/art/cc0/fetch_kenney_cc0.sh
BLENDER="${BLENDER:-blender}"
if ! command -v "$BLENDER" >/dev/null 2>&1; then
  for cand in /tmp/blender-4.0.2-linux-x64/blender /opt/blender/blender; do
    if [[ -x "$cand" ]]; then
      BLENDER="$cand"
      break
    fi
  done
fi
if ! command -v "$BLENDER" >/dev/null 2>&1 && [[ ! -x "$BLENDER" ]]; then
  echo "FAIL: blender not found (set BLENDER=)" >&2
  exit 1
fi
"$BLENDER" -b --factory-startup -noaudio --python "$ROOT/tools/art/blender/garage-scene-hero-compose.py"
test -f docs/art/previews/garage-scene-urp-hero.png
echo "OK garage URP hero render"