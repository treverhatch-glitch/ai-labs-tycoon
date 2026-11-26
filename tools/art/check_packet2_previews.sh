#!/usr/bin/env bash
set -euo pipefail
ROOT="$(cd "$(dirname "$0")/../.." && pwd)"
cd "$ROOT"
IDS=()
while IFS= read -r id; do
  IDS+=("$id")
done < <(python3 -c "
import json
from pathlib import Path
root = Path('.')
staff = [a['id'] for a in json.loads((root/'unity/Assets/Art/EarlyGame/staff-manifest.json').read_text())['assets']]
wisp = ['wisp-happy','wisp-neutral','wisp-worried','wisp-angry','wisp-glowing']
hud = ['hud-topbar-beige','tex-hud-paper-warm']
for i in staff + wisp + hud:
    print(i)
")
python3 tools/art/check_early_quality_previews.py "${IDS[@]}"
python3 tools/art/check_staff_quality_previews.py