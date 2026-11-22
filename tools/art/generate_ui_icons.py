#!/usr/bin/env python3
"""Procedural SVG HUD icons (CC0, in-repo only)."""
from __future__ import annotations

import json
from pathlib import Path

REPO_ROOT = Path(__file__).resolve().parents[2]
OUT = REPO_ROOT / "unity" / "Assets" / "Art" / "Generated" / "ui-icons"
SIZE = 64


def svg_wrap(body: str, name: str) -> str:
    return (
        f'<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 {SIZE} {SIZE}" '
        f'role="img" aria-label="{name}">\n'
        f'  <rect width="{SIZE}" height="{SIZE}" rx="8" fill="#14171D"/>\n'
        f"{body}\n</svg>\n"
    )


def icon_cash() -> str:
    body = (
        '  <circle cx="32" cy="32" r="20" fill="none" stroke="#F4F0E8" stroke-width="3"/>'
        '  <text x="32" y="38" text-anchor="middle" fill="#F4F0E8" font-size="22" '
        'font-family="sans-serif">$</text>'
    )
    return svg_wrap(body, "cash")


def icon_calendar() -> str:
    body = (
        '  <rect x="14" y="18" width="36" height="30" rx="4" fill="#5A8CF2"/>'
        '  <rect x="14" y="18" width="36" height="10" fill="#3A6CD2"/>'
        '  <rect x="22" y="34" width="8" height="8" fill="#F4F0E8"/>'
        '  <rect x="34" y="34" width="8" height="8" fill="#F4F0E8"/>'
    )
    return svg_wrap(body, "calendar")


def icon_compute() -> str:
    body = (
        '  <rect x="16" y="20" width="32" height="28" rx="2" fill="#8C66F2"/>'
        '  <rect x="20" y="26" width="24" height="4" fill="#1A1D24"/>'
        '  <rect x="20" y="34" width="24" height="4" fill="#1A1D24"/>'
        '  <rect x="20" y="42" width="24" height="4" fill="#1A1D24"/>'
    )
    return svg_wrap(body, "compute")


def icon_research() -> str:
    body = (
        '  <circle cx="32" cy="28" r="10" fill="none" stroke="#66BF73" stroke-width="3"/>'
        '  <rect x="38" y="36" width="6" height="18" rx="2" transform="rotate(35 41 45)" fill="#66BF73"/>'
    )
    return svg_wrap(body, "research")


def icon_rivals() -> str:
    body = (
        '  <polygon points="32,16 44,28 32,48 20,28" fill="#F2808C"/>'
        '  <circle cx="32" cy="28" r="6" fill="#14171D"/>'
    )
    return svg_wrap(body, "rivals")


def icon_world() -> str:
    body = (
        '  <circle cx="32" cy="32" r="18" fill="#5A8CF2"/>'
        '  <ellipse cx="32" cy="32" rx="18" ry="8" fill="none" stroke="#F4F0E8" stroke-width="2"/>'
        '  <ellipse cx="32" cy="32" rx="8" ry="18" fill="none" stroke="#F4F0E8" stroke-width="2"/>'
    )
    return svg_wrap(body, "world")


def icon_training() -> str:
    body = (
        '  <rect x="18" y="22" width="28" height="6" rx="2" fill="#8C66F2"/>'
        '  <rect x="18" y="32" width="22" height="6" rx="2" fill="#6A46D0"/>'
        '  <rect x="18" y="42" width="14" height="6" rx="2" fill="#4A26B0"/>'
    )
    return svg_wrap(body, "training")


def icon_hq() -> str:
    body = (
        '  <polygon points="32,14 48,26 48,46 16,46 16,26" fill="#F2BF5A"/>'
        '  <rect x="26" y="32" width="12" height="14" fill="#14171D"/>'
    )
    return svg_wrap(body, "hq")


ICONS = {
    "icon-cash": icon_cash,
    "icon-calendar": icon_calendar,
    "icon-compute": icon_compute,
    "icon-research": icon_research,
    "icon-rivals": icon_rivals,
    "icon-world": icon_world,
    "icon-training": icon_training,
    "icon-hq": icon_hq,
}


def main() -> None:
    OUT.mkdir(parents=True, exist_ok=True)
    entries = []
    for icon_id, fn in ICONS.items():
        path = OUT / f"{icon_id}.svg"
        path.write_text(fn(), encoding="utf-8")
        entries.append({"id": icon_id, "path": f"Assets/Art/Generated/ui-icons/{icon_id}.svg"})
    manifest = REPO_ROOT / "unity" / "Assets" / "Art" / "Generated" / "ui-icons-manifest.json"
    manifest.write_text(json.dumps({"icons": entries}, indent=2) + "\n", encoding="utf-8")
    print(f"Wrote {len(entries)} SVG icons to {OUT}")


if __name__ == "__main__":
    main()