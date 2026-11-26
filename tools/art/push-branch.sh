#!/usr/bin/env bash
# Art lane: prepush + ancestor gate + plain git push (no force).
set -euo pipefail
ROOT="$(cd "$(dirname "$0")/../.." && pwd)"
cd "$ROOT"
BRANCH="${1:-}"
if [[ -z "$BRANCH" ]]; then
  echo "usage: $0 <branch-name>   # e.g. cursor/art-urp-garage-scene" >&2
  exit 1
fi
for arg in "$@"; do
  case "$arg" in
    --force|--force-with-lease)
      echo "FAIL: $arg is banned on art lanes" >&2
      exit 1
      ;;
  esac
  if [[ "$arg" == +* ]]; then
    echo "FAIL: force refspec (+branch) is banned" >&2
    exit 1
  fi
done
REMOTE="origin/$BRANCH"
git fetch origin "$BRANCH" 2>/dev/null || true
export ART_PREPUSH_UPSTREAM="$REMOTE"
./tools/art/prepush.sh
if ! git rev-parse --verify "$REMOTE" >/dev/null 2>&1; then
  echo "OK: first push to new branch $BRANCH"
else
  if ! git merge-base --is-ancestor "$REMOTE" HEAD; then
    echo "FAIL: abort push — $REMOTE is not an ancestor of HEAD" >&2
    exit 1
  fi
fi
exec git push -u origin "$BRANCH"