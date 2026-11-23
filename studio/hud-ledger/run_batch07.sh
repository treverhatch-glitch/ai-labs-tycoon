#!/usr/bin/env bash
# Batch 07: make-chain 69–124, data replay 169+, optional 869→901 when 853 assets exist.
set -euo pipefail
ROOT="$(cd "$(dirname "$0")/../.." && pwd)"
cd "$ROOT"
export PATH="${HOME}/.dotnet:${PATH}"
export GIT_AUTHOR_NAME='treverhatch-glitch'
export GIT_AUTHOR_EMAIL='342607508+treverhatch-glitch@users.noreply.github.com'
export GIT_COMMITTER_NAME="$GIT_AUTHOR_NAME"
export GIT_COMMITTER_EMAIL="$GIT_COMMITTER_EMAIL"
LEDGER="$ROOT/studio/hud-ledger"
REC="$LEDGER/recovered"
FIX="$LEDGER/fix_staff_mascot_tail_chain.py"
MAKE="$LEDGER/materialize_make_runner.py"
MAT="$LEDGER/materialize_runner_from_data.py"
CHAIN="$LEDGER/build_869_885_901_chain.py"

python3 "$FIX"

for make in make_sprints_69_72.py make_sprints_73_76.py make_sprints_77_80.py make_sprints_81_84.py \
  make_sprints_85_88.py make_sprints_89_92.py make_sprints_93_96.py make_sprints_97_100.py \
  make_sprints_101_104.py make_sprints_105_108.py make_sprints_109_112.py make_sprints_113_116.py \
  make_sprints_117_120.py make_sprints_121_124.py; do
  echo "=== $make ==="
  python3 "$MAKE" "$make"
  run="${make/make_sprints_/run_sprints_}"
  python3 "$REC/$run"
  python3 "$FIX"
done

for data in sprint169_172_data.py sprint173_176_data.py sprint177_180_data.py sprint181_184_data.py \
  sprint185_188_data.py sprint189_192_data.py sprint193_196_data.py sprint197_200_data.py \
  sprint201_204_data.py sprint205_208_data.py sprint209_212_data.py sprint213_216_data.py \
  sprint217_220_data.py sprint221_224_data.py sprint225_228_data.py sprint229_232_data.py \
  sprint233_236_data.py sprint237_240_data.py sprint241_244_data.py sprint245_248_data.py \
  sprint249_252_data.py sprint253_256_data.py sprint257_260_data.py sprint261_264_data.py; do
  python3 "$MAT" "$data"
  run="run_sprints_${data#sprint}"
  run="${run/_data.py/.py}"
  echo "=== $run ==="
  python3 "$REC/$run"
  python3 "$FIX"
done

if python3 "$CHAIN"; then
  for run in run_sprints_869_872.py run_sprints_885_888.py run_sprints_901_904.py; do
    if [[ -f "$REC/$run" ]]; then
      echo "=== $run ==="
      python3 "$REC/$run"
      python3 "$FIX"
    fi
  done
else
  echo "869→901 chain skipped (needs sprint853_856 assets on replay path)"
fi

if ! git diff --quiet unity/Assets/Scripts/Hq/StaffGpuPanel.cs unity/Assets/Scripts/Hq/HqMascotStripHud.cs 2>/dev/null; then
  git add unity/Assets/Scripts/Hq/StaffGpuPanel.cs unity/Assets/Scripts/Hq/HqMascotStripHud.cs
  git commit -m "fix(hud): batch07 tail resync"
fi

dotnet test tools/SimTests/SimTests.csproj --filter 'FullyQualifiedName~golden' -v q
echo "EditMode tests:" "$(grep -c '\[Test\]' unity/Assets/Editor/Tests/HqDisplayQueriesHudTests.cs || true)"