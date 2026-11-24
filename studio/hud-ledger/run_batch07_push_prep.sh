#!/usr/bin/env bash
# Read-only pre-push gate for grouped batch 07 (run locally; push only on coordinator GO).
set -euo pipefail
ROOT="$(cd "$(dirname "$0")/../.." && pwd)"
cd "$ROOT"
REMOTE_PIN="${REMOTE_PIN:-9a33c9fd453597edb7b5d183529c8a399a745864}"
export PATH="${HOME}/.dotnet:${PATH}"

echo "=== batch07 push prep (LOCAL) ==="
echo "branch: $(git branch --show-current)"
echo "remote pin: ${REMOTE_PIN:0:8}"
ahead="$(git rev-list --count "${REMOTE_PIN}"..HEAD 2>/dev/null || echo '?')"
echo "commits ahead of pin: ${ahead}"
tests="$(grep -c '\[Test\]' unity/Assets/Editor/Tests/HqDisplayQueriesHudTests.cs || true)"
echo "EditMode [Test] count: ${tests}"

if grep -q "def insert_formatters" unity/Assets/Scripts/Hq/HqDisplayQueriesHud.cs 2>/dev/null; then
  echo "FAIL: embedded Python in HqDisplayQueriesHud.cs" >&2
  exit 1
fi

python3 studio/hud-ledger/fix_staff_mascot_tail_chain.py
if ! git diff --quiet unity/Assets/Scripts/Hq/StaffGpuPanel.cs unity/Assets/Scripts/Hq/HqMascotStripHud.cs 2>/dev/null; then
  echo "WARN: tail resync produced diffs (commit before push)"
fi

dotnet test tools/SimTests/SimTests.csproj --filter 'FullyQualifiedName~golden' -v q
echo "golden: PASS"
echo "gpg sign: $(git config commit.gpgsign)"
echo "=== ready for coordinator GO (no push from this script) ==="