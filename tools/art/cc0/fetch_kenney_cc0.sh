#!/usr/bin/env bash
# Fetch Kenney CC0 kits used by garage URP hero (referer required).
set -euo pipefail
ROOT="$(cd "$(dirname "$0")/../.." && pwd)"
KD="$ROOT/tools/art/cc0/incoming/kenney"
UA="Mozilla/5.0 (compatible; ai-labs-tycoon-art/1.0)"
mkdir -p "$KD"

fetch() {
  local url="$1" ref="$2" out="$3"
  if [[ -f "$out" ]] && file "$out" | grep -q Zip; then
    echo "skip (exists) $(basename "$out")"
    return 0
  fi
  echo "fetch $(basename "$out")"
  curl -fsSL -A "$UA" -e "$ref" -o "$out" "$url"
  file "$out" | grep -q Zip || { echo "FAIL: not a zip: $out" >&2; exit 1; }
}

fetch "https://kenney.nl/media/pages/assets/furniture-kit/440e0608a4-1677580847/kenney_furniture-kit.zip" \
  "https://kenney.nl/assets/furniture-kit" "$KD/kenney_furniture-kit.zip"
fetch "https://kenney.nl/media/pages/assets/building-kit/0de7aaa492-1743244741/kenney_building-kit.zip" \
  "https://kenney.nl/assets/building-kit" "$KD/kenney_building-kit.zip"
fetch "https://kenney.nl/media/pages/assets/city-kit-suburban/2c871b7af2-1745479373/kenney_city-kit-suburban_20.zip" \
  "https://kenney.nl/assets/city-kit-suburban" "$KD/kenney_city-kit-suburban_20.zip"

echo "OK kenney cc0 incoming"