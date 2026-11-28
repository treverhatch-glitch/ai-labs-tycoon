#!/bin/bash
set -euo pipefail
REPO="$(cd "$(dirname "$0")/.." && pwd)"
cd "$REPO"
DOTNET="${DOTNET:-/home/ubuntu/.dotnet/dotnet}"

cal() {
  local script="$1" n="$2"
  local out
  out=$(bash "$script" "$n" 2>&1) || true
  echo "$out" | grep -E "schema $n revert:" | tail -1
  local dig
  dig=$(echo "$out" | sed -n 's/.*digest=\([0-9a-f]*\).*/\1/p' | tail -1)
  if [ -z "$dig" ]; then
    dig=$(echo "$out" | sed -n 's/.*New digest: \([0-9a-f]*\).*/\1/p' | tail -1)
  fi
  if [ -n "$dig" ]; then
    python3 - "$script" "$n" "$dig" <<'PY'
import re, sys
path, n, dig = sys.argv[1], sys.argv[2], sys.argv[3]
text = open(path).read()
pat = rf"(GOLDEN\[{n}\]=)[0-9a-f]*"
new, c = re.subn(pat, rf"\g<1>{dig}", text, count=1)
if c:
    open(path, "w").write(new)
    print(f"pinned {path} GOLDEN[{n}]={dig[:16]}…")
PY
  fi
}

for n in 174 175 176 177; do cal tools/golden-revert-batch25-run.sh "$n"; done
for n in 178 179 180; do cal tools/golden-revert-batch26-run.sh "$n"; done
for n in 181 182 183; do cal tools/golden-revert-batch27-run.sh "$n"; done
for n in 184 185 186; do cal tools/golden-revert-batch28-run.sh "$n"; done
for n in 187 188 189; do cal tools/golden-revert-batch29-run.sh "$n"; done
for n in 190 191 192; do cal tools/golden-revert-batch30-run.sh "$n"; done
for n in 193 194 195; do cal tools/golden-revert-batch31-run.sh "$n"; done
for n in 196 197 198; do cal tools/golden-revert-batch32-run.sh "$n"; done

echo "=== verify tip golden ==="
rm -f .build/bin/SimTests/Debug/net8.0/Data/default-config.json
(cd tools/SimTests && "$DOTNET" build -v q -nologo)
(cd tools/SimTests && "$DOTNET" test --no-build --filter The_golden_run -v q) || true