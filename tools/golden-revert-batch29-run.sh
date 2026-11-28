#!/bin/bash
set -euo pipefail
N="${1:?schema step 186-188}"
REPO="$(cd "$(dirname "$0")/.." && pwd)"
cd "$REPO"
LOCK="${TMPDIR:-/tmp}/b29-golden-revert.lock"
exec 9>"$LOCK"
flock -x -w 600 9
exec 9>&-
bash "$REPO/tools/restore-golden-tip.sh" >/dev/null
DOTNET="${DOTNET:-$(command -v dotnet)}"
export PATH="${PATH:-/home/ubuntu/.dotnet:/usr/bin}"
(cd "$REPO/tools/SimTests" && "$DOTNET" build -v q -nologo) || true

declare -A GOLDEN
GOLDEN[186]=146422d3bf4670f794c01625d8d65ee9f4f6bfa387f4f0a15c6d4c4c0871fbd5
GOLDEN[187]=09fa392729cf09bec0d786f0b116c73488b95dee3a1bc3a34a9058cee98a5cca
GOLDEN[188]=a35183bd3f4337b32250e11b5367031c1fb3b4302aec3063e65b98491da0d6ac
GOLDEN[183]=e8caa9be3c25b5adc2b678e208e17bf1ced1ee46f7934bd3f774c7c1beba5254
GOLDEN[184]=9a2f489e24be86e86b0532c805cca7891eb9361985ecc9b9134fe7e0de82943a
GOLDEN[185]=26587a9375c75fcd824fef821dad06487cb0079a0d9660f8f197021adeb23fcb

PREV=$((N - 1))
BK="/tmp/b29-rev-bak-$N"
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

[ "$N" -le 188 ] && disable_cfg evalFailEraRelief
[ "$N" -le 187 ] && disable_cfg openWeightsCopyEraRelief
[ "$N" -le 186 ] && disable_cfg officeHearingEraGrace
[ "$N" -le 185 ] && disable_cfg publicMetricsEraDiscount
[ "$N" -le 184 ] && disable_cfg evalHarnessEraRelief
[ "$N" -le 183 ] && disable_cfg rungSixEraGrace
[ "$N" -le 182 ] && disable_cfg dataSourceRenewEraDiscount
[ "$N" -le 181 ] && disable_cfg lawShapingEraAmplify
[ "$N" -le 180 ] && disable_cfg escapeCrisisEraGrace
[ "$N" -le 179 ] && disable_cfg dataSourceAcquireEraWeeks
[ "$N" -le 178 ] && disable_cfg officeReleaseEraGrace
[ "$N" -le 177 ] && disable_cfg hearingShareEraDampen
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