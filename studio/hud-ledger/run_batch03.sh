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
git add unity/Assets/Scripts/Hq/StaffGpuPanel.cs unity/Assets/Scripts/Hq/HqMascotStripHud.cs
if ! git diff --cached --quiet; then
  git commit -m "fix(hud): rebuild staff and mascot Part tail chains for sprint replay"
fi

for n in 4 5 6 7 8; do
  python3 "$REC/sprint${n}_wire.py"
done
python3 "$REC/run_sprints_9_10.py"
python3 "$REC/run_sprints_11_12.py"
python3 "$ROOT/studio/hud-ledger/fix_staff_mascot_tail_chain.py"
git add unity/Assets/Scripts/Hq/StaffGpuPanel.cs unity/Assets/Scripts/Hq/HqMascotStripHud.cs
if ! git diff --cached --quiet; then
  git commit -m "fix(hud): resync staff and mascot tails after sprint 12"
fi

dotnet test tools/SimTests/SimTests.csproj --filter 'FullyQualifiedName~golden' -v q
echo "EditMode test count:" "$(grep -c '\[Test\]' unity/Assets/Editor/Tests/HqDisplayQueriesHudTests.cs || true)"