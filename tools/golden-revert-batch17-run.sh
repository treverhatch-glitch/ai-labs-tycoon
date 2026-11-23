#!/bin/bash
set -euo pipefail
N="${1:?schema step 149-153}"
REPO="$(cd "$(dirname "$0")/.." && pwd)"
cd "$REPO"
LOCK="${TMPDIR:-/tmp}/b17-golden-revert.lock"
exec 9>"$LOCK"
flock -x -w 600 9
DOTNET="${DOTNET:-/home/ubuntu/.dotnet/dotnet}"
export PATH="${PATH:-/home/ubuntu/.dotnet:/usr/bin}"
(cd "$REPO/tools/SimTests" && "$DOTNET" build -v q -nologo) || true

declare -A GOLDEN
GOLDEN[149]=41d7ddac50ade9c6f5c34afc97285e3a943c53ece1d99dc25376305f75fbbcc3
GOLDEN[150]=41d7ddac50ade9c6f5c34afc97285e3a943c53ece1d99dc25376305f75fbbcc3
GOLDEN[151]=41d7ddac50ade9c6f5c34afc97285e3a943c53ece1d99dc25376305f75fbbcc3
GOLDEN[152]=a43322c15bc05528c0359d17ecffaa194bfc3fc4cefc46e19c7c778383ab43bd
GOLDEN[153]=a43322c15bc05528c0359d17ecffaa194bfc3fc4cefc46e19c7c778383ab43bd

PREV=$((N - 1))
BK="/tmp/b17-rev-bak-$N"
mkdir -p "$BK"
for f in Rules/State/GameState.cs Rules/Sim/CommandProcessor.Compute.cs Rules/Sim/CommandProcessor.cs Rules/Sim/CommandProcessor.Shady.cs Rules/Systems/DecisionLog.cs Rules/Systems/EndingSystem.cs Rules/Data/default-config.json tools/SimTests/SaveAndDeterminismTests.cs; do
  cp "$f" "$BK/$(echo $f | tr '/' '_')"
done
restore() {
  for f in Rules/State/GameState.cs Rules/Sim/CommandProcessor.Compute.cs Rules/Sim/CommandProcessor.cs Rules/Sim/CommandProcessor.Shady.cs Rules/Systems/DecisionLog.cs Rules/Systems/EndingSystem.cs Rules/Data/default-config.json tools/SimTests/SaveAndDeterminismTests.cs; do
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

[ "$N" -le 149 ] && {
  disable_cfg reservedComputeGate
  sed -i 's/^\([[:space:]]*\)if (!ReservedComputeGate.CanSign/\1\/\/ if (!ReservedComputeGate.CanSign/' Rules/Sim/CommandProcessor.Compute.cs
  sed -i 's/^\([[:space:]]*\)ReservedComputeGate.OnSigned/\1\/\/ ReservedComputeGate.OnSigned/' Rules/Sim/CommandProcessor.Compute.cs
}
[ "$N" -le 150 ] && { disable_cfg rivalIntentNews; sed -i 's/^\([[:space:]]*\)if (state != null) RivalIntentNews.OnDecision/\1\/\/ if (state != null) RivalIntentNews.OnDecision/' Rules/Systems/DecisionLog.cs; }
[ "$N" -le 151 ] && {
  disable_cfg gpuTierUnlock
  sed -i 's/^\([[:space:]]*\)if (!GpuTierUnlock.Unlocked/\1\/\/ if (!GpuTierUnlock.Unlocked/' Rules/Sim/CommandProcessor.cs Rules/Sim/CommandProcessor.Compute.cs
  sed -i 's/^\([[:space:]]*\)return GpuPurchaseBlocked/\1\/\/ return GpuPurchaseBlocked/' Rules/Sim/CommandProcessor.cs Rules/Sim/CommandProcessor.Compute.cs
}
[ "$N" -le 152 ] && {
  disable_cfg endlessScenario
  sed -i 's/^\([[:space:]]*\)if (mode == "keep-going" && record.EndingId == EndlessScenario.WinEndingId)/\1\/\/ if (mode == "keep-going" && record.EndingId == EndlessScenario.WinEndingId)/' Rules/Systems/EndingSystem.cs
  python3 -c "
import json, os
p='Rules/Data/default-config.json'
with open(p) as f: d=json.load(f)
for e in d['endings']['endings']:
    if e.get('id')=='scenario-win':
        e['continueModes']=[x for x in e.get('continueModes',[]) if x!='keep-going']
with open(p+'.tmp','w') as f: json.dump(d,f,indent=2); f.write(chr(10))
os.replace(p+'.tmp',p)
"
}
[ "$N" -le 153 ] && { disable_cfg shadyFreezeAudit; sed -i 's/^\([[:space:]]*\)if (ShadyFreezeAudit.Enabled/\1\/\/ if (ShadyFreezeAudit.Enabled/' Rules/Sim/CommandProcessor.Shady.cs; }

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