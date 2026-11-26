#!/usr/bin/env bash
set -euo pipefail
ROOT="$(cd "$(dirname "$0")/../.." && pwd)"
cd "$ROOT"

# Force-push is banned (--force, --force-with-lease, +refspec). Fix mistakes with a forward commit only.
REMOTE="${ART_PREPUSH_REMOTE:-origin}"
BRANCH="${ART_PREPUSH_BRANCH:-$(git symbolic-ref -q --short HEAD 2>/dev/null || true)}"
if [[ -n "$BRANCH" && "$BRANCH" != "HEAD" ]]; then
  git fetch "$REMOTE" "$BRANCH" 2>/dev/null || git fetch "$REMOTE" 2>/dev/null || true
  UPREF="$REMOTE/$BRANCH"
  if git rev-parse --verify "$UPREF" >/dev/null 2>&1; then
    if ! git merge-base --is-ancestor "$UPREF" HEAD; then
      echo "FAIL: $UPREF is not an ancestor of HEAD" >&2
      echo "Force-push is banned. Leave remote as-is; add a forward commit on top." >&2
      exit 1
    fi
    echo "OK: fast-forward push allowed ($UPREF -> HEAD)"
  else
    echo "WARN: $UPREF not found after fetch; push may create branch (no force)"
  fi
fi
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