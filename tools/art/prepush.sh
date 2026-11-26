#!/usr/bin/env bash
# Before any push: run this script (or push-ff-only.sh). Force-push is banned outright.
set -euo pipefail
ROOT="$(cd "$(dirname "$0")/../.." && pwd)"
cd "$ROOT"

prepusher_require_ff_ancestor() {
  local remote_ref="${1:-}"
  if [[ -z "$remote_ref" ]]; then
    if remote_ref="$(git rev-parse --abbrev-ref --symbolic-full-name '@{u}' 2>/dev/null)"; then
      :
    else
      remote_ref="${ART_PREPUSH_UPSTREAM:-}"
    fi
  fi
  if [[ -z "$remote_ref" ]]; then
    echo "OK: skip ff-ancestor check (no upstream / ART_PREPUSH_UPSTREAM)"
    return 0
  fi
  if ! git rev-parse --verify "$remote_ref" >/dev/null 2>&1; then
    echo "OK: skip ff-ancestor check (missing ref $remote_ref)"
    return 0
  fi
  if ! git merge-base --is-ancestor "$remote_ref" HEAD; then
    echo "FAIL: $remote_ref is not an ancestor of HEAD — push would need force" >&2
    echo "Never: git push --force, --force-with-lease, or +refspec. Leave remote as-is; fix with forward commits only." >&2
    exit 1
  fi
  echo "OK: fast-forward push allowed ($remote_ref..HEAD)"
}

prepusher_require_ff_ancestor "${ART_PREPUSH_FF_REF:-}"
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
echo "OK: HEAD message (git log -1 --format=%B):"
echo "$HEAD_BODY" | sed '/^$/d' | head -5
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
  while IFS= read -r email; do
  [[ -z "$email" ]] && continue
  if [[ "$email" != *@users.noreply.github.com ]]; then
    echo "FAIL: unpushed commit author email not noreply: $email" >&2
    exit 1
  fi
  done < <(git log "$UPSTREAM"..HEAD --format='%ae')
fi
echo "OK: no Co-authored-by in commit messages"
echo "art prepush OK"