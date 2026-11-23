#!/bin/bash
set -euo pipefail
N="${1:?schema step 139-143}"
REPO="$(cd "$(dirname "$0")/.." && pwd)"
cd "$REPO"
LOCK="${TMPDIR:-/tmp}/b15-golden-revert.lock"
exec 9>"$LOCK"
flock -x -w 600 9
# Pre-restore only after batch 15 is committed on this branch (restore-golden-tip checks out HEAD).
bash "$REPO/tools/restore-golden-tip.sh" >/dev/null
(cd "$REPO/tools/SimTests" && dotnet build -v q -nologo) || true

declare -A GOLDEN
GOLDEN[139]=a765c4d62c0a8c9d35704860396e45148658f6703f0235985a26baffebae6aac
GOLDEN[140]=8435b7e616126197d7c25e02e13e8d248c5789bd33191af083298f699704f594
GOLDEN[141]=c07ea29311f9d746c5f527ae6aef3fb55f18d7a824b6a10609e6f4d099715097
GOLDEN[142]=0d1501af776de2ceee41d6e4c46b64771a9db345adce4f23ecd1d993c1338914
GOLDEN[143]=a2d0d0989bae77a01891a37609ae3f511a5dcc22e9d4bb546889461bfddb0e00

PREV=$((N - 1))
BK="/tmp/b15-rev-bak-$N"
mkdir -p "$BK"
for f in Rules/State/GameState.cs Rules/Sim/Simulation.cs Rules/Systems/DisplayQueries.cs Rules/Systems/EndingSystem.cs Rules/Systems/EraRules.cs Rules/Systems/OfficeEvents.cs Rules/Systems/EraSystem.cs Rules/Systems/TrustSystem.cs Rules/Sim/WorldSetup.cs Rules/Sim/Simulation.cs Rules/Data/default-config.json tools/SimTests/SaveAndDeterminismTests.cs; do
  cp "$f" "$BK/$(echo $f | tr '/' '_')"
done
restore() {
  for f in Rules/State/GameState.cs Rules/Sim/Simulation.cs Rules/Systems/DisplayQueries.cs Rules/Systems/EndingSystem.cs Rules/Systems/EraRules.cs Rules/Systems/OfficeEvents.cs Rules/Systems/EraSystem.cs Rules/Systems/TrustSystem.cs Rules/Sim/WorldSetup.cs Rules/Data/default-config.json tools/SimTests/SaveAndDeterminismTests.cs; do
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

[ "$N" -eq 143 ] && { strip_sync 's\.Int\("baggagePicked"'; strip_sync 's\.Str\("baggageId"'; disable_cfg scenarioBaggage; comment_hook 'ScenarioBaggage'; }
[ "$N" -eq 142 ] && { strip_sync 's\.Int\("takeoffCooldownUntil"'; disable_cfg takeoffCrisis; sed -i '/OfficeTrigger.Takeoff/d' Rules/Config/OfficeConfig.cs 2>/dev/null || true; comment_hook 'TakeoffCrisis.RunWeek'; }
[ "$N" -eq 141 ] && { sed -i 's|b.Loans = ExitDebt.LoanBalance(lab)|b.Loans = 0|' Rules/Systems/DisplayQueries.cs; sed -i 's|b.Notes = ExitDebt.NoteBalance(lab)|b.Notes = 0|' Rules/Systems/DisplayQueries.cs; sed -i 's|b.Rbf = ExitDebt.RbfBalance(lab)|b.Rbf = 0|' Rules/Systems/DisplayQueries.cs; sed -i 's|b.CovenantCalls = ExitDebt.CovenantCalls(c, lab)|b.CovenantCalls = 0|' Rules/Systems/DisplayQueries.cs; }
[ "$N" -eq 140 ] && { strip_sync 's\.Str\("lawShapeEra"'; strip_sync 's\.Int\("lawShape'; disable_cfg lawShaping; sed -i '/LawShaping.OnEraBegan/d' Rules/Systems/EraSystem.cs; }
[ "$N" -eq 139 ] && { strip_sync 's\.Int\("campaignDeadlineTick"'; disable_cfg campaignDeadline; comment_hook 'CampaignDeadline.RunWeek'; }

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