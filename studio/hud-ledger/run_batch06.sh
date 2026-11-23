#!/usr/bin/env bash
# Batch 06: materialize sprint 61+ runners, replay toward 901–904.
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
TRANSCRIPT_MAT="$ROOT/studio/hud-ledger/materialize_from_transcript_block.py"

python3 "$FIX"

# --- sprints 61–68 from transcript (57_60 base) ---
if [[ -f "$REC/run_sprints_61_64.py" ]]; then
  python3 "$REC/run_sprints_61_64.py"
  python3 "$FIX"
fi
if [[ ! -f "$REC/run_sprints_65_68.py" ]]; then
  python3 "$TRANSCRIPT_MAT" 65 61 || true
fi
if [[ -f "$REC/run_sprints_65_68.py" ]]; then
  python3 "$REC/run_sprints_65_68.py"
  python3 "$FIX"
fi

# --- materialize + run data-backed blocks (125–168 toward spend/income arc) ---
for data in sprint125_128_data.py sprint129_132_data.py sprint133_136_data.py sprint137_140_data.py \
  sprint141_144_data.py sprint145_148_data.py sprint149_152_data.py sprint153_156_data.py \
  sprint157_160_data.py sprint161_164_data.py sprint165_168_data.py; do
  python3 "$MAT" "$data"
  base="${data/_data.py/}"
  base="${base/sprint/run_sprints_}"
  runner="$REC/${base}.py"
  echo "=== $runner ==="
  python3 "$runner"
  python3 "$FIX"
done

# --- 901–904 mirror when 885 assets exist ---
if [[ -f "$REC/sprint869_872_data.py" ]] || [[ -f "$REC/sprint885_888_data.py" ]]; then
  python3 "$REC/build_901_904_from_885.py" 2>/dev/null || python3 "$ROOT/studio/hud-ledger/build_901_904_from_885.py" || true
fi
if [[ -f /tmp/run_sprints_901_904.py ]]; then
  python3 /tmp/run_sprints_901_904.py
  python3 "$FIX"
elif [[ -f "$REC/run_sprints_901_904.py" ]]; then
  python3 "$REC/run_sprints_901_904.py"
  python3 "$FIX"
fi

if ! git diff --quiet unity/Assets/Scripts/Hq/StaffGpuPanel.cs unity/Assets/Scripts/Hq/HqMascotStripHud.cs 2>/dev/null; then
  git add unity/Assets/Scripts/Hq/StaffGpuPanel.cs unity/Assets/Scripts/Hq/HqMascotStripHud.cs
  git commit -m "fix(hud): batch06 staff and mascot tail resync"
fi

dotnet test tools/SimTests/SimTests.csproj --filter 'FullyQualifiedName~golden' -v q
echo "EditMode tests:" "$(grep -c '\[Test\]' unity/Assets/Editor/Tests/HqDisplayQueriesHudTests.cs || true)"