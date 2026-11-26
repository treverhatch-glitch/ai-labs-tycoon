#!/usr/bin/env bash
# Re-fetch CC0 assets for packet34 hero lane (checksums in incoming/SHA256SUMS.txt).
set -euo pipefail
ROOT="$(cd "$(dirname "$0")" && pwd)"
IN="$ROOT/incoming"
HDR="$ROOT/hdri"
mkdir -p "$IN" "$HDR"
KENNEY_URL='https://opengameart.org/sites/default/files/kenney_city-kit-suburban_2.0.zip'
HDRI_URL='https://dl.polyhaven.org/file/ph-assets/HDRIs/hdr/2k/studio_small_09_2k.hdr'
ZIP="$IN/kenney_city-kit-suburban_2.0.zip"
HDR_FILE="$HDR/studio_small_09_2k.hdr"
echo "Fetching Kenney City Kit (Suburban)..."
curl -fsSL -o "$ZIP" "$KENNEY_URL"
echo "Fetching Poly Haven studio_small_09 2k HDR..."
curl -fsSL -o "$HDR_FILE" "$HDRI_URL"
(
  cd "$IN"
  rm -rf kenney-city-kit-suburban
  unzip -q -o kenney_city-kit-suburban_2.0.zip -d kenney-city-kit-suburban
)
sha256sum "$ZIP" "$HDR_FILE" > "$IN/SHA256SUMS.txt"
echo "OK: $ZIP"
echo "OK: $HDR_FILE"