#!/usr/bin/env python3
"""Dial-up era HUD icons and panel chrome (CC0 SVG)."""
from __future__ import annotations

import json
from pathlib import Path

REPO_ROOT = Path(__file__).resolve().parents[2]
OUT = REPO_ROOT / "unity" / "Assets" / "Art" / "EarlyGame" / "hud-icons"
SIZE = 64


def svg_wrap(body: str, name: str, bg: str = "#14171D") -> str:
    return (
        f'<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 {SIZE} {SIZE}" '
        f'role="img" aria-label="{name}">\n'
        f'  <rect width="{SIZE}" height="{SIZE}" rx="6" fill="{bg}"/>\n'
        f"{body}\n</svg>\n"
    )


def icon_cash_era0() -> str:
    body = (
        '  <rect x="10" y="18" width="44" height="28" rx="4" fill="#C87830" stroke="#F4F0E8" stroke-width="2"/>'
        '  <text x="32" y="38" text-anchor="middle" fill="#F4F0E8" font-size="20" font-family="sans-serif">$</text>'
    )
    return svg_wrap(body, "cash dial-up", "#2A2218")


def icon_date_era0() -> str:
    body = (
        '  <rect x="12" y="14" width="40" height="36" rx="3" fill="#C8B8A0"/>'
        '  <rect x="12" y="14" width="40" height="12" fill="#A85828"/>'
        '  <text x="32" y="40" text-anchor="middle" fill="#1A1D24" font-size="14" font-family="monospace">95</text>'
    )
    return svg_wrap(body, "date dial-up")


def icon_era_bar() -> str:
    body = (
        '  <rect x="8" y="28" width="48" height="8" rx="2" fill="#3A3A44"/>'
        '  <rect x="8" y="28" width="14" height="8" rx="2" fill="#A85828"/>'
        '  <text x="32" y="22" text-anchor="middle" fill="#F4F0E8" font-size="10" font-family="sans-serif">ERA</text>'
    )
    return svg_wrap(body, "era bar")


def icon_build_panel() -> str:
    body = (
        '  <polygon points="16,48 32,16 48,48" fill="#F2BF5A"/>'
        '  <rect x="22" y="38" width="20" height="10" fill="#1A1D24"/>'
    )
    return svg_wrap(body, "build")


def icon_hire_panel() -> str:
    body = (
        '  <circle cx="32" cy="22" r="8" fill="#F4F0E8"/>'
        '  <path d="M18 50c0-10 28-10 28 0v4H18z" fill="#66BF73"/>'
        '  <rect x="40" y="36" width="10" height="10" fill="#A85828"/>'
    )
    return svg_wrap(body, "hire")


def icon_train_panel() -> str:
    body = (
        '  <rect x="14" y="20" width="36" height="6" rx="2" fill="#8C66F2"/>'
        '  <rect x="14" y="30" width="28" height="6" rx="2" fill="#6A46D0"/>'
        '  <rect x="14" y="40" width="20" height="6" rx="2" fill="#4A26B0"/>'
    )
    return svg_wrap(body, "train")


def icon_gpu_hire() -> str:
    body = (
        '  <rect x="18" y="18" width="28" height="32" rx="2" fill="#5A5A66"/>'
        '  <rect x="22" y="24" width="20" height="5" fill="#8C66F2"/>'
        '  <rect x="22" y="32" width="20" height="5" fill="#66BF73"/>'
    )
    return svg_wrap(body, "gpu")


def icon_week_advance() -> str:
    body = (
        '  <polygon points="20,32 44,32 36,22 44,32 36,42" fill="#4088D9"/>'
        '  <rect x="14" y="26" width="8" height="12" fill="#F4F0E8" opacity="0.4"/>'
    )
    return svg_wrap(body, "advance week", "#1A1D24")


def panel_build_bg() -> str:
    return svg_wrap(
        '  <rect x="4" y="4" width="56" height="56" rx="8" fill="#2A2218" stroke="#A85828" stroke-width="2"/>',
        "build panel",
        "#14171D",
    )


def panel_hire_bg() -> str:
    return svg_wrap(
        '  <rect x="4" y="4" width="56" height="56" rx="8" fill="#1E2420" stroke="#66BF73" stroke-width="2"/>',
        "hire panel",
        "#14171D",
    )


def panel_train_bg() -> str:
    return svg_wrap(
        '  <rect x="4" y="4" width="56" height="56" rx="8" fill="#1A1824" stroke="#8C66F2" stroke-width="2"/>',
        "train panel",
        "#14171D",
    )


def hud_topbar_beige() -> str:
    return (
        f'<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 256 48" role="img" aria-label="hud bar">\n'
        f'  <rect width="256" height="48" fill="#14171D" opacity="0.92"/>\n'
        f'  <rect x="0" y="44" width="256" height="4" fill="#A85828"/>\n'
        f"</svg>\n"
    )


BUILDERS: dict[str, callable] = {
    "icon-cash-era0": icon_cash_era0,
    "icon-date-era0": icon_date_era0,
    "icon-era-bar": icon_era_bar,
    "icon-build-panel": icon_build_panel,
    "icon-hire-panel": icon_hire_panel,
    "icon-train-panel": icon_train_panel,
    "icon-gpu-hire": icon_gpu_hire,
    "icon-week-advance": icon_week_advance,
    "panel-build-bg": panel_build_bg,
    "panel-hire-bg": panel_hire_bg,
    "panel-train-bg": panel_train_bg,
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
                "path": f"Assets/Art/EarlyGame/hud-icons/{asset_id}.svg",
                "license": "CC0-1.0 (procedural, in-repo)",
            }
        )
    bar_path = OUT / "hud-topbar-beige.svg"
    bar_path.write_text(hud_topbar_beige(), encoding="utf-8")
    entries.append(
        {
            "id": "hud-topbar-beige",
            "path": "Assets/Art/EarlyGame/hud-icons/hud-topbar-beige.svg",
            "license": "CC0-1.0 (procedural, in-repo)",
        }
    )
    manifest = REPO_ROOT / "unity" / "Assets" / "Art" / "EarlyGame" / "hud-manifest.json"
    manifest.write_text(
        json.dumps(
            {
                "generator": "tools/art/generate_early_hud_icons.py",
                "fidelity": "packet-3-4",
                "assets": entries,
            },
            indent=2,
        )
        + "\n",
        encoding="utf-8",
    )
    print(f"Wrote {len(entries)} early HUD SVGs")


if __name__ == "__main__":
    main()