#!/usr/bin/env bash
set -euo pipefail
ROOT="$(cd "$(dirname "$0")/../.." && pwd)"
cd "$ROOT"
./tools/art/assert_fast_forward_push.sh
for arg in "${@:-}"; do
  case "$arg" in
    --force | --force-with-lease | +*)
      echo "FAIL: force-push banned (--force / --force-with-lease / +refspec)" >&2
      exit 1
      ;;
  esac
done
PUSH_BRANCH="${ART_PREPUSH_PUSH_BRANCH:-$(git symbolic-ref -q --short HEAD 2>/dev/null || true)}"
if [[ -n "$PUSH_BRANCH" ]]; then
  if git rev-parse --verify "origin/${PUSH_BRANCH}" >/dev/null 2>&1; then
    if ! git merge-base --is-ancestor "origin/${PUSH_BRANCH}" HEAD; then
      echo "FAIL: origin/${PUSH_BRANCH} is not an ancestor of HEAD (forward commits only)" >&2
      exit 1
    fi
    echo "OK: merge-base (origin/${PUSH_BRANCH} ancestor of HEAD)"
  fi
fi
python3 tools/art/generate_campus_kit.py
python3 tools/art/generate_ui_icons.py
python3 tools/art/generate_rival_silhouette_svgs.py
cp -f docs/art/world-bloc-tints.json unity/Assets/Art/Generated/world-bloc-tints.json
cp -f docs/art/era-skin-tokens.json unity/Assets/Art/Generated/era-skin-tokens.json
python3 tools/art/ensure_unity_asset_metas.py
python3 tools/art/check_assets_have_meta.py
python3 tools/art/check_preview_quality.py
python3 tools/art/check_prop_desk_assembly.py
python3 tools/art/check_glb_embedded_images.py
python3 tools/art/validate_unity_meta_guids.py
python3 tools/art/check_catalog_manifest.py
python3 tools/art/check_ui_icons_manifest.py
python3 tools/art/check_silhouette_manifest.py
python3 tools/art/verify_handoff_lists.py
python3 tools/art/check_bloc_tints.py
python3 tools/art/check_era_skin_tokens.py
python3 tools/art/check_prop_server_rack_assembly.py
HEAD_BODY=$(git log -1 --format=%B)
HEAD_EMAIL=$(git log -1 --format='%ae')
if echo "$HEAD_BODY" | grep -qiE 'Co-authored-by:'; then
  echo "FAIL: Co-authored-by in HEAD commit message" >&2
  echo "Fix: amend locally only while unpushed; no rewrite of pushed commits." >&2
  exit 1
fi
if [[ "$HEAD_EMAIL" != *@users.noreply.github.com ]]; then
  echo "FAIL: HEAD author email must be GitHub noreply (got: $HEAD_EMAIL)" >&2
  exit 1
fi
if [[ "$HEAD_EMAIL" == *trevorhatch-glitch@* ]]; then
  echo "FAIL: HEAD author email typo (trevorhatch); use 342607508+treverhatch-glitch@users.noreply.github.com" >&2
  exit 1
fi
echo "OK: HEAD message (git log -1 --format=%B):"
echo "$HEAD_BODY" | sed '/^$/d' | head -5
if ! git diff --cached --quiet 2>/dev/null; then
  if [[ -f .git/COMMIT_EDITMSG ]] && grep -qiE 'Co-authored-by:' .git/COMMIT_EDITMSG; then
    echo "FAIL: Co-authored-by in staged COMMIT_EDITMSG" >&2
    exit 1
  fi
fi
CURRENT_BRANCH="$(git symbolic-ref -q --short HEAD 2>/dev/null || true)"
UPSTREAM="${ART_PREPUSH_UPSTREAM:-origin/${CURRENT_BRANCH:-cursor/graphics-pipeline-5032}}"
if git rev-parse --verify "$UPSTREAM" >/dev/null 2>&1; then
  if git log "$UPSTREAM"..HEAD --format=%B | grep -qiE 'Co-authored-by:'; then
    echo "FAIL: Co-authored-by in commits ahead of $UPSTREAM" >&2
    exit 1
  fi
  while IFS= read -r email; do
  [[ -z "$email" ]] && continue
  if [[ "$email" != *@users.noreply.github.com ]]; then
    echo "FAIL: unpushed commit author email not noreply: $email" >&2
    exit 1
  fi
    if [[ "$email" == *trevorhatch-glitch@* ]]; then
      echo "FAIL: unpushed author email typo (trevorhatch); use treverhatch noreply" >&2
      exit 1
    fi
  done < <(git log "$UPSTREAM"..HEAD --format='%ae')
fi
echo "OK: no Co-authored-by in commit messages"
echo "art prepush OK"