#!/usr/bin/env python3
"""Remove genesis-embedded Python runner tail from HqDisplayQueriesHud.cs."""
from pathlib import Path

HUD = Path(__file__).resolve().parents[2] / "unity/Assets/Scripts/Hq/HqDisplayQueriesHud.cs"

def main() -> None:
    lines = HUD.read_text().splitlines(keepends=True)
    start = None
    end = None
    for i, line in enumerate(lines):
        if start is None and 'public static string {name}(LabState lab)' in line:
            start = i
        if start is not None and line.startswith("        public static string BenchmarkSlaSummaryLine"):
            end = i
            break
    if start is None or end is None:
        raise SystemExit(f"markers not found start={start} end={end}")
    cleaned = lines[:start] + lines[end:]
    # drop duplicate python anchor strings if any remain inside file
    text = "".join(cleaned)
    for needle in (
        '            "        public static string BankruptLine(LabState lab)",\n',
        '            block + "\\n        public static string BankruptLine(LabState lab)",\n',
    ):
        text = text.replace(needle, "")
    HUD.write_text(text)
    print(f"removed lines {start + 1}-{end} ({end - start} lines); now {len(text.splitlines())} lines")


if __name__ == "__main__":
    main()