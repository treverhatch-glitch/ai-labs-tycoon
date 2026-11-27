#!/usr/bin/env bash
# Packet34 sign lane (PR #90): prepush + committed verify. No force-push.
set -euo pipefail
ROOT="$(cd "$(dirname "$0")/../.." && pwd)"
cd "$ROOT"
export ART_PREPUSH_BRANCH="${ART_PREPUSH_BRANCH:-cursor/early-art-env-hud-packet34-97cc}"
export ART_PREPUSH_UPSTREAM="${ART_PREPUSH_UPSTREAM:-origin/$ART_PREPUSH_BRANCH}"
export ART_PREPUSH_REMOTE="${ART_PREPUSH_REMOTE:-origin}"

"$ROOT/tools/art/prepush.sh"
python3 tools/art/check_preview_sign_regression.py
if [[ "${SKIP_SIGN_VERIFY:-}" != "1" ]]; then
  python3 tools/art/verify_sign_preview_committed.py
fi
echo "packet34 sign prepush OK"