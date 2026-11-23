#!/bin/bash
set -euo pipefail
N="${1:?schema step 144-148}"
REPO="$(cd "$(dirname "$0")/.." && pwd)"
cd "$REPO"
LOCK="${TMPDIR:-/tmp}/b16-golden-revert.lock"
exec 9>"$LOCK"
flock -x -w 600 9
bash "$REPO/tools/restore-golden-tip.sh" >/dev/null
DOTNET="${DOTNET:-/home/ubuntu/.dotnet/dotnet}"
export PATH="${PATH:-/home/ubuntu/.dotnet:/usr/bin}"
(cd "$REPO/tools/SimTests" && "$DOTNET" build -v q -nologo) || true

declare -A GOLDEN
GOLDEN[144]=a2d0d0989bae77a01891a37609ae3f511a5dcc22e9d4bb546889461bfddb0e00
GOLDEN[145]=a2d0d0989bae77a01891a37609ae3f511a5dcc22e9d4bb546889461bfddb0e00
GOLDEN[146]=a2d0d0989bae77a01891a37609ae3f511a5dcc22e9d4bb546889461bfddb0e00
GOLDEN[147]=a2d0d0989bae77a01891a37609ae3f511a5dcc22e9d4bb546889461bfddb0e00
GOLDEN[148]=55e4c4f09a3b5dca4a9034c1942c1c7ef2c8884d72a43842caa8cbfec3d8a6a4

PREV=$((N - 1))
BK="/tmp/b16-rev-bak-$N"
mkdir -p "$BK"
for f in Rules/State/GameState.cs Rules/Sim/Simulation.cs Rules/Systems/Finance.cs Rules/Systems/OfficeEvents.cs Rules/Systems/EndingSystem.cs Rules/Sim/WorldSetup.cs Rules/Data/default-config.json tools/SimTests/SaveAndDeterminismTests.cs; do
  cp "$f" "$BK/$(echo $f | tr '/' '_')"
done
restore() {
  for f in Rules/State/GameState.cs Rules/Sim/Simulation.cs Rules/Systems/Finance.cs Rules/Systems/OfficeEvents.cs Rules/Systems/EndingSystem.cs Rules/Sim/WorldSetup.cs Rules/Data/default-config.json tools/SimTests/SaveAndDeterminismTests.cs; do
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

[ "$N" -le 148 ] && { disable_cfg lawTarget; }
[ "$N" -le 147 ] && { disable_cfg scenarioGoals; comment_hook 'ScenarioGoals.RunWeek'; }
[ "$N" -le 146 ] && { disable_cfg rungSixCrisis; comment_hook 'RungSixCrisis.RunWeek'; sed -i 's/^[[:space:]]*HearingSharePrice.OnAnswered/            \/\/ HearingSharePrice.OnAnswered/' Rules/Systems/OfficeEvents.cs; sed -i 's/^[[:space:]]*if (e.Trigger == OfficeTrigger.Escape/            \/\/ if (e.Trigger == OfficeTrigger.Escape/' Rules/Systems/OfficeEvents.cs; sed -i 's/^[[:space:]]*RungSixCrisis.OnArcAnswered/                \/\/ RungSixCrisis.OnArcAnswered/' Rules/Systems/OfficeEvents.cs; }
[ "$N" -le 145 ] && { disable_cfg hearingSharePrice; }
[ "$N" -le 144 ] && { disable_cfg marginCall; sed -i 's|MarginCall.RunWeek(c, state, lab);||' Rules/Systems/Finance.cs; }

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