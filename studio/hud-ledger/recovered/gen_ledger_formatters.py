#!/usr/bin/env python3
"""Insert formatter methods before BankruptLine."""
import sys
from pathlib import Path

HUD = Path("/agent/repos/ai-labs-tycoon/unity/Assets/Scripts/Hq/HqDisplayQueriesHud.cs")


def body(name, parts):
    decl = "\n".join(f"            string {v} = {call};" for v, call in parts)
    names = [p[0] for p in parts]
    empty = " && ".join(f"string.IsNullOrEmpty({n})" for n in names)
    first = names[0]
    joins = []
    for n in names[1:]:
        joins.append(
            f"""            if (!string.IsNullOrEmpty({n}))
                line = string.IsNullOrEmpty(line) ? {n} : line + " · " + {n};"""
        )
    join_block = "\n".join(joins)
    return f"""
        public static string {name}(LabState lab)
        {{
{decl}
            if ({empty})
                return "";
            string line = "";
            if (!string.IsNullOrEmpty({first})) line = {first};
{join_block}
            return line;
        }}
"""


def insert(formatters):
    t = HUD.read_text()
    block = ""
    for name, parts in formatters:
        if name in t:
            continue
        block += body(name, parts)
    if not block:
        print("nothing to insert")
        return
    t = t.replace(
        "        public static string BankruptLine(LabState lab)",
        block + "\n        public static string BankruptLine(LabState lab)",
        1,
    )
    HUD.write_text(t)
    print(f"inserted {len(formatters)} formatters")


if __name__ == "__main__":
    # format: name|var:Call,var:Call,...
    batch = []
    for line in sys.stdin:
        line = line.strip()
        if not line or line.startswith("#"):
            continue
        name, rest = line.split("|", 1)
        parts = []
        for chunk in rest.split(","):
            v, call = chunk.split(":", 1)
            parts.append((v, call))
        batch.append((name, parts))
    insert(batch)