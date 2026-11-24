#!/usr/bin/env bash
# Redo 105-108 (GPG), idempotent finish 109-124, then synthetic 853→901.
set -euo pipefail
ROOT="$(cd "$(dirname "$0")/../.." && pwd)"
cd "$ROOT"
export PATH="${HOME}/.dotnet:${PATH}"
export GIT_AUTHOR_NAME='treverhatch-glitch'
export GIT_AUTHOR_EMAIL='342607508+treverhatch-glitch@users.noreply.github.com'
export GIT_COMMITTER_NAME="$GIT_AUTHOR_NAME"
export GIT_COMMITTER_EMAIL="$GIT_COMMITTER_EMAIL"
git config commit.gpgsign false
LEDGER="$ROOT/studio/hud-ledger"
REC="$LEDGER/recovered"
FIX="$LEDGER/fix_staff_mascot_tail_chain.py"
MAKE="$LEDGER/materialize_make_runner.py"

python3 "$FIX"
echo "=== redo run_sprints_105_108.py (gpg-safe) ==="
python3 "$REC/run_sprints_105_108.py"
python3 "$FIX"

for make in make_sprints_109_112.py make_sprints_113_116.py make_sprints_117_120.py make_sprints_121_124.py; do
  echo "=== $make (idempotent) ==="
  python3 "$MAKE" "$make"
  run="${make/make_sprints_/run_sprints_}"
  python3 "$REC/$run"
  python3 "$FIX"
done

for run in run_sprints_853_856.py run_sprints_869_872.py run_sprints_885_888.py run_sprints_901_904.py; do
  echo "=== $run (synthetic bible) ==="
  python3 "$REC/$run"
  python3 "$FIX"
done

if ! git diff --quiet unity/Assets/Scripts/Hq/StaffGpuPanel.cs unity/Assets/Scripts/Hq/HqMascotStripHud.cs 2>/dev/null; then
  git add unity/Assets/Scripts/Hq/StaffGpuPanel.cs unity/Assets/Scripts/Hq/HqMascotStripHud.cs
  git commit -m "fix(hud): batch07 redo 105-901 tail resync"
fi

dotnet test tools/SimTests/SimTests.csproj --filter 'FullyQualifiedName~golden' -v q
echo "EditMode tests:" "$(grep -c '\[Test\]' unity/Assets/Editor/Tests/HqDisplayQueriesHudTests.cs || true)"