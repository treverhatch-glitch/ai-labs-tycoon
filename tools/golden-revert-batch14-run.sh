#!/bin/bash
set -euo pipefail
N="${1:?schema step 135-138}"
REPO="$(cd "$(dirname "$0")/.." && pwd)"
cd "$REPO"
LOCK="${TMPDIR:-/tmp}/b14-golden-revert.lock"
exec 9>"$LOCK"
flock -x -w 600 9
bash "$REPO/tools/restore-golden-tip.sh" >/dev/null
(cd "$REPO/tools/SimTests" && dotnet build -v q -nologo) || true

declare -A GOLDEN
GOLDEN[135]=23718345285dbcbcc19df91bf45eb8ea795f18b7b4d7d52e0f2957189f735527
GOLDEN[136]=cb70363290193e36a6d228450e9b78aec229aec774a9aa638aa7d601cc6adec9
GOLDEN[137]=ef5d70187a3d685f8e0b6f9fe9d3508f172636b9b18dd6e6e397105d89e66f32
GOLDEN[138]=553fc69f1cc1cab4c5a055767a36889d7b7f8de3ee5d32a16b54aa2fefe23d99

PREV=$((N - 1))
BK="/tmp/b14-rev-bak-$N"
mkdir -p "$BK"
for f in Rules/State/GameState.cs Rules/Sim/Simulation.cs Rules/Systems/MarketSystem.cs Rules/Systems/SafetySystem.cs Rules/Systems/TrainingSystem.cs \
  Rules/Systems/DisplayQueries.cs Rules/Systems/EraRules.cs Rules/Data/default-config.json tools/SimTests/SaveAndDeterminismTests.cs; do
  cp "$f" "$BK/$(echo $f | tr '/' '_')"
done
restore() {
  for f in Rules/State/GameState.cs Rules/Sim/Simulation.cs Rules/Systems/MarketSystem.cs Rules/Systems/SafetySystem.cs Rules/Systems/TrainingSystem.cs \
  Rules/Systems/DisplayQueries.cs Rules/Systems/EraRules.cs Rules/Data/default-config.json tools/SimTests/SaveAndDeterminismTests.cs; do
    cp "$BK/$(echo $f | tr '/' '_')" "$REPO/$f"
  done
  (cd "$REPO/tools/SimTests" && dotnet build -v q -nologo) || true
}
trap restore EXIT

sed -i "s/CurrentSchema = [0-9]*;/CurrentSchema = $PREV;/" Rules/State/GameState.cs
strip_sync() { sed -i -E "/$1/d" Rules/State/GameState.cs; }
disable_cfg() {
  python3 -c "
import json, os
p='Rules/Data/default-config.json'
with open(p) as f: d=json.load(f)
if '$1' in d: d['$1']['enabled']=0
t=p+'.tmp'
with open(t,'w') as f: json.dump(d,f,indent=2); f.write(chr(10))
os.replace(t,p)
"
}
comment_hook() { sed -i "s|^[[:space:]]*$1|                    // $1|" Rules/Sim/Simulation.cs; }

[ "$N" -le 138 ] && { strip_sync 's\.StrList\("priceSectors"|s\.IntList\("priceTiers"'; disable_cfg sectorPricing; sed -i '/if (SectorPricing.Enabled(c) && demandLab.Id == state.PlayerLabId)/,+1 s|^|//|' Rules/Systems/MarketSystem.cs; sed -i 's| + SectorPricing.LabOverloadPermille(c, lab)||' Rules/Systems/SafetySystem.cs; }
[ "$N" -le 137 ] && { strip_sync 's\.Int\("wgtOpen"'; disable_cfg openWeights; comment_hook 'OpenWeights.RunWeek'; }
[ "$N" -le 136 ] && { strip_sync 's\.Int\("srcWeeks'; disable_cfg dataSources; comment_hook 'DataSources.RunWeek'; sed -i 's|int src = DataSources.QualityPermille(c, lab) / 50;||' Rules/Systems/TrainingSystem.cs; sed -i 's|model.CapLuck += src;||' Rules/Systems/TrainingSystem.cs; }
[ "$N" -le 135 ] && { strip_sync 's\.Int\("specKind"'; disable_cfg specialistLines; }

# shellcheck source=golden-revert-disable-batch15-tip.sh
source "$REPO/tools/golden-revert-disable-batch15-tip.sh"
disable_batch15_at_tip

EXP="${GOLDEN[$N]}"
export PATH="${PATH:-/home/ubuntu/.dotnet:/usr/bin}"
sed -i "s/const string Golden = \"[0-9a-f]*\"/const string Golden = \"$EXP\"/" "$REPO/tools/SimTests/SaveAndDeterminismTests.cs"
cd tools/SimTests
dotnet build -v q -nologo
OUT=$(dotnet test --no-build --filter "FullyQualifiedName~The_golden_run" 2>&1) || true
echo "$OUT" | grep -E "Passed!|Failed!|New digest" | head -3
if echo "$OUT" | grep -q "Passed!"; then
  echo "schema $N revert: Passed!"
  exit 0
fi
NEW=$(echo "$OUT" | sed -n 's/.*New digest: \([0-9a-f]*\).*/\1/p' | head -1)
echo "schema $N revert: FAILED digest=${NEW:-unknown}"
exit 1
