#!/usr/bin/env python3
"""Dial-up era HUD icons and panel chrome (CC0 SVG). Packet34: transparent ortho glyphs."""
from __future__ import annotations

import argparse
import json
from pathlib import Path

REPO_ROOT = Path(__file__).resolve().parents[2]
OUT = REPO_ROOT / "unity/Assets/Art/EarlyGame/hud-icons"
SIZE = 64

# 1995 garage palette (saturated, 2–3 per icon)
C_ORANGE = "#E87830"
C_TRIM = "#A85828"
C_BEIGE = "#D4A858"
C_PAPER = "#F4E8C8"
C_GREEN = "#3CB86A"
C_PURPLE = "#7A52E8"
C_BLUE = "#4088D9"
C_INK = "#1A1D24"
C_CHIP = "#5A5E68"
C_OUTLINE = "#14171D"


def svg_icon(body: str, name: str, view: int = SIZE) -> str:
    return (
        f'<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 {view} {view}" role="img" aria-label="{name}">\n'
        f"{body}\n</svg>\n"
    )


def icon_cash_era0() -> str:
    body = (
        f'  <rect x="8" y="16" width="48" height="32" rx="5" fill="{C_BEIGE}" stroke="{C_OUTLINE}" stroke-width="3"/>'
        f'  <rect x="12" y="20" width="40" height="8" fill="{C_TRIM}" opacity="0.35"/>'
        f'  <text x="32" y="40" text-anchor="middle" fill="{C_TRIM}" font-size="22" font-weight="bold" '
        f'font-family="Arial,sans-serif">$</text>'
    )
    return svg_icon(body, "cash bill")


def icon_date_era0() -> str:
    body = (
        f'  <rect x="14" y="12" width="36" height="40" rx="4" fill="{C_PAPER}" stroke="{C_OUTLINE}" stroke-width="3"/>'
        f'  <rect x="14" y="12" width="36" height="14" fill="{C_TRIM}"/>'
        f'  <circle cx="22" cy="19" r="2" fill="{C_PAPER}"/><circle cx="42" cy="19" r="2" fill="{C_PAPER}"/>'
        f'  <text x="32" y="38" text-anchor="middle" fill="{C_INK}" font-size="14" font-weight="bold" '
        f'font-family="monospace">26</text>'
    )
    return svg_icon(body, "calendar")


def icon_era_bar() -> str:
    body = (
        f'  <rect x="6" y="26" width="52" height="12" rx="3" fill="{C_CHIP}" stroke="{C_OUTLINE}" stroke-width="2"/>'
        f'  <rect x="8" y="28" width="10" height="8" rx="1" fill="{C_ORANGE}"/>'
        f'  <rect x="22" y="28" width="10" height="8" rx="1" fill="{C_TRIM}"/>'
        f'  <rect x="36" y="28" width="10" height="8" rx="1" fill="{C_BEIGE}"/>'
    )
    return svg_icon(body, "era progress ticks")


def icon_build_panel() -> str:
    body = (
        f'  <rect x="22" y="28" width="20" height="18" fill="{C_TRIM}" stroke="{C_OUTLINE}" stroke-width="2"/>'
        f'  <path d="M18 46 L32 18 L46 46 Z" fill="{C_BEIGE}" stroke="{C_OUTLINE}" stroke-width="3" '
        f'stroke-linejoin="round"/>'
        f'  <rect x="28" y="34" width="8" height="14" fill="{C_ORANGE}" stroke="{C_OUTLINE}" stroke-width="1.5"/>'
    )
    return svg_icon(body, "hammer brick")


def icon_hire_panel() -> str:
    body = (
        f'  <circle cx="32" cy="22" r="9" fill="{C_PAPER}" stroke="{C_OUTLINE}" stroke-width="3"/>'
        f'  <path d="M16 52c0-12 32-12 32 0" fill="{C_GREEN}" stroke="{C_OUTLINE}" stroke-width="2"/>'
        f'  <text x="44" y="42" font-size="18" font-weight="bold" fill="{C_ORANGE}" '
        f'font-family="Arial,sans-serif">+</text>'
    )
    return svg_icon(body, "hire person plus")


def icon_train_panel() -> str:
    body = (
        f'  <rect x="16" y="14" width="32" height="38" rx="6" fill="{C_PAPER}" stroke="{C_OUTLINE}" stroke-width="3"/>'
        f'  <ellipse cx="32" cy="26" rx="10" ry="8" fill="{C_PURPLE}" stroke="{C_OUTLINE}" stroke-width="2"/>'
        f'  <path d="M22 36 h20 v12 h-6 v-6 h-8 v6 h-6 z" fill="{C_BLUE}" stroke="{C_OUTLINE}" stroke-width="1.5"/>'
    )
    return svg_icon(body, "brain book")


def icon_gpu_hire() -> str:
    body = (
        f'  <rect x="16" y="18" width="32" height="30" rx="3" fill="{C_CHIP}" stroke="{C_OUTLINE}" stroke-width="3"/>'
        f'  <rect x="20" y="24" width="8" height="6" fill="{C_PURPLE}"/>'
        f'  <rect x="30" y="24" width="8" height="6" fill="{C_GREEN}"/>'
        f'  <rect x="20" y="34" width="18" height="5" fill="{C_ORANGE}"/>'
        f'  <circle cx="48" cy="28" r="3" fill="{C_TRIM}"/><circle cx="48" cy="38" r="3" fill="{C_TRIM}"/>'
    )
    return svg_icon(body, "gpu chip")


def icon_week_advance() -> str:
    body = (
        f'  <polygon points="22,20 46,32 22,44" fill="{C_BLUE}" stroke="{C_OUTLINE}" stroke-width="3" '
        f'stroke-linejoin="round"/>'
        f'  <polygon points="22,24 38,32 22,40" fill="{C_PAPER}" opacity="0.45"/>'
        f'  <rect x="12" y="22" width="6" height="20" rx="1" fill="{C_TRIM}" stroke="{C_OUTLINE}" stroke-width="1.5"/>'
    )
    return svg_icon(body, "play ff advance")


def panel_build_bg() -> str:
    body = (
        f'  <rect x="10" y="10" width="44" height="44" rx="8" fill="#2A2218" stroke="{C_ORANGE}" stroke-width="3"/>'
        f'  <rect x="14" y="14" width="36" height="10" rx="3" fill="{C_TRIM}"/>'
        f'  <rect x="16" y="26" width="32" height="24" rx="5" fill="#1E1810" stroke="{C_BEIGE}" stroke-width="2"/>'
    )
    return svg_icon(body, "build nine slice")


def panel_hire_bg() -> str:
    body = (
        f'  <rect x="10" y="10" width="44" height="44" rx="8" fill="#142018" stroke="{C_GREEN}" stroke-width="3"/>'
        f'  <rect x="14" y="14" width="36" height="10" rx="3" fill="{C_GREEN}"/>'
        f'  <rect x="16" y="26" width="32" height="24" rx="5" fill="#101814" stroke="#6AD080" stroke-width="2"/>'
    )
    return svg_icon(body, "hire nine slice")


def panel_train_bg() -> str:
    body = (
        f'  <rect x="10" y="10" width="44" height="44" rx="8" fill="#181424" stroke="{C_PURPLE}" stroke-width="3"/>'
        f'  <rect x="14" y="14" width="36" height="10" rx="3" fill="{C_PURPLE}"/>'
        f'  <rect x="16" y="26" width="32" height="24" rx="5" fill="#12101C" stroke="#A888F8" stroke-width="2"/>'
    )
    return svg_icon(body, "train nine slice")


def hud_topbar_beige() -> str:
    return (
        '<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 256 48" role="img" aria-label="hud bar">\n'
        f'  <rect x="12" y="6" width="232" height="36" rx="4" fill="#14171D" stroke="{C_BEIGE}" stroke-width="3"/>\n'
        f'  <rect x="12" y="6" width="232" height="8" rx="4" fill="{C_TRIM}"/>\n'
        f'  <rect x="18" y="18" width="220" height="20" fill="#1E1A14" stroke="{C_ORANGE}" stroke-width="2"/>\n'
        "</svg>\n"
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
    ap = argparse.ArgumentParser()
    ap.add_argument("--packet34", action="store_true", help="Packet34 ortho transparent glyphs")
    args = ap.parse_args()
    _ = args.packet34
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
                "fidelity": "packet34-ortho-glyphs",
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