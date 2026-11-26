"""Shared early-art preview QA (Hatch #88 bar: object mask, bbox fill, duplicates)."""
from __future__ import annotations

import math
from pathlib import Path


MIN_SAT_OBJECT = 0.15
MIN_LUMA_VAR = 0.004
BBOX_FILL_MIN = 0.34  # 35% nominal + 1px pad at 512²
BBOX_FILL_MAX = 0.75
MAX_PAIR_CORR = 0.94


def _pixel_sat(r: int, g: int, b: int) -> float:
    rf, gf, bf = r / 255.0, g / 255.0, b / 255.0
    mx = max(rf, gf, bf)
    mn = min(rf, gf, bf)
    if mx <= 1e-6:
        return 0.0
    return (mx - mn) / mx


def _dist_rgb(a: tuple[int, int, int], b: tuple[int, int, int]) -> float:
    return math.sqrt(sum((x - y) ** 2 for x, y in zip(a, b)))


def _backdrop_rgb(im) -> tuple[int, int, int]:
    w, h = im.size
    samples = [
        im.getpixel((2, 2)),
        im.getpixel((w - 3, 2)),
        im.getpixel((2, h - 3)),
        im.getpixel((w - 3, h - 3)),
        im.getpixel((w // 2, 2)),
        im.getpixel((w // 2, h - 3)),
    ]
    r = sum(p[0] for p in samples) // len(samples)
    g = sum(p[1] for p in samples) // len(samples)
    b = sum(p[2] for p in samples) // len(samples)
    return (r, g, b)


def analyze_preview(path: Path) -> dict:
    from PIL import Image

    im = Image.open(path).convert("RGB")
    w, h = im.size
    bg = _backdrop_rgb(im)
    mask: list[tuple[int, int, int]] = []
    xs: list[int] = []
    ys: list[int] = []
    lum: list[float] = []
    pad_x, pad_y = int(w * 0.04), int(h * 0.05)
    floor_cut = int(h * 0.84)
    for y in range(h):
        for x in range(w):
            rgb = im.getpixel((x, y))
            rf, gf, bf = [c / 255.0 for c in rgb]
            lum_v = 0.2126 * rf + 0.7152 * gf + 0.0722 * bf
            sat = _pixel_sat(*rgb)
            if sat > 0.065 and _dist_rgb(rgb, bg) > 16:
                mask.append(rgb)
                lum.append(lum_v)
                if (
                    pad_x <= x < w - pad_x
                    and pad_y <= y < floor_cut
                ):
                    xs.append(x)
                    ys.append(y)
    if not mask:
        return {"sat": 0.0, "lvar": 0.0, "fill_w": 0.0, "fill_h": 0.0}
    sats = [_pixel_sat(*p) for p in mask]
    vivid = [s for s in sats if s > 0.045] or sats
    sat = sum(vivid) / len(vivid)
    lbar = sum(lum) / len(lum)
    lvar = sum((x - lbar) ** 2 for x in lum) / len(lum)
    bw = (max(xs) - min(xs) + 1) / w
    bh = (max(ys) - min(ys) + 1) / h
    return {"sat": sat, "lvar": lvar, "fill_w": bw, "fill_h": bh}


def gate_preview(path: Path) -> tuple[bool, str]:
    m = analyze_preview(path)
    if m["sat"] < MIN_SAT_OBJECT:
        return False, f"object_mask_sat={m['sat']:.3f} < {MIN_SAT_OBJECT}"
    if m["lvar"] < MIN_LUMA_VAR:
        return False, f"object_luma_var={m['lvar']:.4f} < {MIN_LUMA_VAR}"
    lo = min(m["fill_w"], m["fill_h"])
    hi = max(m["fill_w"], m["fill_h"])
    if lo < BBOX_FILL_MIN:
        return False, f"bbox_min={lo:.3f} < {BBOX_FILL_MIN}"
    if hi > BBOX_FILL_MAX:
        return False, f"bbox_max={hi:.3f} > {BBOX_FILL_MAX}"
    return True, f"sat={m['sat']:.3f} lvar={m['lvar']:.4f} w={m['fill_w']:.2f} h={m['fill_h']:.2f}"


def thumb_vector(path: Path) -> list[float]:
    from PIL import Image

    im = Image.open(path).convert("L")
    w, h = im.size
    im = im.crop((int(w * 0.12), int(h * 0.12), int(w * 0.88), int(h * 0.88)))
    im = im.resize((48, 48))
    data = [v / 255.0 for v in im.get_flattened_data()]
    avg = sum(data) / len(data)
    return [(v - avg) for v in data]


def correlation(a: list[float], b: list[float]) -> float:
    dot = sum(x * y for x, y in zip(a, b))
    na = sum(x * x for x in a) ** 0.5
    nb = sum(y * y for y in b) ** 0.5
    if na < 1e-9 or nb < 1e-9:
        return 0.0
    return dot / (na * nb)


def check_near_duplicates(preview_dir: Path, asset_ids: tuple[str, ...]) -> tuple[int, list[str]]:
    rc = 0
    msgs: list[str] = []
    vectors: dict[str, list[float]] = {}
    for asset_id in asset_ids:
        path = preview_dir / f"{asset_id}.png"
        if path.exists():
            vectors[asset_id] = thumb_vector(path)
    keys = list(vectors.keys())
    for i, a in enumerate(keys):
        for b in keys[i + 1 :]:
            corr = correlation(vectors[a], vectors[b])
            if corr > MAX_PAIR_CORR:
                msgs.append(f"FAIL near-identical {a} vs {b} corr={corr:.3f}")
                rc = 1
    if rc == 0 and keys:
        msgs.append(f"OK uniqueness ({len(keys)} previews)")
    return rc, msgs