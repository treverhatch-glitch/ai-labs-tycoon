#!/usr/bin/env bash
# Fetch CC0 HDRIs listed in tools/art/cc0/manifest.json into incoming/.
set -euo pipefail
ROOT="$(cd "$(dirname "$0")/../.." && pwd)"
INCOMING="$ROOT/tools/art/cc0/incoming"
mkdir -p "$INCOMING"

fetch() {
  local name="$1"
  local url="$2"
  local dest="$INCOMING/$name"
  if [[ -f "$dest" ]]; then
    echo "skip (exists) $name"
    return 0
  fi
  echo "fetch $name"
  curl -fsSL -o "$dest" "$url"
}

fetch "brown_photostudio_02_1k.exr" \
  "https://dl.polyhaven.org/file/ph-assets/HDRIs/exr/1k/brown_photostudio_02_1k.exr"

echo "OK cc0 incoming"