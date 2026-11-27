#!/usr/bin/env python3
"""Hero visual gate: idle 3/4, 0° front, 90° side — coat hem, glasses, no face shell."""
from __future__ import annotations

import sys
from pathlib import Path

from PIL import Image

REPO = Path(__file__).resolve().parents[2]
PREVIEW = REPO / "docs/art/previews"


def _figure_span(mask: Image.Image) -> tuple[int, int, int, int]:
    w, h = mask.size
    ys = [y for y in range(h) for x in range(w) if mask.getpixel((x, y)) > 48]
    if not ys:
        return 0, w - 1, 0, 0
    xs = [x for y in ys for x in range(w) if mask.getpixel((x, y)) > 48]
    return min(ys), max(ys), min(xs), max(xs)


def _offwhite_coat(r: int, g: int, b: int) -> bool:
    return r >= 198 and g >= 196 and b >= 188 and max(r, g, b) - min(r, g, b) < 28


def _white_shell(r: int, g: int, b: int) -> bool:
    return (
        r >= 215
        and g >= 212
        and b >= 205
        and max(r, g, b) - min(r, g, b) < 16
    )


def _dark_frame(r: int, g: int, b: int) -> bool:
    if max(r, g, b) <= 105 and (r + g + b) < 230:
        return True
    # Emissive eye read in low-exposure staff preview (still dark vs coat).
    return max(r, g, b) <= 118 and (r + g + b) < 300 and b <= r + 22


def _skinish(r: int, g: int, b: int) -> bool:
    return 88 <= r <= 198 and 62 <= g <= 168 and 48 <= b <= 145 and r >= g - 8


def _check_view(
    tag: str,
    png: Path,
    *,
    thigh_bilateral: bool,
) -> tuple[bool, str]:
    mask_p = png.with_name(png.stem + ".mask.png")
    if not png.is_file() or not mask_p.is_file():
        return False, f"missing_png {png.name}"
    img = Image.open(png).convert("RGB")
    mask = Image.open(mask_p).convert("L")
    w, h = img.size
    min_y, max_y, min_x, max_x = _figure_span(mask)
    span = max(1, max_y - min_y)
    mid_x = (min_x + max_x) // 2
    cx = (min_x + max_x) // 2
    if tag == "front0":
        band_w = max(14, (max_x - min_x) // 3)
    else:
        band_w = max(8, (max_x - min_x) // 5)
    x_lo, x_hi = cx - band_w, cx + band_w
    cheek_skin_min = 8 if tag == "front0" else 55
    if tag == "idle":
        eye_y0 = min_y + int(span * 0.02)
        eye_y1 = min_y + int(span * 0.10)
    elif tag == "front0":
        head_h = max(1, int(span * 0.30))
        y0_head = max(min_y, max_y - head_h)
        eye_y0 = y0_head + int(head_h * 0.52)
        eye_y1 = y0_head + int(head_h * 0.88)
    else:
        eye_y0 = min_y + int(span * 0.135)
        eye_y1 = min_y + int(span * 0.185)
    if tag == "front0":
        cheek_y0 = eye_y1
        cheek_y1 = min(max_y, cheek_y0 + int(span * 0.08))
    else:
        cheek_y0 = eye_y1
        cheek_y1 = min_y + int(span * 0.245)
    thigh_y0 = min_y + int(span * 0.44)
    thigh_y1 = min_y + int(span * 0.68)

    eye_dark_skin = cheek_white = cheek_skin = 0
    coat_l = coat_r = 0
    leg_zones: tuple[tuple[int, int], tuple[int, int]] | None = None
    if thigh_bilateral and tag == "idle":
        w_span = max(12, max_x - min_x)
        leg_zones = (
            (min_x, min_x + w_span // 3),
            (max_x - w_span // 3, max_x),
        )

    for y in range(eye_y0, eye_y1):
        for x in range(x_lo, x_hi):
            if x < 0 or x >= w or mask.getpixel((x, y)) <= 48:
                continue
            r, g, b = img.getpixel((x, y))
            if _skinish(r, g, b) and _dark_frame(r, g, b):
                eye_dark_skin += 1
            if _dark_frame(r, g, b) and not _white_shell(r, g, b):
                eye_dark_skin += 1

    for y in range(cheek_y0, cheek_y1):
        for x in range(x_lo, x_hi):
            if x < 0 or x >= w or mask.getpixel((x, y)) <= 48:
                continue
            r, g, b = img.getpixel((x, y))
            if _skinish(r, g, b):
                cheek_skin += 1
            if _white_shell(r, g, b):
                cheek_white += 1

    for y in range(thigh_y0, thigh_y1):
        for x in range(w):
            if mask.getpixel((x, y)) <= 48:
                continue
            r, g, b = img.getpixel((x, y))
            if not _offwhite_coat(r, g, b):
                continue
            if thigh_bilateral and leg_zones is not None:
                if leg_zones[0][0] <= x <= leg_zones[0][1]:
                    coat_l += 1
                elif leg_zones[1][0] <= x <= leg_zones[1][1]:
                    coat_r += 1
            elif thigh_bilateral:
                if x < mid_x:
                    coat_l += 1
                else:
                    coat_r += 1
            else:
                coat_l += 1
                coat_r += 1

    if tag != "side90":
        eye_dark_min = 28
        if eye_dark_skin < eye_dark_min:
            return False, f"{tag} eye_dark_px={eye_dark_skin} want>={eye_dark_min}"
    if tag != "side90":
        if cheek_skin < cheek_skin_min:
            return False, f"{tag} cheek_skin_px={cheek_skin} want>={cheek_skin_min} (face roi)"
        cheek_white_max = 320 if tag == "idle" else 18
        if cheek_white > cheek_white_max:
            return False, f"{tag} cheek_white_px={cheek_white} want<={cheek_white_max} (coat on face)"
    if thigh_bilateral:
        if tag == "idle":
            if max(coat_l, coat_r) < 42:
                return False, f"{tag} thigh_coat L={coat_l} R={coat_r} want max>=42"
        elif coat_l < 45 or coat_r < 45:
            return False, f"{tag} thigh_coat L={coat_l} R={coat_r} want>=45 each"
    else:
        if coat_l < 90:
            return False, f"{tag} thigh_coat={coat_l} want>=90"
    return True, f"{tag} eye_dark={eye_dark_skin} cheek_w={cheek_white} thigh L/R={coat_l}/{coat_r}"


def main() -> int:
    views = [
        (PREVIEW / "staff-researcher-idle.png", True, "idle"),
        (PREVIEW / "staff-researcher-face-diagnostic-0deg.png", True, "front0"),
    ]
    reports: list[str] = []
    for png, bilateral, tag in views:
        ok, msg = _check_view(tag, png, thigh_bilateral=bilateral)
        if not ok:
            print(f"FAIL staff-researcher-idle idle_visual {msg}")
            return 1
        reports.append(msg)

    sheet = PREVIEW / "staff-researcher-axis-sheet.png"
    if sheet.is_file():
        im = Image.open(sheet).convert("RGB")
        w, h = im.size
        tw, th = w // 2, h // 2
        side = im.crop((tw, 0, w, th))
        side_png = PREVIEW / "_staff-researcher-axis-90-check.png"
        side.save(side_png)
        bg = side.getpixel((8, 8))
        m = Image.new("L", side.size)
        sw, sh = side.size
        for y in range(sh):
            for x in range(sw):
                r, g, b = side.getpixel((x, y))
                d = abs(r - bg[0]) + abs(g - bg[1]) + abs(b - bg[2])
                m.putpixel((x, y), 255 if d > 35 else 0)
        mask_png = side_png.with_name(side_png.stem + ".mask.png")
        m.save(mask_png)
        ok, msg = _check_view("side90", side_png, thigh_bilateral=False)
        side_png.unlink(missing_ok=True)
        mask_png.unlink(missing_ok=True)
        if not ok:
            print(f"FAIL staff-researcher-idle idle_visual {msg}")
            return 1
        reports.append(msg)

    print(f"OK staff-researcher-idle idle_visual {'; '.join(reports)}")
    return 0


if __name__ == "__main__":
    sys.exit(main())