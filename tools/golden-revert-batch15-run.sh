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
DOTNET="${DOTNET:-$(command -v dotnet)}"
export PATH="${PATH:-/home/ubuntu/.dotnet:/usr/bin}"
(cd "$REPO/tools/SimTests" && "$DOTNET" build -v q -nologo) || true

declare -A GOLDEN
GOLDEN[139]=660520a1b64bcb35121217fd95c8fe8d956ccd3413df107a448cde61c870287f
GOLDEN[140]=c9af30446ac217ab782fdcc81578e520e56e49808d257c01a2b14c0ed7d06edb
GOLDEN[141]=d14c74e0ddc6d5e8898f3a56c05908977050b542f9726e5a682d0fffd14c17ba
GOLDEN[142]=3bce0ebbc6fdb19507e4a5c9bb007c60974df9a2b6e824129777c2fd74e207ae
GOLDEN[143]=76a991fcfed87681e4168427620ff952e60d8da61dbba2a0b883f7d268ebfef4

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
  (cd "$REPO/tools/SimTests" && "$DOTNET" build -v q -nologo) || true
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

[ "$N" -le 143 ] && { strip_sync 's\.Int\("baggagePicked"'; strip_sync 's\.Str\("baggageId"'; disable_cfg scenarioBaggage; comment_hook 'ScenarioBaggage'; }
[ "$N" -le 142 ] && { strip_sync 's\.Int\("takeoffCooldownUntil"'; disable_cfg takeoffCrisis; comment_hook 'TakeoffCrisis.RunWeek'; }
[ "$N" -le 141 ] && { sed -i 's|b.Loans = ExitDebt.LoanBalance(lab)|b.Loans = 0|' Rules/Systems/DisplayQueries.cs; sed -i 's|b.Notes = ExitDebt.NoteBalance(lab)|b.Notes = 0|' Rules/Systems/DisplayQueries.cs; sed -i 's|b.Rbf = ExitDebt.RbfBalance(lab)|b.Rbf = 0|' Rules/Systems/DisplayQueries.cs; sed -i 's|b.CovenantCalls = ExitDebt.CovenantCalls(c, lab)|b.CovenantCalls = 0|' Rules/Systems/DisplayQueries.cs; sed -i 's|b.Total = gross - b.StaffShare - b.Loans - b.Notes - b.Rbf - b.CovenantCalls;|b.Total = gross - b.StaffShare;|' Rules/Systems/DisplayQueries.cs; }
[ "$N" -le 140 ] && { strip_sync 's\.Str\("lawShapeEra"'; strip_sync 's\.Int\("lawShape'; disable_cfg lawShaping; sed -i '/lab.Id == state.PlayerLabId)/,+1d' Rules/Systems/EraRules.cs; }
[ "$N" -le 139 ] && { strip_sync 's\.Int\("campaignDeadlineTick"'; disable_cfg campaignDeadline; comment_hook 'CampaignDeadline.RunWeek'; }

# shellcheck source=golden-revert-disable-batch16-tip.sh
source "$REPO/tools/golden-revert-disable-batch16-tip.sh"
disable_batch16_at_tip
# shellcheck source=golden-revert-disable-batch17-tip.sh
source "$REPO/tools/golden-revert-disable-batch17-tip.sh"
disable_batch17_at_tip

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