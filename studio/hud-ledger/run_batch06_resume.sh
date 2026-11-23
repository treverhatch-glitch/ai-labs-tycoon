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
FIX="$ROOT/studio/hud-ledger/fix_staff_mascot_tail_chain.py"
MAT="$ROOT/studio/hud-ledger/materialize_runner_from_data.py"

python3 "$FIX"

for runner in run_sprints_65_68.py; do
  echo "=== $runner ==="
  python3 "$REC/$runner"
  python3 "$FIX"
done

for data in sprint125_128_data.py sprint129_132_data.py sprint133_136_data.py sprint137_140_data.py \
  sprint141_144_data.py sprint145_148_data.py sprint149_152_data.py sprint153_156_data.py \
  sprint157_160_data.py sprint161_164_data.py sprint165_168_data.py; do
  python3 "$MAT" "$data"
  base="${data/_data.py/}"
  base="${base/sprint/run_sprints_}"
  echo "=== $base ==="
  python3 "$REC/${base}.py"
  python3 "$FIX"
done

if ! git diff --quiet unity/Assets/Scripts/Hq/StaffGpuPanel.cs unity/Assets/Scripts/Hq/HqMascotStripHud.cs 2>/dev/null; then
  git add unity/Assets/Scripts/Hq/StaffGpuPanel.cs unity/Assets/Scripts/Hq/HqMascotStripHud.cs
  git commit -m "fix(hud): batch06 tail resync"
fi

dotnet test tools/SimTests/SimTests.csproj --filter 'FullyQualifiedName~golden' -v q
echo "EditMode:" "$(grep -c '\[Test\]' unity/Assets/Editor/Tests/HqDisplayQueriesHudTests.cs)"