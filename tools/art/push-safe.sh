#!/usr/bin/env bash
# Plain git push only: ancestor check + no force flags.
set -euo pipefail
ROOT="$(cd "$(dirname "$0")/../.." && pwd)"
cd "$ROOT"
# shellcheck source=tools/art/check_push_fastforward.sh
source "$ROOT/tools/art/check_push_fastforward.sh"

if ! reject_force_push_args "$@"; then
  exit 1
fi

REF="${ART_PREPUSH_UPSTREAM:-}"
if [[ -z "$REF" ]] && git rev-parse --verify '@{u}' >/dev/null 2>&1; then
  REF=$(git rev-parse --abbrev-ref --symbolic-full-name '@{u}')
fi
if [[ -z "$REF" ]]; then
  br=$(git branch --show-current)
  REF="origin/${br}"
fi

git fetch origin 2>/dev/null || true
check_push_fastforward "$REF"

if [[ $# -eq 0 ]]; then
  br=$(git branch --show-current)
  exec git push -u origin "$br"
fi
exec git push "$@"