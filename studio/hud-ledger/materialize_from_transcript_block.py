#!/usr/bin/env python3
"""Materialize run_sprints_LO_HI.py from transcript F{LO}_{LO+1} new_block snippet."""
import json
import re
import shutil
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
REC = ROOT / "studio/hud-ledger/recovered"
TRANSCRIPT = Path(
    "/tmp/cursor/cloud-agent-transcripts/2026-11-23T19-33-33Z-8894/"
    "bc-93b5443a-60d6-54d1-bb46-9dd867bcaa70/transcript.json"
)


def walk(o, needle: str):
    if isinstance(o, dict):
        cmd = o.get("command") or ""
        if needle in cmd and "new_block" in cmd:
            yield cmd
        for v in o.values():
            yield from walk(v, needle)
    elif isinstance(o, list):
        for x in o:
            yield from walk(x, needle)


def materialize(lo: int, prev_lo: int) -> Path:
    hi = lo + 3
    needle = f"F{lo}_{lo + 1}"
    out = REC / f"run_sprints_{lo}_{hi}.py"
    prev = REC / f"run_sprints_{prev_lo}_{prev_lo + 3}.py"
    if not prev.is_file():
        raise SystemExit(f"missing template {prev}")
    data = json.loads(TRANSCRIPT.read_text())
    cmds = list(walk(data, needle))
    if not cmds:
        raise SystemExit(f"transcript block {needle} not found")
    cmd = max(cmds, key=len)
    shutil.copy(prev, out)
    m = re.search(r"python3 <<'PY'\n(.*)\nPY", cmd, re.DOTALL)
    if not m:
        m = re.search(r"python3 <<'ENDSCRIPT'\n(.*)\nENDSCRIPT", cmd, re.DOTALL)
    if not m:
        raise SystemExit("heredoc not found")
    py = m.group(1)
    py = py.replace(f'Path("/tmp/run_sprints_{prev_lo}_{prev_lo + 3}.py")', f'Path("{prev}")')
    py = py.replace(f'Path("/tmp/run_sprints_{lo}_{hi}.py")', f'Path("{out}")')
    py = py.replace(f'run_sprints_{prev_lo}_{prev_lo + 3}', f'run_sprints_{lo}_{hi}')
    exec(py, {"Path": Path})
    if not out.is_file():
        raise SystemExit(f"failed to write {out}")
    return out


def main() -> int:
    if len(sys.argv) != 3:
        print("usage: materialize_from_transcript_block.py 61 57", file=sys.stderr)
        return 1
    lo, prev = int(sys.argv[1]), int(sys.argv[2])
    p = materialize(lo, prev)
    print(p)
    return 0


if __name__ == "__main__":
    sys.exit(main())