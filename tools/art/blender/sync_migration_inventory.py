#!/usr/bin/env python3
"""Mark EarlyGame rows rebuilt when Blender .glb/.png primary outputs exist."""
from __future__ import annotations

import re
from pathlib import Path

REPO = Path(__file__).resolve().parents[3]
MIGRATION = REPO / "docs/art/BLENDER-MIGRATION.md"
EARLY = REPO / "unity/Assets/Art/EarlyGame"


def glb_for_stem(stem: str) -> Path | None:
    for kit in ("garage-kit", "staff-kit"):
        p = EARLY / kit / f"{stem}.glb"
        if p.exists():
            return p
    return None


def main() -> int:
    text = MIGRATION.read_text(encoding="utf-8")
    lines = text.splitlines()
    out: list[str] = []
    changed = 0
    for line in lines:
        if not line.startswith("| `unity/Assets/Art/EarlyGame/"):
            out.append(line)
            continue
        m = re.match(r"\| `([^`]+)` \| early \| ([^|]+) \| ([^|]+) \| ([^|]+) \|", line)
        if not m:
            out.append(line)
            continue
        path, kind, refs, status = m.group(1), m.group(2).strip(), m.group(3).strip(), m.group(4).strip()
        if status != "todo":
            out.append(line)
            continue
        stem = Path(path).stem
        new_status = status
        new_path = path
        if kind == "mesh" and path.endswith(".obj"):
            glb = glb_for_stem(stem)
            if glb:
                new_path = f"unity/Assets/Art/EarlyGame/{glb.parent.name}/{glb.name}"
                new_status = "rebuilt"
        if kind == "icon" and "wisp" in path and (EARLY / "wisp-moods" / f"{stem}-sheet.png").exists():
            new_status = "rebuilt"
        if kind == "icon" and "hud-icons" in path and (EARLY / "hud-icons" / f"{stem}.png").exists():
            new_status = "rebuilt"
        if kind == "texture" and (EARLY / "textures" / f"{stem}.png").exists():
            tex_norm = EARLY / "textures" / f"{stem}_normal.png"
            tex_rough = EARLY / "textures" / f"{stem}_roughness.png"
            alb = EARLY / "textures" / f"{stem}.png"
            if tex_rough.exists() or (tex_norm.exists() and tex_norm.stat().st_size > 50000):
                new_status = "rebuilt"
            elif alb.exists() and alb.stat().st_size > 120000:
                new_status = "rebuilt"
        if new_status == status and new_path == path:
            out.append(line)
            continue
        note = line.split("|", 6)[-1] if line.count("|") >= 6 else ""
        newline = f"| `{new_path}` | early | {kind} | {refs} | {new_status} |{note}"
        out.append(newline)
        changed += 1
    if changed:
        header = out
        for i, ln in enumerate(header):
            if ln.startswith("**Audited:**"):
                header[i] = "**Audited:** 2026-11-26 15:05 ICT (early Blender lane finish `bc-d48d70eb`)"
                break
        MIGRATION.write_text("\n".join(header) + "\n", encoding="utf-8")
    print(f"OK migration rows updated: {changed}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())