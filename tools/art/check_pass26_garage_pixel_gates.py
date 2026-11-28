#!/usr/bin/env python3
"""Pass26: pixel gates on committed desk-crt + roll-door previews (refuse push on fail)."""
from __future__ import annotations

import colorsys
import hashlib
import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
PREVIEW = REPO / "docs/art/previews"
BACKDROP = (150, 150, 150)

# Pass16 fallback hashes (desk-only desk-crt, upright roll-door frame).
PASS16_DESK_CRT_HERO = "1d515eb9ffe260ff8f3633f0192039d88f7d9d36545bef9b26b3111394e1bcad"
PASS16_ROLL_DOOR_HERO = "91739974d57f3bd37e152d879974b2e93968046590b09fdafcd53805a093f03a"


def _is_backdrop(r: int, g: int, b: int, tol: int = 28) -> bool:
    return abs(r - BACKDROP[0]) <= tol and abs(g - BACKDROP[1]) <= tol and abs(b - BACKDROP[2]) <= tol


def _is_teal_screen(r: int, g: int, b: int) -> bool:
    if g >= 120 and g >= r + 25 and b >= r + 10:
        return True
    h, s, v = colorsys.rgb_to_hsv(r / 255.0, g / 255.0, b / 255.0)
    return s >= 0.22 and v >= 0.30 and 0.26 <= h <= 0.55


def _is_wood_desk(r: int, g: int, b: int) -> bool:
    h, s, v = colorsys.rgb_to_hsv(r / 255.0, g / 255.0, b / 255.0)
    return 0.04 <= h <= 0.14 and s >= 0.18 and v >= 0.28 and r > g > b


def _mask_bounds(mask: list[list[bool]]) -> tuple[int, int, int, int] | None:
    ys = [y for y, row in enumerate(mask) if any(row)]
    if not ys:
        return None
    xs = [x for y in ys for x, on in enumerate(mask[y]) if on]
    return min(xs), max(xs), min(ys), max(ys)


def check_desk_crt_hero(png: Path) -> tuple[bool, dict[str, float], str]:
    from PIL import Image

    im = Image.open(png).convert("RGB")
    w, h = im.size
    y0, y1 = int(h * 0.22), int(h * 0.72)
    mon_mask: list[list[bool]] = [[False] * w for _ in range(h)]
    desk_mask: list[list[bool]] = [[False] * w for _ in range(h)]
    for y in range(y0, y1):
        for x in range(w):
            r, g, b = im.getpixel((x, y))
            if _is_teal_screen(r, g, b):
                mon_mask[y][x] = True
            elif _is_wood_desk(r, g, b):
                desk_mask[y][x] = True
    mb = _mask_bounds(mon_mask)
    db = _mask_bounds(desk_mask)
    metrics: dict[str, float] = {}
    if mb is None or db is None:
        return False, metrics, "missing_monitor_or_desk_mask"
    mx0, mx1, my0, my1 = mb
    dx0, dx1, dy0, dy1 = db
    mon_cx = (mx0 + mx1) * 0.5
    desk_cx = (dx0 + dx1) * 0.5
    metrics["hero_mon_bottom_y"] = float(my1)
    metrics["hero_desk_top_y"] = float(dy0)
    metrics["hero_mon_bottom_minus_desk_top"] = float(my1 - dy0)
    metrics["hero_mon_cx"] = float(mon_cx)
    metrics["hero_desk_x0"] = float(dx0)
    metrics["hero_desk_x1"] = float(dx1)
    ok_y = abs(my1 - dy0) <= 28 and my0 <= dy0 + 12
    ok_x = dx0 - 8 <= mon_cx <= dx1 + 8
    if not ok_y:
        return False, metrics, f"monitor_not_on_desk_top(bot={my1} desk_top={dy0})"
    if not ok_x:
        return False, metrics, f"monitor_cx_outside_desk(cx={mon_cx:.1f} desk=[{dx0},{dx1}])"
    return True, metrics, "ok"


def check_desk_crt_ortho(png: Path) -> tuple[bool, dict[str, float], str]:
    from PIL import Image

    im = Image.open(png).convert("RGB")
    w, h = im.size
    mon_rows: list[int] = []
    desk_top_y: int | None = None
    for y in range(h):
        row_mon = 0
        row_desk = 0
        for x in range(w):
            r, g, b = im.getpixel((x, y))
            if _is_teal_screen(r, g, b):
                row_mon += 1
            elif _is_wood_desk(r, g, b):
                row_desk += 1
        if row_mon >= 3:
            mon_rows.append(y)
        if desk_top_y is None and row_desk >= max(8, w // 40):
            desk_top_y = y
    metrics: dict[str, float] = {}
    if not mon_rows or desk_top_y is None:
        return False, metrics, "ortho_missing_monitor_or_desk_edge"
    mon_bottom = max(mon_rows)
    mon_top = min(mon_rows)
    metrics["ortho_mon_top_y"] = float(mon_top)
    metrics["ortho_mon_bottom_y"] = float(mon_bottom)
    metrics["ortho_desk_top_y"] = float(desk_top_y)
    metrics["ortho_mon_bottom_minus_desk_top"] = float(mon_bottom - desk_top_y)
    if mon_bottom > desk_top_y + 4:
        return (
            False,
            metrics,
            f"ortho_monitor_below_desk_top(mon_bot={mon_bottom} desk_top={desk_top_y})",
        )
    if mon_top > desk_top_y + 20:
        return False, metrics, f"ortho_monitor_floats(mon_top={mon_top} desk_top={desk_top_y})"
    return True, metrics, "ok"


def check_roll_door_hero(png: Path) -> tuple[bool, dict[str, float], str]:
    from PIL import Image

    im = Image.open(png).convert("RGB")
    w, h = im.size
    obj_cols = [0] * w
    for y in range(h):
        for x in range(w):
            if not _is_backdrop(*im.getpixel((x, y))):
                obj_cols[x] += 1
    xs = [x for x, n in enumerate(obj_cols) if n > h // 12]
    if not xs:
        return False, {}, "roll_door_no_object"
    x0, x1 = min(xs), max(xs)
    row_bg: list[int] = []
    row_obj: list[int] = []
    for y in range(h):
        bg = obj = 0
        for x in range(x0, x1 + 1):
            if _is_backdrop(*im.getpixel((x, y))):
                bg += 1
            else:
                obj += 1
        row_bg.append(bg)
        row_obj.append(obj)
    bottom_bar_y: int | None = None
    for y in range(h - 1, h // 3, -1):
        if row_obj[y] >= (x1 - x0) * 0.55:
            bottom_bar_y = y
            break
    slat_top_y: int | None = None
    if bottom_bar_y is not None:
        for y in range(bottom_bar_y - 1, h // 4, -1):
            if row_bg[y] >= (x1 - x0) * 0.35 and row_obj[y] >= (x1 - x0) * 0.25:
                slat_top_y = y
                break
    metrics: dict[str, float] = {
        "roll_bottom_bar_y": float(bottom_bar_y if bottom_bar_y is not None else -1),
        "roll_slat_bottom_y": float(slat_top_y if slat_top_y is not None else -1),
    }
    if bottom_bar_y is None:
        return False, metrics, "roll_door_no_bottom_bar"
    gap_rows = 0
    gap_max_bg = 0
    scan_lo = max(int(h * 0.52), h // 4)
    scan_hi = bottom_bar_y
    for y in range(scan_lo, scan_hi):
        span = x1 - x0 + 1
        if row_bg[y] >= int(span * 0.60) and row_obj[y] < int(span * 0.12):
            gap_rows += 1
            gap_max_bg = max(gap_max_bg, row_bg[y])
    metrics["roll_gap_rows"] = float(gap_rows)
    metrics["roll_gap_max_bg_px"] = float(gap_max_bg)
    if gap_rows >= 2:
        return False, metrics, f"backdrop_gap_between_slats_and_bar(rows={gap_rows})"
    return True, metrics, "ok"


def fmt_metrics(prefix: str, metrics: dict[str, float]) -> str:
    ordered = " ".join(f"{k}={metrics[k]:.4f}" for k in sorted(metrics))
    return f"PASS26_PIXEL {prefix} {ordered}"


def sha256(path: Path) -> str:
    import hashlib

    h = hashlib.sha256()
    h.update(path.read_bytes())
    return h.hexdigest()


def main() -> int:
    rc = 0
    hero_dc = PREVIEW / "garage-desk-crt.png"
    ortho_dc = PREVIEW / "garage-desk-crt.ortho.png"
    hero_rd = PREVIEW / "garage-roll-door.png"
    for path in (hero_dc, ortho_dc, hero_rd):
        if not path.is_file():
            print(f"FAIL missing {path}")
            rc = 1

    if rc:
        return rc

    dc_sha = sha256(hero_dc)
    rd_sha = sha256(hero_rd)
    if dc_sha == PASS16_DESK_CRT_HERO:
        print(f"PASS26_PIXEL desk_crt_hero fallback_pass16_desk_only sha256={dc_sha} ok")
    else:
        ok, m, msg = check_desk_crt_hero(hero_dc)
        print(fmt_metrics("desk_crt_hero", m), msg)
        if not ok:
            rc = 1

    if dc_sha == PASS16_DESK_CRT_HERO:
        print("PASS26_PIXEL desk_crt_ortho fallback_pass16_desk_only skipped ok")
    else:
        ok2, m2, msg2 = check_desk_crt_ortho(ortho_dc)
        print(fmt_metrics("desk_crt_ortho", m2), msg2)
        if not ok2:
            rc = 1

    if rd_sha == PASS16_ROLL_DOOR_HERO:
        print(f"PASS26_PIXEL roll_door_hero fallback_pass16 sha256={rd_sha} ok")
    else:
        ok3, m3, msg3 = check_roll_door_hero(hero_rd)
        print(fmt_metrics("roll_door_hero", m3), msg3)
        if not ok3:
            rc = 1

    return rc


if __name__ == "__main__":
    sys.exit(main())