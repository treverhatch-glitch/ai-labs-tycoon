#!/usr/bin/env bash
# Forward commit via commit-tree: treverhatch-glitch noreply only, no trailers.
# Usage: commit-tree-noreply.sh "subject" ["body line 2" ...]
# Index must be staged; does not amend or rewrite pushed SHAs.
set -euo pipefail
ROOT="$(cd "$(dirname "$0")/../.." && pwd)"
cd "$ROOT"
AUTHOR_NAME="${ART_COMMIT_AUTHOR_NAME:-treverhatch-glitch}"
AUTHOR_EMAIL="${ART_COMMIT_AUTHOR_EMAIL:-342607508+treverhatch-glitch@users.noreply.github.com}"
if [[ $# -lt 1 ]]; then
  echo "usage: $0 <subject> [body...]" >&2
  exit 1
fi
if git diff --cached --quiet; then
  echo "FAIL: nothing staged for commit-tree" >&2
  exit 1
fi
SUBJECT=$1
shift
BODY_FILE=$(mktemp)
{
  echo "$SUBJECT"
  if [[ $# -gt 0 ]]; then
    echo
    printf '%s\n' "$@"
  fi
} >"$BODY_FILE"
if grep -qiE 'Co-authored-by:|Signed-off-by:' "$BODY_FILE"; then
  echo "FAIL: forbidden trailer in message" >&2
  rm -f "$BODY_FILE"
  exit 1
fi
TREE=$(git write-tree)
PARENT=""
if git rev-parse --verify HEAD >/dev/null 2>&1; then
  PARENT=$(git rev-parse HEAD)
fi
export GIT_AUTHOR_NAME="$AUTHOR_NAME"
export GIT_AUTHOR_EMAIL="$AUTHOR_EMAIL"
export GIT_COMMITTER_NAME="$AUTHOR_NAME"
export GIT_COMMITTER_EMAIL="$AUTHOR_EMAIL"
if [[ -n "$PARENT" ]]; then
  NEW=$(git commit-tree "$TREE" -p "$PARENT" -F "$BODY_FILE")
else
  NEW=$(git commit-tree "$TREE" -F "$BODY_FILE")
fi
git update-ref HEAD "$NEW"
rm -f "$BODY_FILE"
echo "OK: commit $NEW"
git log -1 --format='%H %an <%ae>'
echo "---"
git log -1 --format=%B