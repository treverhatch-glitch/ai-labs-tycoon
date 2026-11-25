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
GOLDEN[149]=bad2e9cec5dfa06b73ce1c4244c7f3440faa70c7179729f4381392d0e2619057
GOLDEN[150]=549ceb4f6cb55b6da2a79db0793598156039e87f1c12bd70151f125efa2d9bd2
GOLDEN[151]=2f483cd5f92d0f579aa10ff391431e59b2852707fa0edb04b02af853b3c8a84b
GOLDEN[152]=5385d133ccb0aa1208f2807e56cf282cc025295e5019d53e99001b243e355b16
GOLDEN[153]=6eb981b19629017bd02d40dde1aeb8dabb44c113289e60f3536168031ae557cd

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