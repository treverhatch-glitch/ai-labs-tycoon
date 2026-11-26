#!/usr/bin/env bash
# Fetch CC0 rigged humanoids + preview HDRI into tools/art/cc0/incoming/
set -euo pipefail
ROOT="$(cd "$(dirname "$0")/../../.." && pwd)"
IN="$ROOT/tools/art/cc0/incoming"
mkdir -p "$IN/kenney" "$IN/quaternius" "$IN/polyhaven"

curl -fsSL -A "ai-labs-tycoon-art/1.0" -o "$IN/kenney/animated-characters-3.zip" \
  "https://kenney.nl/media/pages/assets/animated-characters-3/8bced0719e-1670690055/animated-characters-3.zip"
unzip -q -o "$IN/kenney/animated-characters-3.zip" -d "$IN/kenney/animated-characters-3"

curl -fsSL -o "$IN/quaternius/animated_human.glb" \
  "https://github.com/Papyszoo/CC0-Public-Domain-Models/raw/main/packs/quaternius-animatedman/models/animated_human/animated_human.glb"
curl -fsSL -o "$IN/quaternius/character.glb" \
  "https://github.com/profesorshader/shader-waving-cloth/raw/main/quaternius/character.glb"

curl -fsSL -o "$IN/polyhaven/studio_small_08_2k.hdr" \
  "https://dl.polyhaven.org/file/ph-assets/HDRIs/hdr/2k/studio_small_08_2k.hdr"

python3 "$ROOT/tools/art/cc0/write_manifest.py"
echo "cc0 incoming fetch OK"