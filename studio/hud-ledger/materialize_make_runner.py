#!/usr/bin/env python3
"""Run recovered/make_sprints_LO_HI.py with repo-local paths (not /tmp)."""
from __future__ import annotations

import re
import runpy
import sys
import tempfile
from pathlib import Path

REC = Path(__file__).resolve().parent / "recovered"


def patch_source(text: str) -> str:
    text = text.replace("/tmp/run_sprints_", str(REC / "run_sprints_"))
    text = re.sub(
        r'Path\(["\']/tmp/run_sprints_([^"\']+)["\']\)'
        r'\.write_text\(out\)',
        f'Path(r"{REC}/run_sprints_\\1").write_text(out)',
        text,
    )
    text = re.sub(
        r'py_compile\.compile\(["\']/tmp/run_sprints_([^"\']+)["\']',
        f'py_compile.compile(r"{REC}/run_sprints_\\1"',
        text,
    )
    return text


def main() -> int:
    if len(sys.argv) != 2:
        print("usage: materialize_make_runner.py make_sprints_69_72.py", file=sys.stderr)
        return 1
    make = REC / sys.argv[1]
    if not make.is_file():
        print(f"missing {make}", file=sys.stderr)
        return 1
    patched = patch_source(make.read_text())
    with tempfile.NamedTemporaryFile("w", suffix=".py", delete=False) as tmp:
        tmp.write(patched)
        path = tmp.name
    try:
        runpy.run_path(path, run_name="__main__")
    finally:
        Path(path).unlink(missing_ok=True)
    m = re.match(r"make_sprints_(\d+)_(\d+)\.py$", make.name)
    if not m:
        return 0
    runner = REC / f"run_sprints_{m.group(1)}_{m.group(2)}.py"
    if not runner.is_file():
        print(f"expected {runner} after make", file=sys.stderr)
        return 1
    print("materialized", runner.name)
    return 0


if __name__ == "__main__":
    sys.exit(main())