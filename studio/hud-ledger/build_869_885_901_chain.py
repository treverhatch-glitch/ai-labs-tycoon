#!/usr/bin/env python3
"""Mirror 853→869→885→901 from transcript heredocs; writes into studio/hud-ledger/recovered/."""
from __future__ import annotations

import json
import re
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
REC = ROOT / "studio/hud-ledger/recovered"
TRANSCRIPT = Path(
    "/tmp/cursor/cloud-agent-transcripts/2026-11-23T19-33-33Z-8894/"
    "bc-93b5443a-60d6-54d1-bb46-9dd867bcaa70/transcript.json"
)


def find_mirror_cmd(data, needle: str) -> str:
    best = ""

    def walk(o):
        nonlocal best
        if isinstance(o, dict):
            c = o.get("command") or ""
            if needle in c and ("python3 <<'PY'" in c or "python3 << 'PY'" in c):
                if len(c) > len(best):
                    best = c
            for v in o.values():
                walk(v)
        elif isinstance(o, list):
            for x in o:
                walk(x)

    walk(data)
    if not best:
        raise SystemExit(f"transcript mirror {needle!r} not found")
    return best


def exec_heredoc(cmd: str) -> None:
    m = re.search(r"python3 << ['\"]PY['\"]\n(.*)\nPY", cmd, re.DOTALL)
    if not m:
        raise SystemExit("PY heredoc not found")
    py = m.group(1)
    py = py.replace('"/tmp/', f'"{REC}/')
    py = py.replace("'/tmp/", f"'{REC}/")
    py = py.replace('Path("/tmp/', f'Path("{REC}/')
    py = py.replace("Path('/tmp/", f"Path('{REC}/")
    exec(py, {"Path": Path, "re": re})


def main() -> int:
    if not (REC / "sprint853_856_data.py").is_file() or (REC / "sprint853_856_data.py").read_text().count("SPRINT853") < 1:
        print(
            "sprint853_856_data.py missing or incomplete — replay toward sprint ~853 or drop "
            "a full data file before running 869→901 mirrors.",
            file=sys.stderr,
        )
        return 2
    if not (REC / "run_sprints_853_856.py").is_file():
        print("run_sprints_853_856.py missing — materialize from data first.", file=sys.stderr)
        return 2
    data = json.loads(TRANSCRIPT.read_text())
    for label, needle, out_data, out_run in [
        ("869", "sprint869_872_data", "sprint869_872_data.py", "run_sprints_869_872.py"),
        ("885", "sprint885_888_data", "sprint885_888_data.py", "run_sprints_885_888.py"),
        ("901", "sprint901_904_data", "sprint901_904_data.py", "run_sprints_901_904.py"),
    ]:
        cmd = find_mirror_cmd(data, needle)
        exec_heredoc(cmd)
        if not (REC / out_data).is_file() or not (REC / out_run).is_file():
            raise SystemExit(f"mirror {label} failed to write {out_data} / {out_run}")
        print(f"ok {label}: {out_data}, {out_run}")
    return 0


if __name__ == "__main__":
    sys.exit(main())