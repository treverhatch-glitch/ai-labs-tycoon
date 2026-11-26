#!/usr/bin/env python3
"""Load preview bytes exactly as committed at a git revision (default HEAD)."""
from __future__ import annotations

import io
import subprocess
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]


def git_show_bytes(rev: str, rel_posix: str, *, repo: Path = REPO) -> bytes:
    rel = rel_posix.lstrip("/")
    return subprocess.check_output(["git", "show", f"{rev}:{rel}"], cwd=repo)


def load_rgba_from_git(rel_posix: str, *, rev: str = "HEAD", repo: Path = REPO):
    from PIL import Image

    data = git_show_bytes(rev, rel_posix, repo=repo)
    return Image.open(io.BytesIO(data)).convert("RGBA")


def load_rgb_from_git(rel_posix: str, *, rev: str = "HEAD", repo: Path = REPO):
  return load_rgba_from_git(rel_posix, rev=rev, repo=repo).convert("RGB")