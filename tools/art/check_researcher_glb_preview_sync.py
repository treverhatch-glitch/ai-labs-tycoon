#!/usr/bin/env python3
"""FAIL when researcher GLB bytes change but walk/turnaround PNGs are unchanged vs a ref."""
from __future__ import annotations

import hashlib
import subprocess
import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
GLB = "unity/Assets/Art/EarlyGame/staff-kit/staff-researcher-idle.glb"
WALK = "docs/art/previews/staff-researcher-walk-strip.png"
TURN = "docs/art/previews/staff-researcher-turnaround.png"


def _sha256_bytes(data: bytes) -> str:
    return hashlib.sha256(data).hexdigest()


def _sha256_file(path: Path) -> str:
    return _sha256_bytes(path.read_bytes())


def _sha256_git(ref: str, path: str) -> str | None:
    proc = subprocess.run(
        ["git", "show", f"{ref}:{path}"],
        cwd=REPO,
        capture_output=True,
    )
    if proc.returncode != 0:
        return None
    return _sha256_bytes(proc.stdout)


def main() -> int:
    against = "HEAD^"
    if len(sys.argv) > 1:
        against = sys.argv[1]
    glb_w = REPO / GLB
    walk_w = REPO / WALK
    turn_w = REPO / TURN
    if not glb_w.is_file() or not walk_w.is_file() or not turn_w.is_file():
        print("FAIL researcher_glb_preview_sync missing_asset_files")
        return 1
    glb_ref = _sha256_git(against, GLB)
    walk_ref = _sha256_git(against, WALK)
    turn_ref = _sha256_git(against, TURN)
    if glb_ref is None:
        print(f"SKIP researcher_glb_preview_sync no_glb_at_{against}")
        return 0
    glb_now = _sha256_file(glb_w)
    walk_now = _sha256_file(walk_w)
    turn_now = _sha256_file(turn_w)
    if glb_now == glb_ref:
        print(f"OK researcher_glb_preview_sync glb_unchanged_vs_{against}")
        return 0
    bad: list[str] = []
    if walk_ref is not None and walk_now == walk_ref:
        bad.append(f"walk_strip_unchanged sha={walk_now[:12]}")
    if turn_ref is not None and turn_now == turn_ref:
        bad.append(f"turnaround_unchanged sha={turn_now[:12]}")
    if bad:
        print(f"FAIL researcher_glb_preview_sync glb_changed_vs_{against}: " + "; ".join(bad))
        return 1
    print(
        f"OK researcher_glb_preview_sync glb_changed_vs_{against} "
        f"walk={walk_now[:12]} turn={turn_now[:12]}"
    )
    return 0


if __name__ == "__main__":
    raise SystemExit(main())