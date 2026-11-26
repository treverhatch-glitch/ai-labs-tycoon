#!/usr/bin/env bash
# Fail if origin/<branch> is not an ancestor of HEAD (force-push would be required).
# Force-push (--force, --force-with-lease, +refspec) is banned on all art lanes.
set -euo pipefail

check_push_fastforward() {
  local ref="${1:-}"
  if [[ -z "$ref" ]]; then
  if git rev-parse --verify "@{u}" >/dev/null 2>&1; then
      ref=$(git rev-parse --abbrev-ref --symbolic-full-name '@{u}')
    else
      local br
      br=$(git branch --show-current 2>/dev/null || true)
      if [[ -n "$br" ]]; then
        ref="origin/${br}"
      fi
    fi
  fi
  if [[ -z "$ref" ]]; then
    echo "WARN: check_push_fastforward: no upstream ref (set ART_PREPUSH_UPSTREAM)" >&2
    return 0
  fi
  if ! git rev-parse --verify "$ref" >/dev/null 2>&1; then
    echo "OK: $ref not present locally (first push of branch — plain push only, no force)" >&2
    return 0
  fi
  if git merge-base --is-ancestor "$ref" HEAD; then
    echo "OK: $ref is ancestor of HEAD (fast-forward push allowed)"
    return 0
  fi
  echo "FAIL: $ref is not an ancestor of HEAD" >&2
  echo "Force-push is banned. Leave remote as-is; fix with forward commits only." >&2
  return 1
}

reject_force_push_args() {
  local arg
  for arg in "$@"; do
    case "$arg" in
      --force|--force-with-lease|-f)
        echo "FAIL: $arg is banned on art lanes" >&2
        return 1
        ;;
    esac
    if [[ "$arg" == +* ]]; then
      echo "FAIL: force refspec ($arg) is banned" >&2
      return 1
    fi
  done
  return 0
}