#!/bin/bash
set -euo pipefail
N="${1:?schema step 125-129}"
REPO="$(cd "$(dirname "$0")/.." && pwd)"
cd "$REPO"

declare -A GOLDEN
GOLDEN[125]=1f6f96b71f669ce3c0ef3a7efa141eed7e51b568a9f623b7d5afad295fed7194
GOLDEN[126]=cafae9f5e3d1e9f9369ffd6f6638816a81ba757df1b3477fadd4cba58a0021c9
GOLDEN[127]=5fbc25faa867e70cd74ae198065e8c84d4d3360ab5f8f1db7a36ac151538023b
GOLDEN[128]=26b039ac6cae3e569b80f7fc27b8084297c5f25501e5db6665bf5cfcf804c034
GOLDEN[129]=ce0c133c14c614b17bf2b76a39a16f8ba5e910194264f59806be9c4c68f3cb2b

PREV=$((N - 1))
BK="/tmp/b12-rev-bak-$N"
mkdir -p "$BK"
for f in Rules/State/GameState.cs Rules/Sim/Simulation.cs Rules/Systems/Agency.cs Rules/Systems/SafetySystem.cs \
  Rules/Sim/CommandProcessor.cs Rules/Data/default-config.json tools/SimTests/SaveAndDeterminismTests.cs; do
  cp "$f" "$BK/$(echo $f | tr '/' '_')"
done
restore() {
  for f in Rules/State/GameState.cs Rules/Sim/Simulation.cs Rules/Systems/Agency.cs Rules/Systems/SafetySystem.cs \
    Rules/Sim/CommandProcessor.cs Rules/Data/default-config.json tools/SimTests/SaveAndDeterminismTests.cs; do
    cp "$BK/$(echo $f | tr '/' '_')" "$REPO/$f"
  done
}
trap restore EXIT

sed -i "s/CurrentSchema = [0-9]*;/CurrentSchema = $PREV;/" Rules/State/GameState.cs
strip_sync() { sed -i -E "/$1/d" Rules/State/GameState.cs; }
disable_cfg() {
  python3 -c "
import json
p='Rules/Data/default-config.json'
with open(p) as f: d=json.load(f)
if '$1' in d: d['$1']['enabled']=0
with open(p,'w') as f: json.dump(d,f,indent=2); f.write(chr(10))
"
}
comment_hook() { sed -i "s|^[[:space:]]*$1|                    // $1|" Rules/Sim/Simulation.cs; }

[ "$N" -le 129 ] && strip_sync 's\.Int\("chkDone"|s\.Int\("chkMissed"'
[ "$N" -le 128 ] && strip_sync 's\.Int\("humanQueue"|s\.Int\("humanBusy"|s\.Int\("humanCleared"'
[ "$N" -le 127 ] && strip_sync 's\.Int\("toolMask"'
[ "$N" -le 126 ] && strip_sync 's\.Int\("evalSuite"|s\.Int\("evalLeft"|s\.Int\("evalScore"|s\.Int\("evalRuns"|s\.Int\("evalStreak"'
[ "$N" -le 125 ] && strip_sync 's\.Int\("drillPrep"|s\.Int\("drillLeft"|s\.Int\("drillDue"|s\.Int\("drillPassed"|s\.Int\("drillFailed"'

[ "$N" -le 129 ] && { disable_cfg releaseChecklist; comment_hook 'ReleaseChecklist.RunWeek'; \
  sed -i '/model.Status == ModelStatus.Trained && ReleaseChecklist.Blocks/,+1 s|^|//|' Rules/Sim/CommandProcessor.cs; \
  sed -i 's|ReleaseChecklist.OnFirstRelease(c, state, model);|// ReleaseChecklist.OnFirstRelease(c, state, model);|' Rules/Sim/CommandProcessor.cs; }
[ "$N" -le 128 ] && { disable_cfg humanQueue; comment_hook 'HumanQueue.RunWeek'; \
  sed -i 's|HumanQueue.Accrue(c, state, lab, m);|// HumanQueue.Accrue(c, state, lab, m);|' Rules/Systems/Agency.cs; \
  sed -i 's| \+ HumanQueue.IncidentAdd(c, lab)||' Rules/Systems/SafetySystem.cs; }
[ "$N" -le 127 ] && disable_cfg toolPolicy
[ "$N" -le 126 ] && { disable_cfg evalHarness; comment_hook 'EvalHarness.RunWeek'; \
  sed -i 's| \+ ToolPolicy.IncidentExtra(c, model)||' Rules/Systems/Agency.cs; \
  sed -i 's|baseChance += ToolPolicy.BlowupExtra(c, model);|// baseChance += ToolPolicy.BlowupExtra(c, model);|' Rules/Systems/Agency.cs; }
[ "$N" -le 125 ] && { disable_cfg drills; comment_hook 'Drills.RunWeek'; \
  sed -i 's|before = (long)MathI.Max(0, before - Drills.IncidentCutPermille(c, lab));||' Rules/Systems/SafetySystem.cs; }

if [ "$N" -le 126 ]; then
  sed -i 's|total += MathI.MulPermille(MarketSystem.ModelRevenue(state, lab.Id, m.Id), MathI.MulPermille(Lift(c, m.AgentLevel), HumanQueue.LiftKeep(c, lab) \* ToolPolicy.LiftBonus(c, m) / 1000));|total += MathI.MulPermille(MarketSystem.ModelRevenue(state, lab.Id, m.Id), MathI.MulPermille(Lift(c, m.AgentLevel), MathI.Permille));|' Rules/Systems/Agency.cs
elif [ "$N" -eq 127 ]; then
  sed -i 's|total += MathI.MulPermille(MarketSystem.ModelRevenue(state, lab.Id, m.Id), MathI.MulPermille(Lift(c, m.AgentLevel), HumanQueue.LiftKeep(c, lab) \* ToolPolicy.LiftBonus(c, m) / 1000));|total += MathI.MulPermille(MarketSystem.ModelRevenue(state, lab.Id, m.Id), MathI.MulPermille(Lift(c, m.AgentLevel), ToolPolicy.LiftBonus(c, m)));|' Rules/Systems/Agency.cs
fi

EXP="${GOLDEN[$N]}"
sed -i "s/const string Golden = \"[0-9a-f]*\"/const string Golden = \"$EXP\"/" tools/SimTests/SaveAndDeterminismTests.cs
cd tools/SimTests
OUT=$(dotnet test --filter "FullyQualifiedName~The_golden_run" 2>&1) || true
echo "$OUT" | grep -E "Passed!|Failed!|New digest" | head -3
if echo "$OUT" | grep -q "Passed!"; then
  echo "schema $N revert: Passed!"
  exit 0
fi
echo "schema $N revert: FAILED"
exit 1