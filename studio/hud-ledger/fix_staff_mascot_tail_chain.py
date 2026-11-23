#!/usr/bin/env python3
"""Rebuild StaffGpuPanel / HqMascotStripHud summary tails from declared *Part vars."""
from __future__ import annotations

import re
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
STAFF = ROOT / "unity/Assets/Scripts/Hq/StaffGpuPanel.cs"
MASCOT = ROOT / "unity/Assets/Scripts/Hq/HqMascotStripHud.cs"


def refresh_block(text: str) -> tuple[str, int, int]:
    m = re.search(r"\n        void Refresh\(\)\s*\n        \{", text)
    if not m:
        raise SystemExit("Refresh() not found")
    start = m.end()
    depth = 1
    i = start
    while i < len(text) and depth:
        if text[i] == "{":
            depth += 1
        elif text[i] == "}":
            depth -= 1
        i += 1
    return text[start : i - 1], start, i - 1


def part_vars(block: str) -> list[str]:
    return re.findall(
        r"^\s+string (\w+Part) = string\.IsNullOrEmpty",
        block,
        flags=re.MULTILINE,
    )


def rewrite_assign(block: str, marker: str, base: str) -> str:
    parts = part_vars(block)
    if not parts:
        raise SystemExit(f"No *Part vars before {marker}")
    tail = "".join(f" + {p}" for p in parts)
    new_line = f"            {marker} = {base}{tail};"
    pat = re.compile(rf"^\s+{re.escape(marker)} = .+;$", re.MULTILINE)
    if not pat.search(block):
        raise SystemExit(f"{marker} assignment missing in Refresh")
    return pat.sub(new_line, block, count=1)


def apply(path: Path, marker: str, base: str) -> list[str]:
    text = path.read_text()
    block, start, end = refresh_block(text)
    parts = part_vars(block)
    new_block = rewrite_assign(block, marker, base)
    path.write_text(text[:start] + new_block + text[end:])
    return parts


def main() -> int:
    staff = apply(
        STAFF,
        "_gpuSummary.text",
        '"GPUs: " + rented + " rented, " + owned + " owned (" + gen.Name + ")"',
    )
    mascot = apply(MASCOT, "_line.text", '"Mascots"')
    print(f"staff tail: {len(staff)} parts, last={staff[-1] if staff else 'n/a'}")
    print(f"mascot tail: {len(mascot)} parts, last={mascot[-1] if mascot else 'n/a'}")
    return 0


if __name__ == "__main__":
    sys.exit(main())