#!/usr/bin/env python3
"""AI wisp mascot mood SVGs (CC0 procedural)."""
from __future__ import annotations

import json
from pathlib import Path

REPO_ROOT = Path(__file__).resolve().parents[2]
OUT = REPO_ROOT / "unity" / "Assets" / "Art" / "EarlyGame" / "wisp-moods"
SIZE = 64


def svg_wrap(body: str, name: str, glow: str) -> str:
    return (
        f'<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 {SIZE} {SIZE}" '
        f'role="img" aria-label="{name}">\n'
        f'  <rect width="{SIZE}" height="{SIZE}" rx="12" fill="#14171D"/>\n'
        f'  <circle cx="32" cy="32" r="22" fill="{glow}" opacity="0.35"/>\n'
        f"{body}\n</svg>\n"
    )


def wisp_happy() -> str:
    body = (
        '  <ellipse cx="32" cy="36" rx="18" ry="20" fill="#8C66F2"/>'
        '  <circle cx="26" cy="32" r="3" fill="#F4F0E8"/>'
        '  <circle cx="38" cy="32" r="3" fill="#F4F0E8"/>'
        '  <path d="M24 42 Q32 50 40 42" fill="none" stroke="#F4F0E8" stroke-width="2"/>'
    )
    return svg_wrap(body, "wisp happy", "#66BF73")


def wisp_neutral() -> str:
    body = (
        '  <ellipse cx="32" cy="36" rx="18" ry="20" fill="#8C66F2"/>'
        '  <circle cx="26" cy="32" r="3" fill="#F4F0E8"/>'
        '  <circle cx="38" cy="32" r="3" fill="#F4F0E8"/>'
        '  <rect x="26" y="42" width="12" height="2" fill="#F4F0E8"/>'
    )
    return svg_wrap(body, "wisp neutral", "#5A8CF2")


def wisp_worried() -> str:
    body = (
        '  <ellipse cx="32" cy="36" rx="18" ry="20" fill="#8C66F2"/>'
        '  <circle cx="26" cy="30" r="3" fill="#F4F0E8"/>'
        '  <circle cx="38" cy="30" r="3" fill="#F4F0E8"/>'
        '  <path d="M26 46 Q32 40 38 46" fill="none" stroke="#F4F0E8" stroke-width="2"/>'
    )
    return svg_wrap(body, "wisp worried", "#F2BF5A")


def wisp_angry() -> str:
    body = (
        '  <ellipse cx="32" cy="36" rx="18" ry="20" fill="#8C66F2"/>'
        '  <circle cx="26" cy="32" r="3" fill="#E85D4A"/>'
        '  <circle cx="38" cy="32" r="3" fill="#E85D4A"/>'
        '  <path d="M24 44 L40 44" stroke="#E85D4A" stroke-width="3"/>'
    )
    return svg_wrap(body, "wisp angry", "#E85D4A")


def wisp_glowing() -> str:
    body = (
        '  <ellipse cx="32" cy="36" rx="18" ry="20" fill="#B080FF"/>'
        '  <circle cx="26" cy="32" r="4" fill="#F4F0E8"/>'
        '  <circle cx="38" cy="32" r="4" fill="#F4F0E8"/>'
        '  <path d="M24 42 Q32 50 40 42" fill="none" stroke="#F4F0E8" stroke-width="2"/>'
        '  <circle cx="32" cy="32" r="26" fill="none" stroke="#8C66F2" stroke-width="2" opacity="0.6"/>'
    )
    return svg_wrap(body, "wisp glowing", "#8C66F2")


BUILDERS = {
    "wisp-happy": wisp_happy,
    "wisp-neutral": wisp_neutral,
    "wisp-worried": wisp_worried,
    "wisp-angry": wisp_angry,
    "wisp-glowing": wisp_glowing,
}


def main() -> None:
    OUT.mkdir(parents=True, exist_ok=True)
    entries = []
    for asset_id, fn in BUILDERS.items():
        path = OUT / f"{asset_id}.svg"
        path.write_text(fn(), encoding="utf-8")
        entries.append(
            {
                "id": asset_id,
                "path": f"Assets/Art/EarlyGame/wisp-moods/{asset_id}.svg",
                "license": "CC0-1.0 (procedural, in-repo)",
            }
        )
    manifest = REPO_ROOT / "unity" / "Assets" / "Art" / "EarlyGame" / "wisp-manifest.json"
    manifest.write_text(
        json.dumps({"generator": "tools/art/generate_wisp_mascot.py", "assets": entries}, indent=2) + "\n",
        encoding="utf-8",
    )
    print(f"Wrote {len(entries)} wisp SVGs")


if __name__ == "__main__":
    main()