#!/usr/bin/env bash
# Synthetic CXXIII→CXXXV replay: 853-856, 869-872, 885-888, 901-904 (LOCAL backup lane).
set -euo pipefail
ROOT="$(cd "$(dirname "$0")/../.." && pwd)"
cd "$ROOT"
export PATH="${HOME}/.dotnet:${PATH}"
export GIT_AUTHOR_NAME='treverhatch-glitch'
export GIT_AUTHOR_EMAIL='342607508+treverhatch-glitch@users.noreply.github.com'
export GIT_COMMITTER_NAME="$GIT_AUTHOR_NAME"
export GIT_COMMITTER_EMAIL="$GIT_COMMITTER_EMAIL"
git config commit.gpgsign false
REC="$ROOT/studio/hud-ledger/recovered"
FIX="$ROOT/studio/hud-ledger/fix_staff_mascot_tail_chain.py"

python3 "$FIX"

for run in run_sprints_853_856.py run_sprints_869_872.py run_sprints_885_888.py run_sprints_901_904.py; do
  if [[ ! -f "$REC/$run" ]]; then
    echo "missing $REC/$run" >&2
    exit 1
  fi
  echo "=== $run (synthetic bible chain) ==="
  python3 "$REC/$run"
  python3 "$FIX"
done

if ! git diff --quiet unity/Assets/Scripts/Hq/StaffGpuPanel.cs unity/Assets/Scripts/Hq/HqMascotStripHud.cs 2>/dev/null; then
  git add unity/Assets/Scripts/Hq/StaffGpuPanel.cs unity/Assets/Scripts/Hq/HqMascotStripHud.cs
  git commit -m "fix(hud): batch07 synthetic 853-901 tail resync"
fi

dotnet test tools/SimTests/SimTests.csproj --filter 'FullyQualifiedName~golden' -v q
echo "EditMode tests:" "$(grep -c '\[Test\]' unity/Assets/Editor/Tests/HqDisplayQueriesHudTests.cs || true)"