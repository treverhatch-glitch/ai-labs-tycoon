#!/bin/bash
set -euo pipefail
N="${1:?schema step 130-134}"
REPO="$(cd "$(dirname "$0")/.." && pwd)"
cd "$REPO"
LOCK="${TMPDIR:-/tmp}/b13-golden-revert.lock"
exec 9>"$LOCK"
flock -x -w 600 9
bash "$REPO/tools/restore-golden-tip.sh" >/dev/null
(cd "$REPO/tools/SimTests" && dotnet build -v q -nologo) || true

declare -A GOLDEN
GOLDEN[130]=7726b17363ed81897f702060a1ebbe6407dae14e402ee97461bcc09a8d52b53a
GOLDEN[131]=881ac19a824906cb852509449ef81e7c84383a97d41f51b240462c43b19a91ea
GOLDEN[132]=dd2648e2582adb77b05e55578debcb75d84fd26fbc225adda3ad0a80c17b529e
GOLDEN[133]=5f46a3cf0d37f08e152e6b148b4a1e441a605673d7239bb5ed5eebde80daed8b
GOLDEN[134]=01ff4afd5c52582b701526a060c79e10b2ff1d33987a9f640ce7ee846fdc48d7

PREV=$((N - 1))
BK="/tmp/b13-rev-bak-$N"
mkdir -p "$BK"
for f in Rules/State/GameState.cs Rules/Sim/Simulation.cs Rules/Systems/Agency.cs Rules/Systems/SafetySystem.cs \
  Rules/Systems/MarketSystem.cs Rules/Systems/ToolPolicy.cs Rules/Sim/CommandProcessor.cs Rules/Sim/CommandProcessor.ToolPolicy.cs \
  Rules/Systems/TrainingSystem.cs Rules/Systems/DisplayQueries.cs Rules/Systems/EraRules.cs Rules/Data/default-config.json tools/SimTests/SaveAndDeterminismTests.cs; do
  cp "$f" "$BK/$(echo $f | tr '/' '_')"
done
restore() {
  for f in Rules/State/GameState.cs Rules/Sim/Simulation.cs Rules/Systems/Agency.cs Rules/Systems/SafetySystem.cs \
    Rules/Systems/MarketSystem.cs Rules/Systems/ToolPolicy.cs Rules/Sim/CommandProcessor.cs Rules/Sim/CommandProcessor.ToolPolicy.cs \
    Rules/Systems/TrainingSystem.cs Rules/Systems/DisplayQueries.cs Rules/Systems/EraRules.cs Rules/Data/default-config.json tools/SimTests/SaveAndDeterminismTests.cs; do
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

[ "$N" -le 134 ] && strip_sync 's\.Int\("pubOn"|s\.Int\("pubScore"|s\.Int\("pubMiss"'
[ "$N" -le 133 ] && strip_sync 's\.Int\("routePpm"'
[ "$N" -le 132 ] && strip_sync 's\.Int\("orchLevel"|s\.Int\("orchOver"'
[ "$N" -le 131 ] && strip_sync 's\.Int\("playLevel"|s\.Int\("playArmed"|s\.Int\("playLeft"|s\.Int\("playArmUntil"'
[ "$N" -le 130 ] && strip_sync 's\.Int\("sandLeft"|s\.Int\("sandGrad"'

[ "$N" -le 134 ] && { disable_cfg publicMetrics; comment_hook 'PublicMetrics.RunWeek'; \
  sed -i 's|PublicMetrics.OnIncident(c, state, lab, model);|// PublicMetrics.OnIncident(c, state, lab, model);|' Rules/Systems/SafetySystem.cs; }
[ "$N" -le 133 ] && { disable_cfg trafficRouter; \
  sed -i 's| \+ TrafficRouter.IncidentAdd(c, state, lab)||' Rules/Systems/SafetySystem.cs; \
  perl -0pi -e 's/if \(TrafficRouter\.Enabled\(c\) && demandLab\.Id == state\.PlayerLabId\) \{[^}]+\}\n\s+//ms' Rules/Systems/MarketSystem.cs; }
[ "$N" -le 132 ] && { disable_cfg orchestrationBudget; comment_hook 'OrchestrationBudget.RunWeek'; \
  sed -i 's|int lift = (int)MathI.MulPermille(Lift(c, m.AgentLevel), OrchestrationBudget.KeepPermille(c, lab));|int lift = Lift(c, m.AgentLevel);|' Rules/Systems/Agency.cs; }
[ "$N" -le 131 ] && { disable_cfg runawayPlaybook; comment_hook 'RunawayPlaybook.RunWeek'; \
  sed -i 's|int playCut = RunawayPlaybook.BlowupCutPermille(c, lab);||' Rules/Systems/Agency.cs; \
  sed -i 's|, MathI.Permille - cut - playCut|, MathI.Permille - cut|' Rules/Systems/Agency.cs; \
  sed -i 's|int trust = -c.Agency.BlowupTrust;||' Rules/Systems/Agency.cs; \
  sed -i 's|int keep = RunawayPlaybook.TrustKeepPermille(c, lab);||' Rules/Systems/Agency.cs; \
  sed -i 's|if (keep > 0) trust = (int)MathI.MulPermille(trust, MathI.Permille - keep);||' Rules/Systems/Agency.cs; \
  sed -i 's|TrustSystem.Shift(c, state, lab, m.Sectors\[s\], trust, false);|TrustSystem.Shift(c, state, lab, m.Sectors[s], -c.Agency.BlowupTrust, false);|' Rules/Systems/Agency.cs; }
[ "$N" -le 130 ] && { disable_cfg agentSandbox; comment_hook 'AgentSandbox.RunWeek'; \
  sed -i '/AgentSandbox.Enabled(c) && model.AgentLevel > 0 && model.SandGrad != 1 &&/,+1 s|^|//|' Rules/Sim/CommandProcessor.ToolPolicy.cs; \
  sed -i '/if (AgentSandbox.Enabled(c) && model.AgentLevel > 0 && model.SandGrad != 1)/,/mask &= Web;/ s|^|//|' Rules/Systems/ToolPolicy.cs; }

# batch fourteen must be off at tip when reverting batch thirteen
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
# shellcheck source=golden-revert-disable-batch17-tip.sh
source "$REPO/tools/golden-revert-disable-batch17-tip.sh"
disable_batch17_at_tip

EXP="${GOLDEN[$N]}"
export PATH="${PATH:-/home/ubuntu/.dotnet:/usr/bin}"
if [ -z "$EXP" ]; then
  cd tools/SimTests
  dotnet build -v q -nologo
  OUT=$(dotnet test --no-build --filter "FullyQualifiedName~The_golden_run" 2>&1) || true
  NEW=$(echo "$OUT" | sed -n 's/.*New digest: \([0-9a-f]*\).*/\1/p' | head -1)
  if [ -z "$NEW" ]; then NEW=$(echo "$OUT" | sed -n 's/.*digest moved\. New digest: \([0-9a-f]*\).*/\1/p' | head -1); fi
  echo "schema $N record digest: ${NEW:-unknown}"
  exit 0
fi
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