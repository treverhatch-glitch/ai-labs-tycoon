"""Per-asset PBR bake ramps for early-art quality pass (distinct materials, not uniform beige)."""
from __future__ import annotations

# dark/light RGB 0-1, optional emission for screen-like parts
BakeSpec = dict[str, tuple[float, float, float] | float]

PALETTE: dict[str, dict] = {
    "garage-floor-tile": {"dark": (0.38, 0.35, 0.32), "light": (0.52, 0.48, 0.44), "roughness": 0.82, "noise": 14.0},
    "garage-wall-panel": {"dark": (0.72, 0.68, 0.60), "light": (0.88, 0.84, 0.76), "roughness": 0.62, "noise": 9.0},
    "garage-exterior-wall": {"dark": (0.68, 0.64, 0.58), "light": (0.82, 0.78, 0.70), "roughness": 0.58, "noise": 7.5},
    "garage-roll-door": {"dark": (0.35, 0.37, 0.40), "light": (0.62, 0.64, 0.68), "roughness": 0.35, "noise": 11.0},
    "garage-server-closet": {"dark": (0.32, 0.34, 0.38), "light": (0.55, 0.57, 0.62), "roughness": 0.40, "noise": 10.0},
    "garage-land-for-sale-sign": {"dark": (0.55, 0.52, 0.22), "light": (0.92, 0.88, 0.35), "roughness": 0.48, "noise": 6.0},
    "garage-lot-curb": {"dark": (0.42, 0.40, 0.38), "light": (0.58, 0.55, 0.52), "roughness": 0.78, "noise": 16.0},
    "garage-window-frame": {"dark": (0.48, 0.32, 0.18), "light": (0.72, 0.45, 0.22), "roughness": 0.52, "noise": 8.0},
    "garage-desk-crt": {"dark": (0.42, 0.30, 0.18), "light": (0.68, 0.52, 0.32), "roughness": 0.50, "noise": 7.0},
    "garage-crt-tower": {"dark": (0.28, 0.30, 0.34), "light": (0.50, 0.52, 0.58), "roughness": 0.38, "noise": 9.0},
    "garage-voodude-rack": {"dark": (0.55, 0.30, 0.14), "light": (0.78, 0.42, 0.20), "roughness": 0.45, "noise": 10.0},
    "garage-whiteboard-1995": {"dark": (0.88, 0.88, 0.84), "light": (0.98, 0.98, 0.96), "roughness": 0.28, "noise": 5.0},
    "garage-coffee-drip": {"dark": (0.22, 0.22, 0.24), "light": (0.45, 0.45, 0.48), "roughness": 0.32, "noise": 8.0},
    "garage-folding-chair": {"dark": (0.38, 0.38, 0.42), "light": (0.62, 0.62, 0.66), "roughness": 0.55, "noise": 7.0},
    "garage-cable-spool": {"dark": (0.30, 0.28, 0.26), "light": (0.52, 0.48, 0.44), "roughness": 0.60, "noise": 12.0},
    "staff-engineer-work": {"dark": (0.32, 0.38, 0.48), "light": (0.55, 0.62, 0.72), "roughness": 0.58, "noise": 6.0},
}

DEFAULT = {"dark": (0.45, 0.38, 0.32), "light": (0.72, 0.66, 0.58), "roughness": 0.55, "noise": 8.0}


def spec_for(asset_id: str) -> dict:
    return {**DEFAULT, **PALETTE.get(asset_id, {})}