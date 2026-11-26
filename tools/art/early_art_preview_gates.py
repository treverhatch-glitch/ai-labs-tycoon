"""Early-art preview QA: alpha-measured framing + object-mask saturation."""
from __future__ import annotations

from pathlib import Path

from preview_quality_lib import (
    alpha_bbox_metrics,
    correlation_thumb,
    downscale_rgb_crop,
    rgb_distance,
    thumb_vector,
)

MIN_SAT_OBJECT = 0.15
ENV_ALPHA_MIN = 0.35
ENV_ALPHA_MAX = 0.75
HUD_ALPHA_MIN = 0.60
HUD_ALPHA_MAX = 0.80
MAX_PAIR_CORR = 0.94
MIN_RGB_DISTANCE = 14.0


def gate_preview_env(path: Path) -> tuple[bool, str]:
    m = alpha_bbox_metrics(path, hud_mid_grey=False)
    if m["sat"] < MIN_SAT_OBJECT:
        return False, f"object_mask_sat={m['sat']:.3f} < {MIN_SAT_OBJECT}"
    if m["edge_touch"]:
        return False, "edge_touch"
    lo = min(m["fill_w"], m["fill_h"])
    hi = max(m["fill_w"], m["fill_h"])
    if lo < ENV_ALPHA_MIN:
        return False, f"alpha_min={lo:.3f} < {ENV_ALPHA_MIN}"
    if hi > ENV_ALPHA_MAX:
        return False, f"alpha_max={hi:.3f} > {ENV_ALPHA_MAX}"
    return True, f"sat={m['sat']:.3f} alpha_h={m['fill_h']:.2f} w={m['fill_w']:.2f}"


def gate_preview_hud(path: Path) -> tuple[bool, str]:
    m = alpha_bbox_metrics(path, hud_mid_grey=True)
    if m["sat"] < MIN_SAT_OBJECT:
        return False, f"object_mask_sat={m['sat']:.3f} < {MIN_SAT_OBJECT}"
    if m["edge_touch"]:
        return False, "edge_touch"
    lo = min(m["fill_w"], m["fill_h"])
    hi = max(m["fill_w"], m["fill_h"])
    if hi < HUD_ALPHA_MIN:
        return False, f"ortho_max={hi:.3f} < {HUD_ALPHA_MIN}"
    if hi > HUD_ALPHA_MAX:
        return False, f"ortho_max={hi:.3f} > {HUD_ALPHA_MAX}"
    if lo < 0.22:
        return False, f"ortho_min={lo:.3f} < 0.22"
    return True, f"sat={m['sat']:.3f} ortho_h={m['fill_h']:.2f} w={m['fill_w']:.2f}"


def check_near_duplicates(
    preview_dir: Path, asset_ids: tuple[str, ...], *, hud: bool = False
) -> tuple[int, list[str]]:
    rc = 0
    msgs: list[str] = []
    vectors: dict[str, list] = {}
    for asset_id in asset_ids:
        path = preview_dir / f"{asset_id}.png"
        if path.exists():
            vectors[asset_id] = (
                downscale_rgb_crop(path) if hud else thumb_vector(path)
            )
    keys = list(vectors.keys())
    for i, a in enumerate(keys):
        for b in keys[i + 1 :]:
            if hud:
                dist = rgb_distance(vectors[a], vectors[b])
                if dist < MIN_RGB_DISTANCE:
                    msgs.append(f"FAIL near-identical {a} vs {b} dist={dist:.2f}")
                    rc = 1
            else:
                corr = correlation_thumb(vectors[a], vectors[b])
                if corr > MAX_PAIR_CORR:
                    msgs.append(f"FAIL near-identical {a} vs {b} corr={corr:.3f}")
                    rc = 1
    if rc == 0 and keys:
        msgs.append(f"OK uniqueness ({len(keys)} previews)")
    return rc, msgs