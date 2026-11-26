#!/usr/bin/env python3
"""Fail near-uniform / low-chroma preview PNGs for BlenderBaked gate assets."""
from __future__ import annotations

import hashlib
import json
import struct
import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
PREVIEW_DIR = REPO / "docs" / "art" / "previews"
BLENDER_ROOT = REPO / "unity" / "Assets" / "Art" / "BlenderBaked"

GATED_ASSETS = (
    "prop-desk",
    "prop-server-rack",
    "prop-whiteboard",
    "ui-rival-badge",
)

MIN_STDDEV = 8.0
MIN_COVERAGE = 0.20
MIN_MEAN_LUM = 40.0
MIN_MEAN_SAT = 0.10
BORDER_PX = 4
BORDER_MIN_PIXELS = 18
MIN_SIL_FRAC = 0.35
MAX_SIL_FRAC = 0.75
MIN_SUBJECT_BBOX_FRAC = 0.60
MAX_SUBJECT_BBOX_FRAC = 0.90
MIN_FRAME_MARGIN_FRAC = 0.05
MAX_FLAT_PIXEL_FRAC = 0.50
MAX_GROUND_LUM = 165


def _read_png_rgb(path: Path) -> tuple[int, int, list[tuple[int, int, int]]]:
    try:
        from PIL import Image

        im = Image.open(path).convert("RGB")
        w, h = im.size
        return w, h, list(im.getdata())
    except ImportError:
        pass
    import zlib

    data = path.read_bytes()
    if data[:8] != b"\x89PNG\r\n\x1a\n":
        raise ValueError(f"not PNG: {path}")
    pos = 8
    width = height = None
    color_type = None
    raw = b""
    while pos < len(data):
        length = struct.unpack(">I", data[pos : pos + 4])[0]
        pos += 4
        ctype = data[pos : pos + 4]
        pos += 4
        chunk = data[pos : pos + length]
        pos += length + 4
        if ctype == b"IHDR":
            width, height, bitdepth, color_type = struct.unpack(">IIBB", chunk[:10])[:4]
        elif ctype == b"IDAT":
            raw += chunk
        elif ctype == b"IEND":
            break
    if width is None or height is None:
        raise ValueError(f"no IHDR: {path}")
    bpp = {0: 1, 2: 3, 3: 1, 4: 2, 6: 4}.get(color_type or 2, 3)
    inflated = zlib.decompress(raw)

    def _paeth(a: int, b: int, c: int) -> int:
        p = a + b - c
        pa, pb, pc = abs(p - a), abs(p - b), abs(p - c)
        if pa <= pb and pa <= pc:
            return a
        if pb <= pc:
            return b
        return c

    row_bytes = width * bpp
    prior = bytearray(row_bytes)
    flat = bytearray()
    pos = 0
    for _y in range(height):
        ftype = inflated[pos]
        pos += 1
        chunk = bytearray(inflated[pos : pos + row_bytes])
        pos += row_bytes
        if ftype == 1:
            for i in range(row_bytes):
                chunk[i] = (chunk[i] + prior[i]) & 0xFF
        elif ftype == 2:
            for i in range(row_bytes):
                chunk[i] = (chunk[i] + flat[i]) & 0xFF
        elif ftype == 3:
            for i in range(row_bytes):
                left = prior[i] if i >= bpp else 0
                chunk[i] = (chunk[i] + (left // 2)) & 0xFF
        elif ftype == 4:
            for i in range(row_bytes):
                left = prior[i] if i >= bpp else 0
                up = flat[i]
                up_left = prior[i - bpp] if i >= bpp else 0
                chunk[i] = (chunk[i] + _paeth(left, up, up_left)) & 0xFF
        prior = chunk
        flat.extend(chunk)

    pixels: list[tuple[int, int, int]] = []
    for y in range(height):
        row = flat[y * row_bytes : (y + 1) * row_bytes]
        for x in range(width):
            i = x * bpp
            if bpp >= 3:
                pixels.append((row[i], row[i + 1], row[i + 2]))
            else:
                g = row[i]
                pixels.append((g, g, g))
    return width, height, pixels


def _bg_rgb(pixels: list[tuple[int, int, int]], width: int, height: int) -> tuple[int, int, int]:
    picks = [
        width // 2,
        width // 4,
        (3 * width) // 4,
        8,
        width - 9,
    ]
    samples = [pixels[x] for x in picks]
    return (
        sum(s[0] for s in samples) // len(samples),
        sum(s[1] for s in samples) // len(samples),
        sum(s[2] for s in samples) // len(samples),
    )


def _is_subject(r: int, g: int, b: int, bg: tuple[int, int, int]) -> bool:
    dist = abs(r - bg[0]) + abs(g - bg[1]) + abs(b - bg[2])
    if dist < 32:
        return False
    lum = (r * 299 + g * 587 + b * 114) // 1000
    mx, mn = max(r, g, b), min(r, g, b)
    sat = (mx - mn) / 255.0 if mx else 0.0
    if sat < 0.07 and 100 <= lum <= 150:
        return False
    if r > g + 3 and g >= b - 2 and r > 68 and lum > 24:
        return True
    if r > g + 8 and g > b + 6 and lum > 50:
        return True
    return sat > 0.09 and lum > 52 and lum < 245


def _object_pixel(r: int, g: int, b: int, bg: tuple[int, int, int]) -> bool:
    return abs(r - bg[0]) + abs(g - bg[1]) + abs(b - bg[2]) > 28


def _framing_metrics(
    width: int,
    height: int,
    pixels: list[tuple[int, int, int]],
    *,
    asset_id: str | None = None,
) -> dict[str, float | bool]:
    bg = _bg_rgb(pixels, width, height)
    edge_hits = {"t": 0, "b": 0, "l": 0, "r": 0}
    row_hits = [0] * height
    for y in range(height):
        for x in range(width):
            r, g, b = pixels[y * width + x]
            if not _object_pixel(r, g, b, bg):
                continue
            row_hits[y] += 1
            subj = (
                _object_pixel(r, g, b, bg)
                if asset_id == "prop-desk"
                else _is_subject(r, g, b, bg)
            )
            if subj:
                if y < BORDER_PX:
                    edge_hits["t"] += 1
                if y >= height - BORDER_PX and y < int(height * 0.92):
                    edge_hits["b"] += 1
                if x < BORDER_PX:
                    edge_hits["l"] += 1
                if x >= width - BORDER_PX:
                    edge_hits["r"] += 1
    border_touch = any(v >= BORDER_MIN_PIXELS for v in edge_hits.values())
    subj_ys: list[int] = []
    for y in range(height):
        for x in range(width):
            if _object_pixel(*pixels[y * width + x], bg):
                subj_ys.append(y)
                break
    sil_frac = (max(subj_ys) - min(subj_ys) + 1) / height if subj_ys else 0.0
    xs: list[int] = []
    ys2: list[int] = []
    for y in range(height):
        for x in range(width):
            r, g, b = pixels[y * width + x]
            subj = (
                _object_pixel(r, g, b, bg)
                if asset_id == "prop-desk"
                else _is_subject(r, g, b, bg)
            )
            if subj:
                xs.append(x)
                ys2.append(y)
    if xs:
        bw = (max(xs) - min(xs) + 1) / width
        bh = (max(ys2) - min(ys2) + 1) / height
        subject_bbox = max(bw, bh)
        margin_l = min(xs) / width
        margin_r = (width - 1 - max(xs)) / width
        margin_t = min(ys2) / height
        margin_b = (height - 1 - max(ys2)) / height
        min_margin = min(margin_l, margin_r, margin_t, margin_b)
    else:
        subject_bbox = 0.0
        min_margin = 0.0
    return {
        "border_touch": border_touch,
        "silhouette_frac": sil_frac,
        "subject_bbox_frac": subject_bbox,
        "min_margin_frac": min_margin,
    }


def stats(path: Path, asset_id: str | None = None) -> dict[str, float]:
    width, height, pixels = _read_png_rgb(path)
    if not pixels:
        return {"stddev": 0.0, "coverage": 0.0, "mean_lum": 0.0, "mean_sat": 0.0}
    lums: list[int] = []
    sats: list[float] = []
    for r, g, b in pixels:
        lums.append((r * 299 + g * 587 + b * 114) // 1000)
        mx, mn = max(r, g, b), min(r, g, b)
        sats.append((mx - mn) / 255.0 if mx else 0.0)
    n = len(lums)
    mean = sum(lums) / n
    var = sum((x - mean) ** 2 for x in lums) / n
    std = var**0.5
    coverage = sum(1 for x in lums if x > 24) / n
    bg = _bg_rgb(pixels, width, height)
    subj_lum: list[int] = []
    subj_sat: list[float] = []
    for i, (r, g, b) in enumerate(pixels):
        if _is_subject(r, g, b, bg):
            subj_lum.append(lums[i])
            subj_sat.append(sats[i])
    cn = len(subj_lum) or 1
    frame = _framing_metrics(width, height, pixels, asset_id=asset_id)
    ground_lum: list[int] = []
    y0 = int(height * 0.88)
    for y in range(y0, height):
        for x in range(int(width * 0.25), int(width * 0.75)):
            r, g, b = pixels[y * width + x]
            ground_lum.append((r * 299 + g * 587 + b * 114) // 1000)
    gmean = sum(ground_lum) / len(ground_lum) if ground_lum else 255.0
    from collections import Counter

    near_bg = sum(
        1
        for r, g, b in pixels
        if abs(r - bg[0]) + abs(g - bg[1]) + abs(b - bg[2]) < 18
    )
    flat_frac = near_bg / n
    corner_samples: list[tuple[int, int, int]] = []
    margin = 8
    for cx, cy in (
        (margin, margin),
        (width - 1 - margin, margin),
        (margin, height - 1 - margin),
        (width - 1 - margin, height - 1 - margin),
    ):
        corner_samples.append(pixels[cy * width + cx])
    corner_lums = [
        (r * 299 + g * 587 + b * 114) // 1000 for r, g, b in corner_samples
    ]
    corner_mean = sum(corner_lums) / len(corner_lums) if corner_lums else 255.0
    desk_extra: dict[str, float] = {}
    if asset_id == "prop-desk":
        subj_sats_sorted = sorted(subj_sat)
        med_sat = (
            subj_sats_sorted[len(subj_sats_sorted) // 2] if subj_sats_sorted else 0.0
        )
        y0, y1 = int(height * 0.40), int(height * 0.62)
        x0, x1 = int(width * 0.12), int(width * 0.88)
        desk_px = 0
        brown_px = 0
        for y in range(y0, y1):
            for x in range(x0, x1):
                r, g, b = pixels[y * width + x]
                if not _is_subject(r, g, b, bg):
                    continue
                desk_px += 1
                if r > g + 4 and g > b and r > 95:
                    brown_px += 1
        desk_brown_frac = brown_px / desk_px if desk_px else 0.0
        sy0, sy1 = int(height * 0.16), int(height * 0.50)
        sx0, sx1 = int(width * 0.36), int(width * 0.64)
        screen_lums: list[float] = []
        for y in range(sy0, sy1):
            for x in range(sx0, sx1):
                r, g, b = pixels[y * width + x]
                if not _is_subject(r, g, b, bg):
                    continue
                screen_lums.append(max(r, g, b) / 255.0)
        screen_peak = max(screen_lums) if screen_lums else 0.0
        corner_rgb_max_delta = 0.0
        for r, g, b in corner_samples:
            for ch in (r, g, b):
                corner_rgb_max_delta = max(corner_rgb_max_delta, abs(ch - 150))
        outside_screen_sat: list[float] = []
        blow_n = 0
        outside_n = 0
        for y in range(height):
            for x in range(width):
                r, g, b = pixels[y * width + x]
                if not _is_subject(r, g, b, bg):
                    continue
                in_screen = sx0 <= x <= sx1 and sy0 <= y <= sy1
                mx, mn = max(r, g, b), min(r, g, b)
                if not in_screen:
                    outside_n += 1
                    outside_screen_sat.append((mx - mn) / 255.0)
                    if mx > 250:
                        blow_n += 1
        blow_frac_outside = blow_n / outside_n if outside_n else 0.0
        outside_mean_sat = (
            sum(outside_screen_sat) / len(outside_screen_sat) if outside_screen_sat else 0.0
        )
        if outside_n < 80:
            outside_mean_sat = sum(subj_sat) / cn if subj_sat else 0.0
        desk_extra = {
            "subject_median_sat": med_sat,
            "desk_top_brown_frac": desk_brown_frac,
            "screen_peak_lum": screen_peak,
            "corner_rgb_max_delta": corner_rgb_max_delta,
            "outside_screen_mean_sat": outside_mean_sat,
            "blow_frac_outside_screen": blow_frac_outside,
        }
    return {
        "stddev": std,
        "coverage": coverage,
        "mean_lum": sum(subj_lum) / cn,
        "mean_sat": sum(subj_sat) / cn,
        "border_touch": float(frame["border_touch"]),
        "silhouette_frac": float(frame["silhouette_frac"]),
        "subject_bbox_frac": float(frame["subject_bbox_frac"]),
        "min_margin_frac": float(frame["min_margin_frac"]),
        "flat_pixel_frac": flat_frac,
        "ground_lum": gmean,
        "corner_lum_mean": corner_mean,
        **desk_extra,
    }


def gate_fail(asset_id: str, s: dict[str, float]) -> list[str]:
    reasons: list[str] = []
    if s["stddev"] < MIN_STDDEV or s["coverage"] < MIN_COVERAGE:
        reasons.append(
            f"uniform stddev={s['stddev']:.2f} coverage={s['coverage']:.1%}"
        )
    min_sat = 0.075 if asset_id == "prop-desk" else MIN_MEAN_SAT
    if s["mean_lum"] < MIN_MEAN_LUM or s["mean_sat"] < min_sat:
        reasons.append(
            f"chroma mean_lum={s['mean_lum']:.1f} mean_sat={s['mean_sat']:.3f}"
        )
    if s.get("border_touch", 0) >= 0.5:
        reasons.append("framing border_touch")
    sil = s.get("silhouette_frac", 0.0)
    sil_max = 0.90 if asset_id == "prop-desk" else MAX_SIL_FRAC
    if sil and (sil < MIN_SIL_FRAC or sil > sil_max):
        reasons.append(f"framing silhouette_frac={sil:.2%}")
    if asset_id == "prop-desk":
        sb = s.get("subject_bbox_frac", 0.0)
        if sb < MIN_SUBJECT_BBOX_FRAC or sb > MAX_SUBJECT_BBOX_FRAC:
            reasons.append(f"subject_bbox_frac={sb:.2%}")
        mg = s.get("min_margin_frac", 0.0)
        if mg < MIN_FRAME_MARGIN_FRAC:
            reasons.append(f"min_margin_frac={mg:.2%}")
        flat_max = 0.72 if asset_id == "prop-desk" else MAX_FLAT_PIXEL_FRAC
        if s.get("flat_pixel_frac", 1.0) > flat_max:
            reasons.append(f"flat_pixel_frac={s['flat_pixel_frac']:.2%}")
        ground_max = 178 if asset_id == "prop-desk" else MAX_GROUND_LUM
        if s.get("ground_lum", 255) > ground_max:
            reasons.append(f"ground_lum={s['ground_lum']:.1f}")
        if s.get("subject_median_sat", 1.0) < 0.08:
            reasons.append(f"subject_median_sat={s.get('subject_median_sat', 0):.3f}")
        if s.get("desk_top_brown_frac", 0.0) < 0.12:
            reasons.append(f"desk_top_brown_frac={s.get('desk_top_brown_frac', 0):.2%}")
        if s.get("screen_peak_lum", 0.0) < 0.6:
            reasons.append(f"screen_peak_lum={s.get('screen_peak_lum', 0):.2f}")
        if s.get("outside_screen_mean_sat", 0.0) < 0.04:
            reasons.append(f"outside_screen_mean_sat={s.get('outside_screen_mean_sat', 0):.3f}")
        if s.get("blow_frac_outside_screen", 1.0) > 0.02:
            reasons.append(f"blow_frac_outside_screen={s.get('blow_frac_outside_screen', 0):.2%}")
        if s.get("corner_rgb_max_delta", 99.0) > 2.0:
            reasons.append(f"corner_rgb_max_delta={s.get('corner_rgb_max_delta', 0):.1f} (want ±2)")
        frame = PREVIEW_DIR / "prop-desk.frame.json"
        glb = BLENDER_ROOT / "prop-desk" / "mesh_prop-desk.glb"
        if glb.is_file():
            sha = hashlib.sha256(glb.read_bytes()).hexdigest()
            if frame.is_file():
                meta = json.loads(frame.read_text())
                if meta.get("glb_sha256") and meta.get("glb_sha256") != sha:
                    reasons.append("preview_frame_glb_sha_mismatch")
    return reasons


def glb_sha256(asset_id: str) -> str | None:
    glb = BLENDER_ROOT / asset_id / f"mesh_{asset_id}.glb"
    if not glb.is_file():
        return None
    return hashlib.sha256(glb.read_bytes()).hexdigest()


def write_frame_json(preview: Path, asset_id: str, s: dict[str, float]) -> None:
    sha = glb_sha256(asset_id)
    frame = PREVIEW_DIR / f"{asset_id}.frame.json"
    frame.write_text(
        json.dumps(
            {
                "assetId": asset_id,
                "preview": str(preview.relative_to(REPO)),
                "glb_sha256": sha,
                "resolution": 1024,
                "quality": s,
            },
            indent=2,
        )
        + "\n",
        encoding="utf-8",
    )


def main() -> int:
    failures: list[str] = []
    for aid in GATED_ASSETS:
        png = PREVIEW_DIR / f"{aid}.png"
        if not png.is_file():
            failures.append(f"{aid}: missing preview")
            continue
        s = stats(png, aid)
        write_frame_json(png, aid, s)
        bad = gate_fail(aid, s)
        if bad:
            failures.append(f"{aid}: " + "; ".join(bad))
    if failures:
        for line in failures:
            print(f"FAIL preview quality: {line}", file=sys.stderr)
        return 1
    print(
        f"OK: {len(GATED_ASSETS)} gated preview(s) pass uniform/chroma/framing gates"
    )
    return 0


if __name__ == "__main__":
    sys.exit(main())