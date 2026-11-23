#!/usr/bin/env bash
set -euo pipefail
ROOT="$(cd "$(dirname "$0")/../.." && pwd)"
cd "$ROOT"
export PATH="${HOME}/.dotnet:${PATH}"
export GIT_AUTHOR_NAME='treverhatch-glitch'
export GIT_AUTHOR_EMAIL='342607508+treverhatch-glitch@users.noreply.github.com'
export GIT_COMMITTER_NAME="$GIT_AUTHOR_NAME"
export GIT_COMMITTER_EMAIL="$GIT_COMMITTER_EMAIL"
REC="$ROOT/studio/hud-ledger/recovered"
for n in 3 4 5 6 7 8; do
  python3 "$REC/sprint${n}_wire.py" || true
done
python3 "$REC/run_sprints_9_10.py" || true
python3 "$REC/run_sprints_11_12.py" || true
dotnet test tools/SimTests/SimTests.csproj --filter 'FullyQualifiedName~golden' -v q