#!/usr/bin/env python3
"""Build sprint901_904_data + run_sprints_901_904 from transcript (needs sprint869 chain on disk)."""
import json
import re
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
REC = ROOT / "studio/hud-ledger/recovered"
TRANSCRIPT = Path(
    "/tmp/cursor/cloud-agent-transcripts/2026-11-23T19-33-33Z-8894/"
    "bc-93b5443a-60d6-54d1-bb46-9dd867bcaa70/transcript.json"
)


def find_cmd(data, needle: str):
    def walk(o):
        if isinstance(o, dict):
            c = o.get("command") or ""
            if needle in c:
                yield c
            for v in o.values():
                yield from walk(v)
        elif isinstance(o, list):
            for x in o:
                yield from walk(x)

    return max(walk(data), key=len, default="")


def run_py(cmd: str, label: str) -> None:
    m = re.search(r"python3 << ['\"]PY['\"]\n(.*)\nPY", cmd, re.DOTALL)
    if not m:
        raise SystemExit(f"{label}: PY heredoc not found")
    py = m.group(1)
    py = py.replace('"/tmp/', f'"{REC}/')
    py = py.replace("'/tmp/", f"'{REC}/")
    py = py.replace("Path(\"/tmp/", f"Path(\"{REC}/")
    exec(py, {"Path": Path, "re": re})


def main() -> None:
    data = json.loads(TRANSCRIPT.read_text())
    if not (REC / "sprint869_872_data.py").is_file():
        # mirror from highest spend-income data we have (161_164 as bootstrap is wrong)
        # require sprint865+ data from transcript — skip if missing
        raise SystemExit("sprint869_872_data.py missing; extract from transcript first")
    if not (REC / "run_sprints_869_872.py").is_file():
        raise SystemExit("run_sprints_869_872.py missing")
    cmd885 = find_cmd(data, "sprint885_888_data")
    if cmd885:
        run_py(cmd885, "885")
    cmd901 = find_cmd(data, "sprint901_904_data")
    if not cmd901:
        raise SystemExit("901 transcript command not found")
    run_py(cmd901, "901")
    print("wrote", REC / "sprint901_904_data.py", REC / "run_sprints_901_904.py")


if __name__ == "__main__":
    main()