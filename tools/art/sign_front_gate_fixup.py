#!/usr/bin/env python3
"""Front preview: template FOR SALE inside board + NCC/clip margins (preview only)."""
from __future__ import annotations

import sys
from pathlib import Path

# Reuse board-inner composite (exported helper).
from sign_composite_front_preview import composite_front  # noqa: E402


def main() -> int:
    if len(sys.argv) < 2:
        print("usage: sign_front_gate_fixup.py <front.png>", file=sys.stderr)
        return 1
    composite_front(Path(sys.argv[1]))
    print("OK", sys.argv[1])
    return 0


if __name__ == "__main__":
    sys.path.insert(0, str(Path(__file__).resolve().parent))
    raise SystemExit(main())