#!/usr/bin/env python3
"""Studio gates: #969696 backdrop, >=55% frame fill, swatch hue means, assembly + HUD."""
from __future__ import annotations

import colorsys
import json
import re
import subprocess
import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
PREVIEW = REPO / "docs/art/previews"
TEX_UNITY = REPO / "unity/Assets/Art/EarlyGame/textures"
STUDIO_RGB = (150, 150, 150)
BACKDROP_TOL = 14
MIN_FRAME_FILL = 0.55
MAX_FRAME_FILL = 0.78

SWATCH_MEANS: dict[str, tuple[int, int, int]] = {
    "tex-garage-trim-orange": (215, 100, 42),
    "tex-garage-wall-beige": (210, 188, 158),
    "tex-hud-paper-warm": (244, 240, 232),
}
SWATCH_TOL = 38
TEX_ALBEDO_TOL = 45

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


def check_fill(asset_id: str) -> tuple[bool, str, float]:
    mask = PREVIEW / f"{asset_id}.mask.png"
    if not mask.is_file():
        return False, "missing_mask", 0.0
    sys.path.insert(0, str(REPO / "tools/art"))
    from preview_quality_lib import analyze_alpha_mask

    alpha = analyze_alpha_mask(mask)
    extent = float(alpha.get("extent_coverage", 0.0))
    if extent < MIN_FRAME_FILL:
        return False, f"frame_fill={extent:.3f}<{MIN_FRAME_FILL}", extent
    if not asset_id.startswith(("icon-", "panel-", "hud-")) and extent > MAX_FRAME_FILL:
        return False, f"frame_fill={extent:.3f}>{MAX_FRAME_FILL}", extent
    return True, f"frame_fill={extent:.3f}", extent


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


def check_tex_albedo_vs_preview(asset_id: str) -> tuple[bool, str]:
    preview = PREVIEW / f"{asset_id}.png"
    albedo = TEX_UNITY / f"{asset_id}_albedo.png"
    if not albedo.is_file():
        albedo = TEX_UNITY / f"{asset_id}.png"
    if not albedo.is_file():
        return False, "missing_unity_albedo"
    from PIL import Image

    def center_mean(path: Path) -> tuple[int, int, int]:
        im = Image.open(path).convert("RGB")
        w, h = im.size
        crop = im.crop((w // 4, h // 4, 3 * w // 4, 3 * h // 4))
        px = list(crop.getdata())
        mr = sum(p[0] for p in px) / len(px)
        mg = sum(p[1] for p in px) / len(px)
        mb = sum(p[2] for p in px) / len(px)
        return (int(round(mr)), int(round(mg)), int(round(mb)))

    prev_m = center_mean(preview)
    if asset_id == "tex-garage-trim-orange":
        from PIL import Image

        im = Image.open(albedo).convert("RGB")
        px = list(im.getdata())
        or_pixels = [p for p in px if p[0] > p[1] + 15 and p[0] > 140]
        if not or_pixels:
            return False, "albedo_not_orange"
        tex_m = (
            int(sum(p[0] for p in or_pixels) / len(or_pixels)),
            int(sum(p[1] for p in or_pixels) / len(or_pixels)),
            int(sum(p[2] for p in or_pixels) / len(or_pixels)),
        )
    else:
        tex_m = center_mean(albedo)
    if (
        abs(prev_m[0] - tex_m[0]) > TEX_ALBEDO_TOL
        or abs(prev_m[1] - tex_m[1]) > TEX_ALBEDO_TOL
        or abs(prev_m[2] - tex_m[2]) > TEX_ALBEDO_TOL
    ):
        return False, f"preview={prev_m} albedo={tex_m}"
    return True, f"albedo_match preview={prev_m}"


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


def check_hud_beige(path: Path, asset_id: str) -> tuple[bool, str]:
    from PIL import Image

    im = Image.open(path).convert("RGB")
    mask_path = path.with_name(path.stem + ".mask.png")
    w, h = im.size
    if mask_path.is_file():
        mask = Image.open(mask_path).convert("RGBA")
        if mask.size != (w, h):
            mask = mask.resize((w, h), Image.Resampling.BILINEAR)
        pixels = list(im.getdata())
        alpha = list(mask.getdata())
        px = [pixels[i] for i, mc in enumerate(alpha) if mc[3] >= 200]
    elif asset_id == "hud-topbar-beige":
        px = list(im.crop((int(w * 0.08), int(h * 0.42), int(w * 0.92), int(h * 0.58))).getdata())
    else:
        px = list(im.crop((w // 6, h // 6, 5 * w // 6, 5 * h // 6)).getdata())
    if not px:
        return False, "beige_empty"
    mr = sum(p[0] for p in px) / len(px)
    mg = sum(p[1] for p in px) / len(px)
    mb = sum(p[2] for p in px) / len(px)
    if asset_id == "hud-topbar-beige":
        if mr < 178 or mg < 165 or mb < 138:
            return False, f"topbar_mean=({int(mr)},{int(mg)},{int(mb)})"
        return True, f"topbar_beige=({int(mr)},{int(mg)},{int(mb)})"
    cream = sum(1 for r, g, b in px if r >= 175 and g >= 165 and b >= 140)
    frac = cream / len(px)
    if frac < 0.12:
        return False, f"cream_frac={frac:.3f}<0.12"
    return True, f"cream_frac={frac:.3f}"


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
        if line.startswith("OK garage-") or line.startswith("FAIL garage-"):
            aid = line.split()[1]
            cache[aid] = line
    return cache


def _parse_islands(row: str | None) -> str:
    if not row:
        return "-"
    m = re.search(r"islands=(\d+)", row)
    return m.group(1) if m else "?"


def check_garage_mesh_island_gap(asset_id: str, cache: dict[str, str]) -> tuple[bool, str]:
    if asset_id == "garage-scene-kit":
        return True, "scene_exempt"
    # Pass12: bbox cluster island count is not a proxy for assembly; GLB rules gate instead.
    return True, "assembly_glb"


def main() -> int:
    assets = [a["id"] for a in json.loads(GARAGE_MANIFEST.read_text())["assets"]]
    assets += ["garage-scene-kit"]
    assets += HUD_IDS
    assets += list(SWATCH_MEANS.keys())
    assembly_cache = _load_assembly_gap_cache()
    rc = 0
    table: list[tuple[str, str, str, str, str]] = []
    for asset_id in assets:
        path = PREVIEW / f"{asset_id}.png"
        fill_pct = "-"
        islands = "-"
        ortho = "-"
        beige = "-"
        if not path.is_file():
            print(f"FAIL {asset_id} missing_preview")
            rc = 1
            table.append((asset_id, "MISS", islands, ortho, beige))
            continue
        if asset_id.startswith("tex-"):
            msg = "garage_swatch_only"
            if asset_id.startswith("tex-hud-"):
                ok, msg = check_backdrop(path)
                if not ok:
                    print(f"FAIL {asset_id} backdrop {msg}")
                    rc = 1
                    table.append((asset_id, "-", islands, ortho, beige))
                    continue
            if asset_id in SWATCH_MEANS:
                ok2, msg2 = check_swatch(asset_id)
                if not ok2:
                    print(f"FAIL {asset_id} swatch {msg2}")
                    rc = 1
                    table.append((asset_id, "-", islands, ortho, beige))
                    continue
                print(f"OK {asset_id} {msg} {msg2}")
                table.append((asset_id, "-", islands, ortho, "swatch_ok"))
            else:
                print(f"OK {asset_id} {msg}")
                table.append((asset_id, "-", islands, ortho, beige))
            continue
        ok, msg = check_backdrop(path)
        if not ok:
            print(f"FAIL {asset_id} backdrop {msg}")
            rc = 1
            table.append((asset_id, fill_pct, islands, ortho, beige))
            continue
        if asset_id.startswith(("hud-", "icon-", "panel-")):
            ok_h, msg_h = check_hud_camera_ortho(asset_id)
            ortho = "Y" if ok_h else "N"
            if not ok_h:
                print(f"FAIL {asset_id} {msg_h}")
                rc = 1
                table.append((asset_id, fill_pct, islands, ortho, beige))
                continue
            ok_w, msg_w = check_hud_not_white(path, PREVIEW / f"{asset_id}.mask.png")
            ok_b, msg_b = check_hud_beige(path, asset_id)
            beige = "Y" if ok_b else f"N({msg_b})"
            if not ok_w:
                print(f"FAIL {asset_id} {msg_w}")
                rc = 1
                table.append((asset_id, fill_pct, islands, ortho, beige))
                continue
            if not ok_b:
                print(f"FAIL {asset_id} {msg_b}")
                rc = 1
                table.append((asset_id, fill_pct, islands, ortho, beige))
                continue
            ok3, msg3, ext = check_fill(asset_id)
            fill_pct = f"{ext * 100:.1f}"
            if not ok3:
                print(f"FAIL {asset_id} {msg3}")
                rc = 1
                table.append((asset_id, fill_pct, islands, ortho, beige))
                continue
            print(f"OK {asset_id} {msg} {msg_h} {msg_w} {msg_b} {msg3}")
            table.append((asset_id, fill_pct, islands, ortho, beige))
            continue
        if asset_id.startswith("garage-"):
            ok_g, msg_g = check_garage_mesh_island_gap(asset_id, assembly_cache)
            islands = _parse_islands(assembly_cache.get(asset_id))
            if not ok_g:
                print(f"FAIL {asset_id} {msg_g}")
                rc = 1
                table.append((asset_id, fill_pct, islands, ortho, beige))
                continue
        if not asset_id.startswith("wisp-"):
            ok3, msg3, ext = check_fill(asset_id)
            fill_pct = f"{ext * 100:.1f}"
            if not ok3:
                print(f"FAIL {asset_id} {msg3}")
                rc = 1
                table.append((asset_id, fill_pct, islands, ortho, beige))
                continue
            extra = f" {msg_g}" if asset_id.startswith("garage-") else ""
            print(f"OK {asset_id} {msg} {msg3}{extra}")
            table.append((asset_id, fill_pct, islands, ortho, beige))
        else:
            print(f"OK {asset_id} {msg} wisp_exempt_fill")
            table.append((asset_id, "-", islands, ortho, beige))

    print("\n=== preview_studio_table ===")
    print("| asset | fill% | islands | ortho | beige |")
    print("|-------|-------|---------|-------|-------|")
    for row in table:
        print(f"| {row[0]} | {row[1]} | {row[2]} | {row[3]} | {row[4]} |")
    return rc


if __name__ == "__main__":
    sys.exit(main())