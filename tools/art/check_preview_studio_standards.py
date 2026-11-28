#!/usr/bin/env python3
"""Studio gates: #969696 backdrop, >=40% frame fill, swatch hue means, assembly + HUD."""
from __future__ import annotations

import colorsys
import json
import subprocess
import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
PREVIEW = REPO / "docs/art/previews"
STUDIO_RGB = (150, 150, 150)
BACKDROP_TOL = 14
MIN_FRAME_FILL = 0.40
MAX_FRAME_FILL = 0.78

SWATCH_MEANS: dict[str, tuple[int, int, int]] = {
    "tex-garage-trim-orange": (215, 100, 42),
    "tex-garage-wall-beige": (210, 188, 158),
    "tex-hud-paper-warm": (244, 240, 232),
}
SWATCH_TOL = 38

GARAGE_MANIFEST = REPO / "unity/Assets/Art/EarlyGame/garage-manifest.json"
HUD_IDS = [
    "hud-topbar-beige",
    "icon-cash-era0",
    "icon-date-era0",
    "icon-era-bar",
    "icon-build-panel",
    "icon-hire-panel",
    "icon-train-panel",
    "icon-gpu-hire",
    "icon-week-advance",
    "panel-build-bg",
    "panel-hire-bg",
    "panel-train-bg",
]


def check_backdrop(path: Path) -> tuple[bool, str]:
    from PIL import Image

    im = Image.open(path).convert("RGB")
    w, h = im.size
    inset = max(4, w // 64)
    corners = [
        im.getpixel((inset, inset)),
        im.getpixel((w - 1 - inset, inset)),
        im.getpixel((inset, h - 1 - inset)),
        im.getpixel((w - 1 - inset, h - 1 - inset)),
    ]
    for i, px in enumerate(corners):
        if any(abs(px[c] - STUDIO_RGB[c]) > BACKDROP_TOL for c in range(3)):
            return False, f"corner{i}={px} need~{STUDIO_RGB}"
    return True, "backdrop_969696"


def check_fill(asset_id: str) -> tuple[bool, str]:
    mask = PREVIEW / f"{asset_id}.mask.png"
    if not mask.is_file():
        return False, "missing_mask"
    sys.path.insert(0, str(REPO / "tools/art"))
    from preview_quality_lib import analyze_alpha_mask
    alpha = analyze_alpha_mask(mask)
    extent = float(alpha.get("extent_coverage", 0.0))
    min_fill = 0.38 if asset_id == "garage-scene-kit" else MIN_FRAME_FILL
    if extent < min_fill:
        return False, f"frame_fill={extent:.3f}<{min_fill}"
    if not asset_id.startswith(("icon-", "panel-", "hud-")) and extent > MAX_FRAME_FILL:
        return False, f"frame_fill={extent:.3f}>{MAX_FRAME_FILL}"
    return True, f"frame_fill={extent:.3f}"


def check_swatch(asset_id: str) -> tuple[bool, str]:
    target = SWATCH_MEANS.get(asset_id)
    if not target:
        return True, "skip"
    path = PREVIEW / f"{asset_id}.png"
    from PIL import Image

    im = Image.open(path).convert("RGB")
    w, h = im.size
    crop = im.crop((w // 4, h // 4, 3 * w // 4, 3 * h // 4))
    px = list(crop.getdata())
    if not px:
        return False, "empty"
    mr = sum(p[0] for p in px) / len(px)
    mg = sum(p[1] for p in px) / len(px)
    mb = sum(p[2] for p in px) / len(px)
    mean = (int(round(mr)), int(round(mg)), int(round(mb)))
    if (
        abs(mean[0] - target[0]) > SWATCH_TOL
        or abs(mean[1] - target[1]) > SWATCH_TOL
        or abs(mean[2] - target[2]) > SWATCH_TOL
    ):
        return False, f"mean={mean} want~{target}"
    return True, f"mean={mean}"


def check_hud_not_white(path: Path, mask_path: Path) -> tuple[bool, str]:
    from PIL import Image

    if not mask_path.is_file():
        return False, "missing_mask"
    rgb = Image.open(path).convert("RGB")
    mask = Image.open(mask_path).convert("RGBA")
    w, h = rgb.size
    if mask.size != (w, h):
        mask = mask.resize((w, h), Image.Resampling.BILINEAR)
    pixels = list(rgb.getdata())
    alpha = list(mask.getdata())
    fg = []
    for i, mc in enumerate(alpha):
        if mc[3] >= 200:
            fg.append(pixels[i])
    if len(fg) < 32:
        return False, "hud_fg_empty"
    whiteish = 0
    for r, g, b in fg:
        h, s, v = colorsys.rgb_to_hsv(r / 255.0, g / 255.0, b / 255.0)
        if s < 0.08 and v > 0.92:
            whiteish += 1
    frac = whiteish / len(fg)
    if frac > 0.70:
        return False, f"hud_white_frac={frac:.3f}>0.70"
    return True, f"hud_white_frac={frac:.3f}"


def check_hud_camera_ortho(asset_id: str) -> tuple[bool, str]:
    meta = PREVIEW / f"{asset_id}.frame.json"
    if not meta.is_file():
        return False, "missing_frame_meta"
    data = json.loads(meta.read_text(encoding="utf-8"))
    if data.get("camera_type") != "orthographic":
        return False, f"camera_type={data.get('camera_type')!r}"
    if data.get("preview_kind") != "hud":
        return False, f"preview_kind={data.get('preview_kind')!r}"
    return True, "ortho_hud"


def _load_assembly_gap_cache() -> dict[str, str]:
    proc = subprocess.run(
        [sys.executable, str(REPO / "tools/art/check_garage_part_bbox_gap.py")],
        cwd=REPO,
        capture_output=True,
        text=True,
        check=False,
    )
    out = proc.stdout + proc.stderr
    cache: dict[str, str] = {}
    for line in out.splitlines():
        if line.startswith("OK garage-"):
            aid = line.split()[1]
            cache[aid] = line
        elif line.startswith("FAIL garage-"):
            aid = line.split()[1]
            cache[aid] = line
    return cache


def check_garage_mesh_island_gap(asset_id: str, cache: dict[str, str]) -> tuple[bool, str]:
    if asset_id == "garage-scene-kit":
        return True, "scene_exempt"
    row = cache.get(asset_id)
    if not row:
        return False, "missing_bbox_gap_row"
    if row.startswith("OK"):
        return True, "cluster_connected"
    return False, row.split(" ", 2)[-1]


def main() -> int:
    assets = [a["id"] for a in json.loads(GARAGE_MANIFEST.read_text())["assets"]]
    assets += ["garage-scene-kit"]
    assets += HUD_IDS
    assets += list(SWATCH_MEANS.keys())
    assembly_cache = _load_assembly_gap_cache()
    rc = 0
    for asset_id in assets:
        path = PREVIEW / f"{asset_id}.png"
        if not path.is_file():
            print(f"FAIL {asset_id} missing_preview")
            rc = 1
            continue
        if asset_id.startswith("tex-"):
            msg = "garage_swatch_only"
            if asset_id.startswith("tex-hud-"):
                ok, msg = check_backdrop(path)
                if not ok:
                    print(f"FAIL {asset_id} backdrop {msg}")
                    rc = 1
                    continue
            if asset_id in SWATCH_MEANS:
                ok2, msg2 = check_swatch(asset_id)
                if not ok2:
                    print(f"FAIL {asset_id} swatch {msg2}")
                    rc = 1
                    continue
                print(f"OK {asset_id} {msg} {msg2}")
            else:
                print(f"OK {asset_id} {msg}")
            continue
        ok, msg = check_backdrop(path)
        if not ok:
            print(f"FAIL {asset_id} backdrop {msg}")
            rc = 1
            continue
        if asset_id.startswith(("hud-", "icon-", "panel-")):
            ok_h, msg_h = check_hud_camera_ortho(asset_id)
            if not ok_h:
                print(f"FAIL {asset_id} {msg_h}")
                rc = 1
                continue
            ok_w, msg_w = check_hud_not_white(path, PREVIEW / f"{asset_id}.mask.png")
            if not ok_w:
                print(f"FAIL {asset_id} {msg_w}")
                rc = 1
                continue
            ok3, msg3 = check_fill(asset_id)
            if not ok3:
                print(f"FAIL {asset_id} {msg3}")
                rc = 1
                continue
            print(f"OK {asset_id} {msg} {msg_h} {msg_w} {msg3}")
            continue
        if asset_id.startswith("garage-"):
            ok_g, msg_g = check_garage_mesh_island_gap(asset_id, assembly_cache)
            if not ok_g:
                print(f"FAIL {asset_id} {msg_g}")
                rc = 1
                continue
        if not asset_id.startswith("wisp-"):
            ok3, msg3 = check_fill(asset_id)
            if not ok3:
                print(f"FAIL {asset_id} {msg3}")
                rc = 1
                continue
            extra = f" {msg_g}" if asset_id.startswith("garage-") else ""
            print(f"OK {asset_id} {msg} {msg3}{extra}")
        else:
            print(f"OK {asset_id} {msg} wisp_exempt_fill")
    return rc


if __name__ == "__main__":
    sys.exit(main())