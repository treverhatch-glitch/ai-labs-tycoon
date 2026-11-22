#!/usr/bin/env python3
"""Print CampusIconCatalog ids (one per line)."""
import re
from pathlib import Path

CS = Path(__file__).resolve().parents[2] / "unity/Assets/Scripts/Hq/CampusIconCatalog.cs"
text = CS.read_text(encoding="utf-8")
block = re.search(r"AllIconIds\s*=\s*\{([^}]+)\}", text, re.S)
ids = re.findall(r'"([^"]+)"', block.group(1)) if block else []
for i in ids:
    print(i)