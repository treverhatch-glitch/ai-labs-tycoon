#!/usr/bin/env python3
"""Verify founder hero blobs match freeze commit b5c67733 (paths present in that tree)."""
from __future__ import annotations

import subprocess
import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
FREEZE = "b5c67733"
KEY_PATHS = [
    "unity/Assets/Art/EarlyGame/staff-kit/staff-founder-idle.glb",
    "unity/Assets/Art/EarlyGame/staff-kit/staff-founder-idle.asset.json",
    "unity/Assets/Art/EarlyGame/staff-kit/textures/staff-founder-idle/staff-founder-idle_albedo.png",
    "unity/Assets/Art/EarlyGame/staff-kit/textures/staff-founder-idle/staff-founder-idle_normal.png",
    "unity/Assets/Art/EarlyGame/staff-kit/textures/staff-founder-idle/staff-founder-idle_roughness.png",
    "docs/art/previews/staff-founder-walk-strip.png",
    "docs/art/previews/staff-founder-idle.png",
    "unity/Assets/Art/EarlyGame/staff-manifest.json",
]


def _blob_at(ref: str, path: str) -> str | None:
    r = subprocess.run(
        ["git", "rev-parse", f"{ref}:{path}"],
        cwd=REPO,
        capture_output=True,
        text=True,
    )
    if r.returncode != 0:
        return None
    return r.stdout.strip()


def _blob_worktree(path: str) -> str | None:
    fp = REPO / path
    if not fp.is_file():
        return None
    r = subprocess.run(
        ["git", "hash-object", str(fp)],
        cwd=REPO,
        capture_output=True,
        text=True,
    )
    if r.returncode != 0:
        return None
    return r.stdout.strip()


def main() -> int:
    fail = 0
    for path in KEY_PATHS:
        want = _blob_at(FREEZE, path)
        if want is None:
            print(f"SKIP {path} (missing at {FREEZE})")
            continue
        have = _blob_worktree(path)
        if have is None:
            print(f"FAIL {path} missing at HEAD")
            fail += 1
            continue
        if have != want:
            print(f"FAIL {path} blob HEAD={have[:12]} want {FREEZE}={want[:12]}")
            fail += 1
        else:
            print(f"OK {path} blob {have}")
    return fail


if __name__ == "__main__":
    raise SystemExit(main())