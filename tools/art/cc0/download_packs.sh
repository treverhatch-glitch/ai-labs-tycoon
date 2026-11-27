#!/usr/bin/env bash
# CC0 source packs for art lanes — real curl downloads into tools/art/cc0/incoming/.
set -euo pipefail
ROOT="$(cd "$(dirname "$0")/../../.." && pwd)"
IN="$ROOT/tools/art/cc0/incoming"
ARCH="$IN/archives"
mkdir -p "$ARCH"

curl -fSL --retry 3 -o "$ARCH/kenney_furniturePack.zip" \
  "https://opengameart.org/sites/default/files/kenney_furniturePack.zip"
curl -fSL --retry 3 -o "$ARCH/kenney_factory-kit_3.0.zip" \
  "https://opengameart.org/sites/default/files/kenney_factory-kit_3.0.zip"
curl -fSL --retry 3 -o "$ARCH/modular_scifi_megakitstandard.zip" \
  "https://opengameart.org/sites/default/files/modular_scifi_megakitstandard.zip"
curl -fSL --retry 3 -o "$ARCH/studio_small_09_1k.hdr" \
  "https://dl.polyhaven.org/file/ph-assets/HDRIs/hdr/1k/studio_small_09_1k.hdr"
curl -fSL --retry 3 -o "$ARCH/Metal048A_1K-JPG.zip" \
  "https://ambientcg.com/get?file=Metal048A_1K-JPG.zip"

rm -rf "$IN/kenney-furniture-kit" "$IN/kenney-factory-kit" "$IN/quaternius-scifi-megakit"
rm -rf "$IN/polyhaven" "$IN/ambientcg"
mkdir -p "$IN/kenney-furniture-kit" "$IN/kenney-factory-kit" "$IN/quaternius-scifi-megakit" \
  "$IN/polyhaven" "$IN/ambientcg"

unzip -qo "$ARCH/kenney_furniturePack.zip" -d "$IN/kenney-furniture-kit"
unzip -qo "$ARCH/kenney_factory-kit_3.0.zip" -d "$IN/kenney-factory-kit"
unzip -qo "$ARCH/modular_scifi_megakitstandard.zip" -d "$IN/quaternius-scifi-megakit"
unzip -qo "$ARCH/Metal048A_1K-JPG.zip" -d "$IN/ambientcg"
cp -f "$ARCH/studio_small_09_1k.hdr" "$IN/polyhaven/studio_small_09_1k.hdr"

python3 "$ROOT/tools/art/cc0/refresh_packs_manifest.py"
echo "CC0 packs ready under $IN"