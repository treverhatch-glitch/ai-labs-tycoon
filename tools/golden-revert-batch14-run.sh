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
GOLDEN[135]=02d59ecd6d2fe5d30c4433299fd261aa043383417cab5af62cbaa5360ac1417d
GOLDEN[136]=ba20786126337213bdca40c6143c358d16e51dedd7a685fe934e25597a55b634
GOLDEN[137]=f1726079645bc79590ae609f45b089897e519ce512a47822286c397b78d9e44e
GOLDEN[138]=425ae8994c068efac820428f13d0eda161bde6a5763d28fdb5e7da4e348b7f1c

PREV=$((N - 1))
BK="/tmp/b14-rev-bak-$N"
mkdir -p "$BK"
for f in Rules/State/GameState.cs Rules/Sim/Simulation.cs Rules/Systems/MarketSystem.cs Rules/Systems/SafetySystem.cs Rules/Systems/TrainingSystem.cs Rules/Data/default-config.json tools/SimTests/SaveAndDeterminismTests.cs; do
  cp "$f" "$BK/$(echo $f | tr '/' '_')"
done
restore() {
  for f in Rules/State/GameState.cs Rules/Sim/Simulation.cs Rules/Systems/MarketSystem.cs Rules/Systems/SafetySystem.cs Rules/Systems/TrainingSystem.cs Rules/Data/default-config.json tools/SimTests/SaveAndDeterminismTests.cs; do
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
[ "$N" -le 143 ] && { strip_sync 's\.Int\("baggagePicked"'; strip_sync 's\.Str\("baggageId"'; disable_cfg scenarioBaggage; }
[ "$N" -le 142 ] && { strip_sync 's\.Int\("takeoffCooldownUntil"'; disable_cfg takeoffCrisis; comment_hook 'TakeoffCrisis.RunWeek'; }
[ "$N" -le 140 ] && { strip_sync 's\.Str\("lawShapeEra"'; strip_sync 's\.Int\("lawShape'; disable_cfg lawShaping; sed -i '/LawShaping.OnEraBegan/d' Rules/Systems/EraSystem.cs; }
[ "$N" -le 139 ] && { strip_sync 's\.Int\("campaignDeadlineTick"'; disable_cfg campaignDeadline; comment_hook 'CampaignDeadline.RunWeek'; }

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
