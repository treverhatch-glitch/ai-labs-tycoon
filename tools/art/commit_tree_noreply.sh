#!/usr/bin/env bash
# commit-tree with correct Hatch noreply (treverhatch-glitch — not trevor).
set -euo pipefail
ROOT="$(cd "$(dirname "$0")/../.." && pwd)"
cd "$ROOT"
if ! git diff --cached --quiet 2>/dev/null; then
  TREE=$(git write-tree)
else
  echo "FAIL: no staged changes for commit-tree (git add first)" >&2
  exit 1
fi
PARENT="${1:-$(git rev-parse HEAD)}"
shift || true
export GIT_AUTHOR_NAME="treverhatch-glitch"
export GIT_COMMITTER_NAME="treverhatch-glitch"
export GIT_AUTHOR_EMAIL="342607508+treverhatch-glitch@users.noreply.github.com"
export GIT_COMMITTER_EMAIL="342607508+treverhatch-glitch@users.noreply.github.com"
if [[ $# -eq 0 ]]; then
  set -- "chore: commit-tree"
fi
ARGS=()
for line in "$@"; do
  ARGS+=( -m "$line" )
done
NEW=$(git commit-tree "$TREE" -p "$PARENT" "${ARGS[@]}")
git reset --hard "$NEW"
echo "$NEW"