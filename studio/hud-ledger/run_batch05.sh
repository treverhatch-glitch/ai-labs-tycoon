#!/usr/bin/env bash
# Batch 05: replay recovered 4-sprint blocks 21-60 toward bible macro (~880+ needs more runners).
set -euo pipefail
ROOT="$(cd "$(dirname "$0")/../.." && pwd)"
cd "$ROOT"
export PATH="${HOME}/.dotnet:${PATH}"
export GIT_AUTHOR_NAME='treverhatch-glitch'
export GIT_AUTHOR_EMAIL='342607508+treverhatch-glitch@users.noreply.github.com'
export GIT_COMMITTER_NAME="$GIT_AUTHOR_NAME"
export GIT_COMMITTER_EMAIL="$GIT_COMMITTER_EMAIL"
REC="$ROOT/studio/hud-ledger/recovered"
FIX="$ROOT/studio/hud-ledger/fix_staff_mascot_tail_chain.py"

python3 "$FIX"

for block in 21_24 25_28 29_32 33_36 37_40 41_44 45_48 49_52 53_56 57_60; do
  echo "=== run_sprints_${block} ==="
  python3 "$REC/run_sprints_${block}.py"
  python3 "$FIX"
done

if ! git diff --quiet unity/Assets/Scripts/Hq/StaffGpuPanel.cs unity/Assets/Scripts/Hq/HqMascotStripHud.cs; then
  git add unity/Assets/Scripts/Hq/StaffGpuPanel.cs unity/Assets/Scripts/Hq/HqMascotStripHud.cs
  git commit -m "fix(hud): batch05 staff and mascot tail resync after sprint 60"
fi

dotnet test tools/SimTests/SimTests.csproj --filter 'FullyQualifiedName~golden' -v q
echo "EditMode tests:" "$(grep -c '\[Test\]' unity/Assets/Editor/Tests/HqDisplayQueriesHudTests.cs || true)"
echo "Batch05 replay through sprint 60 complete (macro 880+ needs transcript runners beyond recovered/)."