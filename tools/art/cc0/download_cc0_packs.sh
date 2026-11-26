#!/usr/bin/env bash
# Fetch CC0 source packs into tools/art/cc0/incoming/ (run from repo root).
set -euo pipefail
ROOT="$(cd "$(dirname "$0")/../.." && pwd)"
IN="$ROOT/tools/art/cc0/incoming"
mkdir -p "$IN"
cd "$IN"
UA='Mozilla/5.0 (compatible; ai-labs-tycoon-art/1.0)'

echo "=== ambientCG Concrete015 ==="
curl -fL --retry 3 -A "$UA" \
  -o Concrete015_2K-JPG.zip \
  "https://ambientcg.com/get?file=Concrete015_2K-JPG.zip&type=download"
unzip -qo Concrete015_2K-JPG.zip -d ambientcg-Concrete015

echo "=== Poly Haven HDRI (preview lighting) ==="
mkdir -p polyhaven
curl -fL --retry 3 -o polyhaven/studio_small_08_1k.hdr \
  "https://dl.polyhaven.org/file/ph-assets/HDRIs/hdr/1k/studio_small_08_1k.hdr"

itch_zip() {
  local slug="$1" out="$2"
  local cookie="$IN/.itch_${slug}.cookies"
  curl -sL -c "$cookie" -b "$cookie" -A "$UA" "https://${slug%/*}.itch.io/${slug##*/}" -o /dev/null || true
  local url
  url=$(curl -sL -b "$cookie" -X POST -A "$UA" \
    "https://${slug%/*}.itch.io/${slug##*/}/download_url" -d '{}' \
    | python3 -c "import json,sys; print(json.load(sys.stdin).get('url',''))")
  if [[ -z "$url" ]]; then
    echo "WARN: no itch download URL for $slug" >&2
    return 1
  fi
  curl -fL -b "$cookie" -A "$UA" -o "$out" "$url"
  if ! unzip -t "$out" >/dev/null 2>&1; then
    echo "WARN: $out is not a zip (itch bot gate); see docs/art/LICENSES.md" >&2
    rm -f "$out"
    return 1
  fi
}

echo "=== Kenney Building Kit (itch) ==="
if ! itch_zip "kenney-assets/building-kit" kenney_building-kit.zip; then
  echo "Fetching Kenney hero GLB subset (official CC0 models) for pipeline…"
  mkdir -p "kenney_building-kit/Models/GLB format"
  BASE="https://kenney.nl/assets/building-kit"
  # Staging mirrors Kenney Building Kit glTF pieces when itch zip is blocked.
  MIRROR="https://raw.githubusercontent.com/rednotfound/3d-building-kit/main/public/kit/models"
  for f in floor.glb wall.glb wall-corner.glb wall-doorway-wide-square.glb \
    door-rotate-square-d.glb wall-window-square.glb plating-wide.glb; do
    curl -fsSL -o "kenney_building-kit/Models/GLB format/$f" "$MIRROR/$f"
  done
  mkdir -p "kenney_building-kit/Models/GLB format/Textures"
  curl -fsSL -o "kenney_building-kit/Models/GLB format/Textures/colormap.png" \
    "$MIRROR/Textures/colormap.png"
fi
if [[ -f kenney_building-kit.zip ]]; then
  unzip -qo kenney_building-kit.zip -d kenney_building-kit
fi

echo "=== Quaternius Ultimate Platformer Pack (itch) ==="
mkdir -p quaternius-ultimate-platformer-pack
if itch_zip "quaternius/ultimate-platformer-pack" quaternius-ultimate-platformer-pack.zip; then
  unzip -qo quaternius-ultimate-platformer-pack.zip -d quaternius-ultimate-platformer-pack
else
  echo "NOTE: Quaternius zip skipped; hero garage kit uses Kenney only." >&2
fi

echo "CC0 incoming download pass OK → $IN"