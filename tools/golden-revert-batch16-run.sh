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
GOLDEN[144]=5ff2995196a68720af1612888d840123ce4a7bd59650173e83c743cfb32fa6a3
GOLDEN[145]=9aea1d33813ae72b35627df0553f1b2c88d14f09843b4a2f9deb6b35b9dfbd50
GOLDEN[146]=073ebffe1a0fe94a2869c2d76a3f407118ddf3dcddd0651d67a68f0493afd684
GOLDEN[147]=ee9a0e134bbed458174deb6a13d392311b2f1c8921d3e7cd9a60773e7477b703
GOLDEN[148]=78a76c64eb38ae969492e10f4acd618ccccb4886c556024475685bf5d687d4b5

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