"""Per-asset PBR bake ramps — saturated identity colors (quality pass packet 1)."""
from __future__ import annotations

PALETTE: dict[str, dict] = {
    "garage-floor-tile": {"dark": (0.38, 0.32, 0.28), "light": (0.62, 0.52, 0.44), "roughness": 0.85, "noise": 14.0},
    "garage-wall-panel": {"dark": (0.62, 0.48, 0.36), "light": (0.86, 0.72, 0.58), "roughness": 0.62, "noise": 9.0},
    "garage-exterior-wall": {"dark": (0.58, 0.46, 0.34), "light": (0.82, 0.70, 0.52), "roughness": 0.58, "noise": 7.5},
    "garage-roll-door": {"dark": (0.25, 0.28, 0.32), "light": (0.55, 0.58, 0.62), "roughness": 0.32, "noise": 14.0},
    "garage-server-closet": {"dark": (0.06, 0.07, 0.09), "light": (0.22, 0.24, 0.28), "roughness": 0.38, "noise": 9.0},
    "garage-land-for-sale-sign": {"dark": (0.55, 0.48, 0.08), "light": (0.92, 0.82, 0.15), "roughness": 0.45, "noise": 5.0},
    "garage-lot-curb": {"dark": (0.38, 0.36, 0.34), "light": (0.55, 0.52, 0.48), "roughness": 0.78, "noise": 16.0},
    "garage-window-frame": {"dark": (0.55, 0.28, 0.12), "light": (0.78, 0.42, 0.18), "roughness": 0.48, "noise": 8.0},
    "garage-desk-crt": {"dark": (0.35, 0.22, 0.12), "light": (0.62, 0.48, 0.32), "roughness": 0.48, "noise": 7.0},
    "garage-crt-monitor": {"dark": (0.22, 0.24, 0.28), "light": (0.42, 0.44, 0.48), "roughness": 0.38, "noise": 6.0},
    "garage-crt-tower": {"dark": (0.55, 0.52, 0.48), "light": (0.82, 0.78, 0.72), "roughness": 0.38, "noise": 8.0},
    "garage-voodude-rack": {"dark": (0.05, 0.05, 0.07), "light": (0.18, 0.17, 0.20), "roughness": 0.42, "noise": 10.0},
    "garage-whiteboard-1995": {"dark": (0.72, 0.74, 0.70), "light": (0.92, 0.94, 0.88), "roughness": 0.35, "noise": 4.0},
    "garage-coffee-drip": {"dark": (0.42, 0.06, 0.04), "light": (0.82, 0.22, 0.08), "roughness": 0.35, "noise": 7.0},
    "garage-folding-chair": {"dark": (0.22, 0.32, 0.55), "light": (0.38, 0.52, 0.78), "roughness": 0.52, "noise": 6.0},
    "garage-cable-spool": {"dark": (0.28, 0.26, 0.24), "light": (0.48, 0.44, 0.38), "roughness": 0.58, "noise": 12.0},
    "staff-researcher-idle": {"dark": (0.22, 0.32, 0.58), "light": (0.42, 0.55, 0.82), "roughness": 0.48, "noise": 6.0},
    "staff-engineer-work": {"dark": (0.18, 0.28, 0.22), "light": (0.42, 0.58, 0.38), "roughness": 0.45, "noise": 7.0},
    "staff-safety-idle": {"dark": (0.72, 0.38, 0.12), "light": (0.92, 0.58, 0.22), "roughness": 0.42, "noise": 5.5},
    "staff-sales-idle": {"dark": (0.32, 0.22, 0.48), "light": (0.58, 0.42, 0.72), "roughness": 0.46, "noise": 6.5},
    "staff-recruiter-idle": {"dark": (0.48, 0.32, 0.18), "light": (0.78, 0.55, 0.32), "roughness": 0.48, "noise": 6.0},
    "staff-gpu-tech-idle": {"dark": (0.08, 0.42, 0.18), "light": (0.22, 0.72, 0.35), "roughness": 0.40, "noise": 8.0},
    "staff-founder-idle": {"dark": (0.42, 0.28, 0.16), "light": (0.72, 0.52, 0.32), "roughness": 0.50, "noise": 6.0},
    "staff-pose-typing": {"dark": (0.28, 0.32, 0.38), "light": (0.52, 0.58, 0.68), "roughness": 0.44, "noise": 7.0},
    "wisp-happy": {"dark": (0.35, 0.22, 0.82), "light": (0.62, 0.45, 0.98), "roughness": 0.25, "noise": 3.0},
    "wisp-neutral": {"dark": (0.22, 0.38, 0.78), "light": (0.48, 0.62, 0.95), "roughness": 0.28, "noise": 3.0},
    "wisp-worried": {"dark": (0.72, 0.48, 0.12), "light": (0.95, 0.72, 0.32), "roughness": 0.30, "noise": 3.0},
    "wisp-angry": {"dark": (0.72, 0.12, 0.08), "light": (0.95, 0.35, 0.22), "roughness": 0.28, "noise": 3.0},
    "wisp-glowing": {"dark": (0.48, 0.28, 0.92), "light": (0.78, 0.55, 1.0), "roughness": 0.20, "noise": 2.5},
}

DEFAULT = {"dark": (0.35, 0.30, 0.26), "light": (0.58, 0.52, 0.45), "roughness": 0.55, "noise": 8.0}


def spec_for(asset_id: str) -> dict:
    return {**DEFAULT, **PALETTE.get(asset_id, {})}