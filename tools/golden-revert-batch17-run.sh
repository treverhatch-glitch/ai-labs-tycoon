#!/bin/bash
set -euo pipefail
N="${1:?schema step 149-153}"
REPO="$(cd "$(dirname "$0")/.." && pwd)"
cd "$REPO"
LOCK="${TMPDIR:-/tmp}/b17-golden-revert.lock"
exec 9>"$LOCK"
flock -x -w 600 9
bash "$REPO/tools/restore-golden-tip.sh" >/dev/null
DOTNET="${DOTNET:-$(command -v dotnet)}"
export PATH="${PATH:-/home/ubuntu/.dotnet:/usr/bin}"
(cd "$REPO/tools/SimTests" && "$DOTNET" build -v q -nologo) || true

declare -A GOLDEN
GOLDEN[149]=251123df1e277cb217093bb2c40c3e5c5d6dd127984383047588907547127558
GOLDEN[150]=3d730523f634c455053ca72fd144fe8bebc836d6be8522e5c97e0d4a253e7e20
GOLDEN[151]=e7794a15e3645e44b8b1d71964b36bd5758d4236380b8a5f52e0187e8e6a249e
GOLDEN[152]=676fa692dbadb2853bf07d34b5cc053fe721b454b23fc6e87ff7884d4cb8e2d0
GOLDEN[153]=dda6abea805efa874fdb0b14fdaaa4af3a06acfaf6540630ede9a5efc2ca331f

PREV=$((N - 1))
BK="/tmp/b17-rev-bak-$N"
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

[ "$N" -le 153 ] && disable_cfg shadyFreezeAudit
[ "$N" -le 152 ] && {
  disable_cfg endlessScenario
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
[ "$N" -le 151 ] && disable_cfg gpuTierUnlock
[ "$N" -le 150 ] && disable_cfg rivalIntentNews
[ "$N" -le 149 ] && disable_cfg reservedComputeGate
[ "$N" -le 155 ] && disable_cfg sectorPricingBreakdown
[ "$N" -le 154 ] && disable_cfg specialistEraHooks

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