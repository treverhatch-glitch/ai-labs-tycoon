#!/usr/bin/env bash
# ambientCG CC0 shell textures for garage hero (concrete + brick).
set -euo pipefail
ROOT="$(cd "$(dirname "$0")/../../.." && pwd)"
DEST="$ROOT/tools/art/cc0/incoming/ambientcg"
mkdir -p "$DEST"
fetch() {
  local zip="$1"
  local url="https://ambientcg.com/get?file=${zip}"
  local out="$DEST/$zip"
  if [[ -f "$out" && -s "$out" ]]; then
    echo "skip (exists) $zip"
  else
    curl -fsSL "$url" -o "$out"
    echo "OK fetch $zip"
  fi
  unzip -qo -o "$out" -d "$DEST/${zip%.zip}" 2>/dev/null || true
}
fetch "Concrete047A_1K-JPG.zip"
fetch "Bricks085_1K-JPG.zip"
echo "OK ambientcg shell incoming"