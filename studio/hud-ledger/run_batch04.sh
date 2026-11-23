#!/usr/bin/env bash
set -euo pipefail
ROOT="$(cd "$(dirname "$0")/../.." && pwd)"
cd "$ROOT"
export PATH="${HOME}/.dotnet:${PATH}"
export GIT_AUTHOR_NAME='treverhatch-glitch'
export GIT_AUTHOR_EMAIL='342607508+treverhatch-glitch@users.noreply.github.com'
export GIT_COMMITTER_NAME="$GIT_AUTHOR_NAME"
export GIT_COMMITTER_EMAIL="$GIT_COMMITTER_EMAIL"
REC="$ROOT/studio/hud-ledger/recovered"

python3 "$ROOT/studio/hud-ledger/fix_staff_mascot_tail_chain.py"

export GEN19="$REC/gen_formatters_19_20.py"
for block in 13_14 15_16 17_18 19_20; do
  python3 "$REC/run_sprints_${block}.py"
  python3 "$ROOT/studio/hud-ledger/fix_staff_mascot_tail_chain.py"
done

git add unity/Assets/Scripts/Hq/StaffGpuPanel.cs unity/Assets/Scripts/Hq/HqMascotStripHud.cs 2>/dev/null || true
if ! git diff --cached --quiet; then
  git commit -m "fix(hud): batch04 staff and mascot tail resync after sprint 20"
fi

dotnet test tools/SimTests/SimTests.csproj --filter 'FullyQualifiedName~golden' -v q
echo "EditMode tests:" "$(grep -c '\[Test\]' unity/Assets/Editor/Tests/HqDisplayQueriesHudTests.cs || true)"