#!/usr/bin/env bash
# Unity / HUD backup lane — run before every plain `git push`. Hard-fails on force-push risk.
set -euo pipefail
ROOT="$(cd "$(dirname "$0")/../.." && pwd)"
cd "$ROOT"

BRANCH="${UNITY_PREPUSH_BRANCH:-$(git branch --show-current)}"
REMOTE="${UNITY_PREPUSH_REMOTE:-origin}"
UPSTREAM="${UNITY_PREPUSH_UPSTREAM:-${REMOTE}/${BRANCH}}"

echo "== unity prepush: branch=${BRANCH} upstream=${UPSTREAM} =="

git fetch "${REMOTE}" "${BRANCH}" 2>/dev/null || true

if git rev-parse --verify "${UPSTREAM}" >/dev/null 2>&1; then
  if ! git merge-base --is-ancestor "${UPSTREAM}" HEAD; then
    echo "ABORT: ${UPSTREAM} is not an ancestor of HEAD (force-push would be required)." >&2
    echo "  Fix forward: fetch, merge or commit-tree on top of ${UPSTREAM}, then push without --force." >&2
    exit 1
  fi
  PUSH_RANGE="${UPSTREAM}..HEAD"
else
  echo "note: no ${UPSTREAM} yet (first push for branch)."
  PUSH_RANGE="HEAD"
fi

if git log --format=%B ${PUSH_RANGE} | grep -qi 'co-authored-by'; then
  echo "ABORT: Co-authored-by in commits to push (${PUSH_RANGE})." >&2
  exit 1
fi

if git log --format='%an <%ae>' ${PUSH_RANGE} | grep -v 'treverhatch-glitch <342607508+treverhatch-glitch@users.noreply.github.com>' | grep -q .; then
  echo "ABORT: non-noreply author in commits to push." >&2
  git log --format='%h %an <%ae>' ${PUSH_RANGE}
  exit 1
fi

export PATH="${HOME}/.dotnet:${PATH}"
if [[ -f tools/SimTests/SimTests.csproj ]]; then
  dotnet test tools/SimTests/SimTests.csproj --filter 'FullyQualifiedName~golden' -v q
fi

echo "unity prepush: GREEN (${BRANCH})"