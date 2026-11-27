#!/bin/bash
set -euo pipefail
N="${1:?schema step 173-176}"
REPO="$(cd "$(dirname "$0")/.." && pwd)"
cd "$REPO"
LOCK="${TMPDIR:-/tmp}/b25-golden-revert.lock"
exec 9>"$LOCK"
flock -x -w 600 9
exec 9>&-
bash "$REPO/tools/restore-golden-tip.sh" >/dev/null
DOTNET="${DOTNET:-/home/ubuntu/.dotnet/dotnet}"
export PATH="${PATH:-/home/ubuntu/.dotnet:/usr/bin}"
(cd "$REPO/tools/SimTests" && "$DOTNET" build -v q -nologo) || true

declare -A GOLDEN
GOLDEN[173]=08d490e25136e0b8ca2a394a3867aa7aa28f56f53b0efffca43d80e07d8e5c58
GOLDEN[174]=4b7ab508ce93864e804ee6de2f7ed52de07530be3323056fb4836b1ef53d8c11
GOLDEN[175]=4ea22bf072b8cf83d3afee29229c8103ab18e0bc92fa6bc65294f449e6c4b359
GOLDEN[176]=3a2c60a1367a521af3d0abc481d40bbea4ad9348915c8405aa4f4dffb46970bb

PREV=$((N - 1))
BK="/tmp/b25-rev-bak-$N"
mkdir -p "$BK"
for f in Rules/State/GameState.cs Rules/Data/default-config.json tools/SimTests/SaveAndDeterminismTests.cs; do
  cp "$f" "$BK/$(echo $f | tr '/' '_')"
done
restore() {
  for f in Rules/State/GameState.cs Rules/Data/default-config.json tools/SimTests/SaveAndDeterminismTests.cs; do
    cp "$BK/$(echo $f | tr '/' '_')" "$REPO/$f"
  done
  (cd "$REPO/tools/SimTests" && "$DOTNET" build -v q -nologo) || true
}
trap restore EXIT

sed -i "s/CurrentSchema = [0-9]*;/CurrentSchema = $PREV;/" Rules/State/GameState.cs
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

[ "$N" -le 176 ] && disable_cfg scenarioGoalEraTrust
[ "$N" -le 175 ] && disable_cfg takeoffAnswerEraCooldown
[ "$N" -le 174 ] && disable_cfg exitDebtEraHaircut
[ "$N" -le 173 ] && disable_cfg computeEquipment
[ "$N" -le 172 ] && disable_cfg takeoffCrisisEraGrace
[ "$N" -le 171 ] && disable_cfg scenarioGoalsEraSlack
[ "$N" -le 170 ] && disable_cfg marginSatisfyEraRelief
[ "$N" -le 169 ] && disable_cfg dataSourceAcquireEraDiscount
[ "$N" -le 168 ] && disable_cfg rungSixEraCooldown
[ "$N" -le 167 ] && disable_cfg hearingSharePriceEraBoost
[ "$N" -le 166 ] && disable_cfg marginCallEraGrace
[ "$N" -le 165 ] && disable_cfg specialistBranchEraDiscount
[ "$N" -le 164 ] && disable_cfg lawTargetEraAmplify
[ "$N" -le 163 ] && disable_cfg sectorPricingEraRelief
[ "$N" -le 162 ] && disable_cfg dataSourceRenewEraBonus
[ "$N" -le 161 ] && disable_cfg lawShapingEraCooldown
[ "$N" -le 160 ] && disable_cfg campaignDeadlineEraGrace
[ "$N" -le 159 ] && disable_cfg evalHarnessEraBoost
[ "$N" -le 158 ] && disable_cfg openWeightsEraThrottle
[ "$N" -le 157 ] && disable_cfg publicMetricsEraLoad
[ "$N" -le 156 ] && disable_cfg dataSourceSpecialistPairing

EXP="${GOLDEN[$N]}"
sed -i "s/const string Golden = \"[0-9a-f]*\"/const string Golden = \"$EXP\"/" "$REPO/tools/SimTests/SaveAndDeterminismTests.cs"
cd tools/SimTests
"$DOTNET" build -v q -nologo
OUT=$("$DOTNET" test --no-build --filter "FullyQualifiedName~The_golden_run" 2>&1) || true
echo "$OUT" | grep -E "Passed!|Failed!|New digest" | head -3
if echo "$OUT" | grep -q "Passed!"; then
  echo "schema $N revert: Passed!"
  exit 0
fi
NEW=$(echo "$OUT" | sed -n 's/.*New digest: \([0-9a-f]*\).*/\1/p' | head -1)
echo "schema $N revert: FAILED digest=${NEW:-unknown}"
exit 1