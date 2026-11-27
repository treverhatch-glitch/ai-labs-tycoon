#!/usr/bin/env bash
# Abort push prep unless origin/<branch> is an ancestor of HEAD (forward-only).
# Force-push (--force, --force-with-lease, +refspec) is banned. If remote is wrong,
# leave it as-is and fix with a new forward commit — never rewrite published history.
set -euo pipefail
ROOT="$(cd "$(dirname "$0")/../.." && pwd)"
cd "$ROOT"
BRANCH="${ART_PREPUSH_BRANCH:-$(git branch --show-current)}"
REMOTE_REF="${ART_PREPUSH_REMOTE:-origin/${BRANCH}}"
if ! git rev-parse --verify "$REMOTE_REF" >/dev/null 2>&1; then
  echo "OK: no local ref for $REMOTE_REF (first push — use plain git push -u origin $BRANCH, never --force)" >&2
  exit 0
fi
if ! git merge-base --is-ancestor "$REMOTE_REF" HEAD; then
  echo "FAIL: $REMOTE_REF is not an ancestor of HEAD" >&2
  echo "Force-push is banned. Do not run git push --force or +refspec." >&2
  echo "Leave origin as-is; reconcile with a forward commit on top of origin/$BRANCH." >&2
  exit 1
fi
echo "OK: fast-forward only ($REMOTE_REF is ancestor of HEAD)"