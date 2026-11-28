#!/usr/bin/env bash
# Push current branch to origin after ff-ancestor + prepush gates. Force-push is banned.
set -euo pipefail
ROOT="$(cd "$(dirname "$0")/../.." && pwd)"
cd "$ROOT"

for arg in "$@"; do
  case "$arg" in
    --force|--force-with-lease|-f)
      echo "FAIL: force-push is banned (--force / --force-with-lease / +refspec)" >&2
      exit 1
      ;;
  esac
  if [[ "$arg" == +* ]]; then
    echo "FAIL: force-push is banned (+refspec)" >&2
    exit 1
  fi
done

BRANCH="${ART_PUSH_BRANCH:-$(git branch --show-current)}"
REMOTE="${ART_PUSH_REMOTE:-origin}"
REMOTE_REF="$REMOTE/$BRANCH"

if git rev-parse --verify "$REMOTE_REF" >/dev/null 2>&1; then
  if ! git merge-base --is-ancestor "$REMOTE_REF" HEAD; then
    echo "FAIL: $REMOTE_REF is not an ancestor of HEAD — aborting push" >&2
    echo "Leave remote as-is; add forward commits only. No force-push." >&2
    exit 1
  fi
  echo "OK: merge-base --is-ancestor $REMOTE_REF HEAD"
else
  echo "OK: new branch push ($REMOTE_REF does not exist yet)"
fi

if [[ "${SKIP_ART_PREPUSH:-}" != "1" ]]; then
  ART_PREPUSH_FF_REF="$REMOTE_REF" ./tools/art/prepush.sh
fi

git push -u "$REMOTE" "$BRANCH"
echo "OK: pushed $REMOTE/$BRANCH (fast-forward only)"
echo "--- push report: git log -1 --format=%B ---"
git log -1 --format=%B