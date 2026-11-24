#!/usr/bin/env bash
set -euo pipefail
ROOT="$(cd "$(dirname "$0")/../.." && pwd)"
cd "$ROOT"
python3 tools/art/generate_campus_kit.py
python3 tools/art/generate_ui_icons.py
python3 tools/art/generate_rival_silhouette_svgs.py
cp -f docs/art/world-bloc-tints.json unity/Assets/Art/Generated/world-bloc-tints.json
cp -f docs/art/era-skin-tokens.json unity/Assets/Art/Generated/era-skin-tokens.json
python3 tools/art/validate_unity_meta_guids.py
python3 tools/art/check_catalog_manifest.py
python3 tools/art/check_ui_icons_manifest.py
python3 tools/art/check_silhouette_manifest.py
python3 tools/art/verify_handoff_lists.py
python3 tools/art/check_bloc_tints.py
python3 tools/art/check_era_skin_tokens.py
if git log -1 --format=%B | grep -qiE 'Co-authored-by:'; then
  echo "FAIL: Co-authored-by in HEAD commit message" >&2
  exit 1
fi
if ! git diff --cached --quiet 2>/dev/null; then
  if [[ -f .git/COMMIT_EDITMSG ]] && grep -qiE 'Co-authored-by:' .git/COMMIT_EDITMSG; then
    echo "FAIL: Co-authored-by in staged COMMIT_EDITMSG" >&2
    exit 1
  fi
fi
UPSTREAM="${ART_PREPUSH_UPSTREAM:-origin/cursor/graphics-pipeline-5032}"
if git rev-parse --verify "$UPSTREAM" >/dev/null 2>&1; then
  if git log "$UPSTREAM"..HEAD --format=%B | grep -qiE 'Co-authored-by:'; then
    echo "FAIL: Co-authored-by in commits ahead of $UPSTREAM" >&2
    exit 1
  fi
fi
echo "OK: no Co-authored-by in commit messages"
echo "art prepush OK"