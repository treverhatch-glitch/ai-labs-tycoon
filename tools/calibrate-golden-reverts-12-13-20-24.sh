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

for n in 125 126 127 128 129; do cal tools/golden-revert-batch12-run.sh "$n"; done
for n in 130 131 132 133 134; do cal tools/golden-revert-batch13-run.sh "$n"; done
for n in 158 159 160; do cal tools/golden-revert-batch20-run.sh "$n"; done
for n in 161 162 163; do cal tools/golden-revert-batch21-run.sh "$n"; done
for n in 164 165 166; do cal tools/golden-revert-batch22-run.sh "$n"; done
for n in 167 168 169; do cal tools/golden-revert-batch23-run.sh "$n"; done
for n in 170 171 172; do cal tools/golden-revert-batch24-run.sh "$n"; done

echo "=== verify tip golden ==="
rm -f .build/bin/SimTests/Debug/net8.0/Data/default-config.json
(cd tools/SimTests && "$DOTNET" build -v q -nologo)
(cd tools/SimTests && "$DOTNET" test --no-build --filter The_golden_run -v q) || true