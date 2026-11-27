#!/usr/bin/env bash
set -euo pipefail
ROOT="$(cd "$(dirname "$0")/../../.." && pwd)"
cd "$ROOT"
export ART_PREPUSH_BRANCH="${ART_PREPUSH_BRANCH:-cursor/art-compute-hardware-cc0}"
./tools/art/assert_fast_forward_push.sh
UPSTREAM="origin/${ART_PREPUSH_BRANCH}"
if git rev-parse --verify "$UPSTREAM" >/dev/null 2>&1; then
  PARENTS=$(git log -1 --format=%P | wc -w | tr -d ' ')
  if [[ "$PARENTS" -ge 2 ]]; then
    BODY=$(git log -1 --format=%B)
  else
    BODY=$(git log "$UPSTREAM"..HEAD --format=%B)
  fi
  if echo "$BODY" | grep -qiE 'Co-authored-by:'; then
    echo "FAIL: Co-authored-by in commits to push" >&2
    exit 1
  fi
fi
if [[ -n "$(git ls-files '*.pyc')" ]]; then
  echo "FAIL: tracked .pyc files in index" >&2
  exit 1
fi
python3 tools/art/cc0/check_cc0_compute_manifest.py
echo "compute CC0 prepush OK"