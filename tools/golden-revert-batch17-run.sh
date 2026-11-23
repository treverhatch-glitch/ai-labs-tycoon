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
GOLDEN[149]=45a2624bb2b6050b0cf19fd2b62ea7974319851a4e94f4bedf17cf1ecc4e87e1
GOLDEN[150]=c1891ce34df311a08174236dc14591a5bdd4019280d02ad3f4b528e537ec3e1a
GOLDEN[151]=ab08e3c739bdf6e1975df782c7b808eae431335dadb70847c121f17c9fb5cecd
GOLDEN[152]=6835d1c8706cf7ef032dd105f1de29baada60028f1b0cd3a121535f958e1da50
GOLDEN[153]=470da77b1e6d001a6d024f1154f58d2ead553f253ed787bb2378f65e327aa40d

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