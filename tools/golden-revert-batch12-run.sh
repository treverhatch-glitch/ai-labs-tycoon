#!/bin/bash
set -euo pipefail
N="${1:?schema step 125-129}"
REPO="$(cd "$(dirname "$0")/.." && pwd)"
cd "$REPO"
LOCK="${TMPDIR:-/tmp}/b12-golden-revert.lock"
exec 9>"$LOCK"
flock -x -w 600 9
bash "$REPO/tools/restore-golden-tip.sh" >/dev/null
DOTNET="${DOTNET:-$(command -v dotnet)}"
export PATH="${PATH:-/home/ubuntu/.dotnet:/usr/bin}"
(cd "$REPO/tools/SimTests" && "$DOTNET" build -v q -nologo) || true

declare -A GOLDEN
GOLDEN[125]=a02fd101a39954ba24ca296a154ae97e52a3b2fa5d546c26d0050925236cd9fe
GOLDEN[126]=2debb2710173b4fb639d5d8c88707d752fa55bcdef8d39c1d49f293e48065b6d
GOLDEN[127]=df6af8a854d83cfb048952dc9c54eb1b517d74f70efb8c1bed195f8544255815
GOLDEN[128]=fd25659126ed48bf369bea521263958427a13030d25e3029ea80251997b49da7
GOLDEN[129]=55c5c356e024838979a242dca3af659a103b8af258bae658fc75d491edb1a883

PREV=$((N - 1))
BK="/tmp/b12-rev-bak-$N"
mkdir -p "$BK"
for f in Rules/State/GameState.cs Rules/Sim/Simulation.cs Rules/Systems/Agency.cs Rules/Systems/SafetySystem.cs \
  Rules/Systems/MarketSystem.cs Rules/Systems/ToolPolicy.cs Rules/Sim/CommandProcessor.cs Rules/Sim/CommandProcessor.ToolPolicy.cs \
  Rules/Systems/TrainingSystem.cs Rules/Systems/DisplayQueries.cs Rules/Systems/EraRules.cs Rules/Data/default-config.json tools/SimTests/SaveAndDeterminismTests.cs; do
  cp "$f" "$BK/$(echo $f | tr '/' '_')"
done
export PATH="${PATH:-/home/ubuntu/.dotnet:/usr/bin}"
restore() {
  for f in Rules/State/GameState.cs Rules/Sim/Simulation.cs Rules/Systems/Agency.cs Rules/Systems/SafetySystem.cs \
    Rules/Systems/MarketSystem.cs Rules/Systems/ToolPolicy.cs Rules/Sim/CommandProcessor.cs Rules/Sim/CommandProcessor.ToolPolicy.cs \
    Rules/Systems/TrainingSystem.cs Rules/Systems/DisplayQueries.cs Rules/Systems/EraRules.cs Rules/Data/default-config.json tools/SimTests/SaveAndDeterminismTests.cs; do
    cp "$BK/$(echo $f | tr '/' '_')" "$REPO/$f"
  done
  (cd "$REPO/tools/SimTests" && "$DOTNET" build -v q -nologo) >/dev/null || true
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

# batch fourteen must be off at tip when reverting batch twelve
for _b14 in specialistLines dataSources openWeights sectorPricing; do disable_cfg "$_b14"; done
comment_hook 'DataSources.RunWeek'
comment_hook 'OpenWeights.RunWeek'
sed -i 's| + SectorPricing.LabOverloadPermille(c, lab)||' Rules/Systems/SafetySystem.cs
sed -i '/if (SectorPricing.Enabled(c) && demandLab.Id == state.PlayerLabId)/,+1 s|^|//|' Rules/Systems/MarketSystem.cs
sed -i 's|int src = DataSources.QualityPermille(c, lab) / 50;||' Rules/Systems/TrainingSystem.cs
sed -i 's|model.CapLuck += src;||' Rules/Systems/TrainingSystem.cs

# shellcheck source=golden-revert-disable-batch15-tip.sh
source "$REPO/tools/golden-revert-disable-batch15-tip.sh"
disable_batch15_at_tip
# shellcheck source=golden-revert-disable-batch16-tip.sh
source "$REPO/tools/golden-revert-disable-batch16-tip.sh"
disable_batch16_at_tip

if [ "$N" -le 126 ]; then
  sed -i 's|total += MathI.MulPermille(MarketSystem.ModelRevenue(state, lab.Id, m.Id), MathI.MulPermille(lift, HumanQueue.LiftKeep(c, lab) \* ToolPolicy.LiftBonus(c, m) / 1000));|total += MathI.MulPermille(MarketSystem.ModelRevenue(state, lab.Id, m.Id), MathI.MulPermille(Lift(c, m.AgentLevel), MathI.Permille));|' Rules/Systems/Agency.cs
elif [ "$N" -eq 127 ]; then
  sed -i 's|total += MathI.MulPermille(MarketSystem.ModelRevenue(state, lab.Id, m.Id), MathI.MulPermille(lift, HumanQueue.LiftKeep(c, lab) \* ToolPolicy.LiftBonus(c, m) / 1000));|total += MathI.MulPermille(MarketSystem.ModelRevenue(state, lab.Id, m.Id), MathI.MulPermille(Lift(c, m.AgentLevel), ToolPolicy.LiftBonus(c, m)));|' Rules/Systems/Agency.cs
else
  sed -i 's|total += MathI.MulPermille(MarketSystem.ModelRevenue(state, lab.Id, m.Id), MathI.MulPermille(lift, HumanQueue.LiftKeep(c, lab) \* ToolPolicy.LiftBonus(c, m) / 1000));|total += MathI.MulPermille(MarketSystem.ModelRevenue(state, lab.Id, m.Id), MathI.MulPermille(Lift(c, m.AgentLevel), HumanQueue.LiftKeep(c, lab) * ToolPolicy.LiftBonus(c, m) / 1000));|' Rules/Systems/Agency.cs
fi

EXP="${GOLDEN[$N]}"
sed -i "s/const string Golden = \"[0-9a-f]*\"/const string Golden = \"$EXP\"/" tools/SimTests/SaveAndDeterminismTests.cs
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