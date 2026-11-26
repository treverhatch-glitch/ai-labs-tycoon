#!/usr/bin/env bash
set -euo pipefail
ROOT="$(cd "$(dirname "$0")/../.." && pwd)"
cd "$ROOT"

# Hatch: force-push is banned (--force, --force-with-lease, +refspec). Run before every push.
# If this fails, leave remote as-is and add forward commits only.
PUSH_BRANCH="${ART_PREPUSH_BRANCH:-$(git rev-parse --abbrev-ref HEAD)}"
if [[ -n "$PUSH_BRANCH" && "$PUSH_BRANCH" != "HEAD" ]]; then
  git fetch origin "$PUSH_BRANCH" 2>/dev/null || true
  REMOTE_REF="origin/${PUSH_BRANCH}"
  if git rev-parse --verify "$REMOTE_REF" >/dev/null 2>&1; then
    if ! git merge-base --is-ancestor "$REMOTE_REF" HEAD; then
      echo "FAIL: ${REMOTE_REF} is not an ancestor of HEAD" >&2
      echo "No force-push. Fix with forward commits only; do not rewrite remote." >&2
      exit 1
    fi
    echo "OK: ${REMOTE_REF} is ancestor of HEAD (plain push allowed)"
  else
    echo "WARN: ${REMOTE_REF} not found after fetch; set ART_PREPUSH_BRANCH or fetch before push" >&2
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