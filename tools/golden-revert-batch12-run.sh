#!/bin/bash
set -euo pipefail
N="${1:?schema step 125-129}"
REPO="$(cd "$(dirname "$0")/.." && pwd)"
cd "$REPO"
LOCK="${TMPDIR:-/tmp}/b12-golden-revert.lock"
exec 9>"$LOCK"
flock -x -w 600 9

declare -A GOLDEN
GOLDEN[125]=f4c325030695f46d47fdb9ba9b9035375a9db6675bf91119c94527adb56c3c0a
GOLDEN[126]=f72c92807d52957f83b20dedd2cf591ef44188cb362cf8260a948359017f4d24
GOLDEN[127]=b01de421fe9259fe584333eceeaaae1f6bafcf3c2c88c2e9dfff1672c5a02674
GOLDEN[128]=a6992081eb7717d178614724ee6ad8b61caa5ec56153c237ecd8a64c758ace28
GOLDEN[129]=b508c30b01b4de7e20790cae7750df5793725a15ab001331e3340fcca840e071

PREV=$((N - 1))
BK="/tmp/b12-rev-bak-$N"
mkdir -p "$BK"
for f in Rules/State/GameState.cs Rules/Sim/Simulation.cs Rules/Systems/Agency.cs Rules/Systems/SafetySystem.cs \
  Rules/Systems/MarketSystem.cs Rules/Systems/ToolPolicy.cs Rules/Sim/CommandProcessor.cs Rules/Sim/CommandProcessor.ToolPolicy.cs \
  Rules/Data/default-config.json tools/SimTests/SaveAndDeterminismTests.cs; do
  cp "$f" "$BK/$(echo $f | tr '/' '_')"
done
export PATH="${PATH:-/home/ubuntu/.dotnet:/usr/bin}"
restore() {
  for f in Rules/State/GameState.cs Rules/Sim/Simulation.cs Rules/Systems/Agency.cs Rules/Systems/SafetySystem.cs \
    Rules/Systems/MarketSystem.cs Rules/Systems/ToolPolicy.cs Rules/Sim/CommandProcessor.cs Rules/Sim/CommandProcessor.ToolPolicy.cs \
    Rules/Data/default-config.json tools/SimTests/SaveAndDeterminismTests.cs; do
    cp "$BK/$(echo $f | tr '/' '_')" "$REPO/$f"
  done
  (cd "$REPO/tools/SimTests" && dotnet build -v q -nologo) >/dev/null || true
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

# batch thirteen must be off at tip when reverting batch twelve
for _b13 in agentSandbox runawayPlaybook orchestrationBudget trafficRouter publicMetrics; do disable_cfg "$_b13"; done
comment_hook 'AgentSandbox.RunWeek'
comment_hook 'RunawayPlaybook.RunWeek'
comment_hook 'OrchestrationBudget.RunWeek'
comment_hook 'PublicMetrics.RunWeek'
sed -i 's| \+ TrafficRouter.IncidentAdd(c, state, lab)||' Rules/Systems/SafetySystem.cs
sed -i 's|PublicMetrics.OnIncident(c, state, lab, model);|// PublicMetrics.OnIncident(c, state, lab, model);|' Rules/Systems/SafetySystem.cs
sed -i 's|int lift = (int)MathI.MulPermille(Lift(c, m.AgentLevel), OrchestrationBudget.KeepPermille(c, lab));|int lift = Lift(c, m.AgentLevel);|' Rules/Systems/Agency.cs
sed -i 's|int playCut = RunawayPlaybook.BlowupCutPermille(c, lab);||' Rules/Systems/Agency.cs
sed -i 's|, MathI.Permille - cut - playCut|, MathI.Permille - cut|' Rules/Systems/Agency.cs
sed -i 's|int trust = -c.Agency.BlowupTrust;||' Rules/Systems/Agency.cs
sed -i 's|int keep = RunawayPlaybook.TrustKeepPermille(c, lab);||' Rules/Systems/Agency.cs
sed -i 's|if (keep > 0) trust = (int)MathI.MulPermille(trust, MathI.Permille - keep);||' Rules/Systems/Agency.cs
sed -i 's|TrustSystem.Shift(c, state, lab, m.Sectors\[s\], trust, false);|TrustSystem.Shift(c, state, lab, m.Sectors[s], -c.Agency.BlowupTrust, false);|' Rules/Systems/Agency.cs
perl -0pi -e 's/if \(TrafficRouter\.Enabled\(c\) && demandLab\.Id == state\.PlayerLabId\) \{[^}]+\}\n\s+//ms' Rules/Systems/MarketSystem.cs
sed -i '/AgentSandbox.Enabled(c) && model.AgentLevel > 0 && model.SandGrad != 1 &&/,+1 s|^|//|' Rules/Sim/CommandProcessor.ToolPolicy.cs
sed -i '/AgentSandbox.Enabled(c) && model.AgentLevel > 0 && model.SandGrad != 1)/,+1 s/^/\\/\\//' Rules/Systems/ToolPolicy.cs 2>/dev/null || sed -i '/AgentSandbox.Enabled(c) && model.AgentLevel > 0 && model.SandGrad != 1)/,+1 s|^|//|' Rules/Systems/ToolPolicy.cs

if [ "$N" -le 126 ]; then
  sed -i 's|total += MathI.MulPermille(MarketSystem.ModelRevenue(state, lab.Id, m.Id), MathI.MulPermille(lift, HumanQueue.LiftKeep(c, lab) \* ToolPolicy.LiftBonus(c, m) / 1000));|total += MathI.MulPermille(MarketSystem.ModelRevenue(state, lab.Id, m.Id), MathI.MulPermille(Lift(c, m.AgentLevel), MathI.Permille));|' Rules/Systems/Agency.cs
elif [ "$N" -eq 127 ]; then
  sed -i 's|total += MathI.MulPermille(MarketSystem.ModelRevenue(state, lab.Id, m.Id), MathI.MulPermille(lift, HumanQueue.LiftKeep(c, lab) \* ToolPolicy.LiftBonus(c, m) / 1000));|total += MathI.MulPermille(MarketSystem.ModelRevenue(state, lab.Id, m.Id), MathI.MulPermille(Lift(c, m.AgentLevel), ToolPolicy.LiftBonus(c, m)));|' Rules/Systems/Agency.cs
else
  sed -i 's|total += MathI.MulPermille(MarketSystem.ModelRevenue(state, lab.Id, m.Id), MathI.MulPermille(lift, HumanQueue.LiftKeep(c, lab) \* ToolPolicy.LiftBonus(c, m) / 1000));|total += MathI.MulPermille(MarketSystem.ModelRevenue(state, lab.Id, m.Id), MathI.MulPermille(Lift(c, m.AgentLevel), HumanQueue.LiftKeep(c, lab) * ToolPolicy.LiftBonus(c, m) / 1000));|' Rules/Systems/Agency.cs
fi

EXP="${GOLDEN[$N]}"
sed -i "s/const string Golden = \"[0-9a-f]*\"/const string Golden = \"$EXP\"/" tools/SimTests/SaveAndDeterminismTests.cs
export PATH="${PATH:-/home/ubuntu/.dotnet:/usr/bin}"
cd tools/SimTests
dotnet build -v q -nologo
OUT=$(dotnet test --no-build --filter "FullyQualifiedName~The_golden_run" 2>&1) || true
echo "$OUT" | grep -E "Passed!|Failed!|New digest" | head -3
if echo "$OUT" | grep -q "Passed!"; then
  echo "schema $N revert: Passed!"
  exit 0
fi
echo "schema $N revert: FAILED"
exit 1