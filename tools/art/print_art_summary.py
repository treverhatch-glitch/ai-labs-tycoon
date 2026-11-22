#!/usr/bin/env python3
"""One-line art gate summary for GO packets."""
import subprocess
import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]


def main() -> int:
    r = subprocess.run([str(REPO / "tools/art/prepush.sh")], cwd=REPO, capture_output=True, text=True)
    oks = [ln for ln in r.stdout.splitlines() if ln.startswith("OK:")]
    if r.returncode != 0:
        print(r.stdout, file=sys.stderr)
        print(r.stderr, file=sys.stderr)
        return r.returncode
    print(" | ".join(oks))
    return 0


if __name__ == "__main__":
    sys.exit(main())