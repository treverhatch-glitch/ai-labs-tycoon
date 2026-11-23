#!/usr/bin/env python3
"""Extract /tmp/*.py sprint runners from a bc-93b5443a-style transcript.json into recovered/."""
import json
import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
OUT = Path(__file__).resolve().parent / "recovered"


def main() -> None:
    transcript = Path(sys.argv[1] if len(sys.argv) > 1 else "")
    if not transcript.is_file():
        print("usage: extract_transcript_runners.py /path/to/transcript.json", file=sys.stderr)
        sys.exit(1)
    data = json.loads(transcript.read_text())
    files: dict[str, str] = {}

    def walk(o):
        if isinstance(o, dict):
            ds = o.get("diffString")
            if isinstance(ds, str) and ds.startswith("--- /dev/null") and "+++ b//tmp/" in ds:
                name = ds.split("+++ b//tmp/", 1)[1].split("\n", 1)[0]
                if not name.endswith(".py"):
                    return
                lines = [l[1:] for l in ds.split("\n") if l.startswith("+") and not l.startswith("+++")]
                if len(lines) > 5:
                    prev = files.get(name)
                    if prev is None or len(lines) > len(prev.splitlines()):
                        files[name] = "\n".join(lines)
            for v in o.values():
                walk(v)
        elif isinstance(o, list):
            for x in o:
                walk(x)

    walk(data)
    OUT.mkdir(parents=True, exist_ok=True)
    for name in sorted(files):
        (OUT / name).write_text(files[name])
    print(f"wrote {len(files)} files under {OUT}")


if __name__ == "__main__":
    main()