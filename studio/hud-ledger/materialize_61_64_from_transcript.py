#!/usr/bin/env python3
"""Rebuild run_sprints_61_64.py from bc-93b5443a transcript shell snippet."""
import json
import re
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
REC = ROOT / "studio/hud-ledger/recovered"
TRANSCRIPT = Path(
    "/tmp/cursor/cloud-agent-transcripts/2026-11-23T19-33-33Z-8894/"
    "bc-93b5443a-60d6-54d1-bb46-9dd867bcaa70/transcript.json"
)
TEMPLATE = REC / "run_sprints_57_60.py"
OUT = REC / "run_sprints_61_64.py"


def walk(o):
    if isinstance(o, dict):
        cmd = o.get("command") or ""
        if "F61_62" in cmd and "F63_64" in cmd and "new_block" in cmd:
            yield cmd
        for v in o.values():
            yield from walk(v)
    elif isinstance(o, list):
        for x in o:
            yield from walk(x)


def main() -> None:
    data = json.loads(TRANSCRIPT.read_text())
    cmds = list(walk(data))
    if not cmds:
        raise SystemExit("F61_62 new_block command not found in transcript")
    cmd = max(cmds, key=len)
    # extract python between python3 <<'PY' and closing PY (last heredoc in command)
    m = re.search(r"python3 <<'PY'\n(.*)\nPY\s*$", cmd, re.DOTALL)
    if not m:
        raise SystemExit("heredoc not found")
    ns: dict = {"Path": Path}
    exec(m.group(1), ns)
    if not OUT.exists():
        raise SystemExit(f"expected {OUT} after transcript exec")
    print(f"materialized {OUT}")


if __name__ == "__main__":
    main()