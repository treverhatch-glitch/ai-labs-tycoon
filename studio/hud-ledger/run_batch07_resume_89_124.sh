#!/usr/bin/env bash
set -euo pipefail
ROOT="$(cd "$(dirname "$0")/../.." && pwd)"
cd "$ROOT"
export PATH="${HOME}/.dotnet:${PATH}"
export GIT_AUTHOR_NAME='treverhatch-glitch'
export GIT_AUTHOR_EMAIL='342607508+treverhatch-glitch@users.noreply.github.com'
export GIT_COMMITTER_NAME="$GIT_AUTHOR_NAME"
export GIT_COMMITTER_EMAIL="$GIT_COMMITTER_EMAIL"
LEDGER="$ROOT/studio/hud-ledger"
REC="$LEDGER/recovered"
FIX="$LEDGER/fix_staff_mascot_tail_chain.py"
MAKE="$LEDGER/materialize_make_runner.py"

# Finish sprint 88 wires if 85-88 batch died mid-cycle (idempotent runners).
if [[ -f "$REC/run_sprints_85_88.py" ]]; then
  echo "=== resume run_sprints_85_88.py ==="
  python3 "$REC/run_sprints_85_88.py" || true
  python3 "$FIX"
fi

for make in make_sprints_89_92.py make_sprints_93_96.py make_sprints_97_100.py \
  make_sprints_101_104.py make_sprints_105_108.py make_sprints_109_112.py make_sprints_113_116.py \
  make_sprints_117_120.py make_sprints_121_124.py; do
  echo "=== $make ==="
  python3 "$MAKE" "$make"
  run="${make/make_sprints_/run_sprints_}"
  python3 "$REC/$run"
  python3 "$FIX"
done
dotnet test tools/SimTests/SimTests.csproj --filter 'FullyQualifiedName~golden' -v q
echo "EditMode tests:" "$(grep -c '\[Test\]' unity/Assets/Editor/Tests/HqDisplayQueriesHudTests.cs || true)"