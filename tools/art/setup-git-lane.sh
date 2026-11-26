#!/usr/bin/env bash
# Configure this clone for art-lane commits: no Co-authored-by, hooks strip trailers.
set -euo pipefail
ROOT="$(cd "$(dirname "$0")/../.." && pwd)"
cd "$ROOT"
git config core.hooksPath .githooks
git config commit.cleanup scissors
git config commit.gpgsign false
git config merge.log false
chmod +x .githooks/prepare-commit-msg 2>/dev/null || true
echo "OK: core.hooksPath=.githooks (prepare-commit-msg strips Co-authored-by)"