#!/usr/bin/env bash
# Fetch Kenney Factory Kit into docs/art/cc0/kenney-factory-kit (CC0, local bake).
set -euo pipefail
ROOT="$(cd "$(dirname "$0")/../../.." && pwd)"
DEST="$ROOT/docs/art/cc0/kenney-factory-kit"
ARCH="$ROOT/docs/art/cc0/.archives"
mkdir -p "$ARCH" "$DEST"
ZIP="$ARCH/kenney_factory-kit_3.0.zip"
if [[ ! -f "$ZIP" ]]; then
  curl -fSL --retry 3 -o "$ZIP" \
    "https://opengameart.org/sites/default/files/kenney_factory-kit_3.0.zip"
fi
rm -rf "$DEST"
mkdir -p "$DEST"
unzip -qo "$ZIP" -d "$DEST"
echo "OK: Kenney Factory Kit at $DEST"