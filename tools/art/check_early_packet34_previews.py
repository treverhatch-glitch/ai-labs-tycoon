#!/usr/bin/env python3
"""Packet 3–4 Hatch hard gates (packet34 bar)."""
from __future__ import annotations

import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
PREVIEW = REPO / "docs/art/previews"
sys.path.insert(0, str(REPO / "tools/art"))

from check_env_glb_packet34 import check_all_env_glbs  # noqa: E402
from check_env_preview_glb_linkage import main as check_preview_glb_linkage  # noqa: E402

TEX_DIR = REPO / "unity/Assets/Art/EarlyGame/textures"

from packet34_preview_gates import (  # noqa: E402
    check_sign_assembly_metadata,
    check_sign_front_board_frame_width,
    check_sign_front_ocr_words,
    check_sign_front_posts_to_ground,
    check_sign_preview_subject_framing,
    check_sign_preview_yaw,
    check_sign_text_board_wcag_contrast,
    check_sign_text_inside_board_mask,
    ALL_HUD_SPRITE_IDS,
    ICON_IDS,
    check_backdrop_mid_grey,
    check_sign_preview_png_scope,
    check_env_preview_backdrop,
    check_wall_cinderblock_visible,
    check_wall_mortar_horizontal,
    check_wall_uv_blocks_per_metre,
    check_sign_albedo_layout,
    check_sign_front_red_inside_board,
    check_sign_preview_board_text,
    check_sign_text_template_orientation,
    check_sign_for_sale_contrast,
    check_curb_preview_concrete,
    check_sign_albedo_file,
    check_icon_ortho,
    check_icon_sat_coverage,
    check_icon_sheet,
    check_near_duplicates,
    check_texture_swatch,
    check_tile2x2_seam,
)
from preview_quality_lib import mask_gate_errors  # noqa: E402

ENV_IDS = (
    "garage-exterior-wall",
    "garage-land-for-sale-sign",
    "garage-lot-curb",
    "garage-window-frame",
)
TEX_IDS = ("tex-hud-paper-warm", "tex-garage-wall-beige", "tex-garage-trim-orange")


def check_env_preview(asset_id: str) -> int:
    rc = 0
    rgb = PREVIEW / f"{asset_id}.png"
    mask = PREVIEW / f"{asset_id}.mask.png"
    meta = PREVIEW / f"{asset_id}.frame.json"
    if not rgb.is_file():
        print(f"FAIL {asset_id} missing rgb")
        return 1
    if asset_id != "garage-land-for-sale-sign" and (not mask.is_file() or not meta.is_file()):
        print(f"FAIL {asset_id} missing rgb/mask/frame")
        return 1
    if asset_id == "garage-land-for-sale-sign" and not meta.is_file():
        print(f"FAIL {asset_id} missing frame")
        return 1
    if asset_id != "garage-land-for-sale-sign":
        for err in check_backdrop_mid_grey(rgb):
            print(f"FAIL {asset_id} {err}")
            rc = 1
    if mask.is_file():
        for err in check_env_preview_backdrop(asset_id, rgb, mask):
            print(f"FAIL {asset_id} {err}")
            rc = 1
    if asset_id == "garage-exterior-wall":
        for err in check_wall_uv_blocks_per_metre(meta):
            print(f"FAIL {asset_id} {err}")
            rc = 1
        for err in check_wall_cinderblock_visible(rgb, mask, TEX_DIR):
            print(f"FAIL {asset_id} {err}")
            rc = 1
        for err in check_wall_mortar_horizontal(rgb, mask):
            print(f"FAIL {asset_id} {err}")
            rc = 1
    if asset_id == "garage-land-for-sale-sign":
        sign_tex = REPO / "unity/Assets/Art/EarlyGame/garage-kit/textures/garage-land-for-sale-sign"
        for err in check_sign_albedo_file(sign_tex):
            print(f"FAIL {asset_id} {err}")
            rc = 1
        for err in check_sign_albedo_layout(sign_tex):
            print(f"FAIL {asset_id} {err}")
            rc = 1
        front_rgb = PREVIEW / "garage-land-for-sale-sign-front.png"
        front_mask = PREVIEW / "garage-land-for-sale-sign-front.mask.png"
        for err in check_backdrop_mid_grey(front_rgb):
            print(f"FAIL {asset_id} front {err}")
            rc = 1
        for err in check_sign_preview_board_text(front_rgb, front_mask, label="sign_front"):
            print(f"FAIL {asset_id} {err}")
            rc = 1
        for err in check_sign_text_template_orientation(front_rgb, front_mask, label="sign_front"):
            print(f"FAIL {asset_id} {err}")
            rc = 1
        front_frame = PREVIEW / "garage-land-for-sale-sign-front.frame.json"
        for err in check_sign_front_ocr_words(front_rgb):
            print(f"FAIL {asset_id} {err}")
            rc = 1
        for err in check_sign_front_board_frame_width(front_rgb, front_mask):
            print(f"FAIL {asset_id} {err}")
            rc = 1
        for err in check_sign_preview_yaw(front_frame, expected_deg=0.0, label="sign_front"):
            print(f"FAIL {asset_id} {err}")
            rc = 1
        for err in check_sign_assembly_metadata(front_frame, label="sign_front"):
            print(f"FAIL {asset_id} {err}")
            rc = 1
        for err in check_sign_preview_subject_framing(front_rgb, front_mask, label="sign_front"):
            print(f"FAIL {asset_id} {err}")
            rc = 1
        for err in check_sign_text_inside_board_mask(front_rgb, front_mask, label="sign_front"):
            print(f"FAIL {asset_id} {err}")
            rc = 1
        for err in check_sign_text_board_wcag_contrast(front_rgb, front_mask, label="sign_front"):
            print(f"FAIL {asset_id} {err}")
            rc = 1
        for err in check_sign_front_posts_to_ground(front_rgb, front_mask):
            print(f"FAIL {asset_id} {err}")
            rc = 1
    if asset_id == "garage-lot-curb":
        for err in check_curb_preview_concrete(rgb, mask):
            print(f"FAIL {asset_id} {err}")
            rc = 1
    if mask.is_file():
        for err in mask_gate_errors(mask):
            print(f"FAIL {asset_id} {err}")
            rc = 1
    if rc == 0:
        print(f"OK {asset_id} env_preview")
    return rc


def main() -> int:
    try:
        from PIL import Image  # noqa: F401
    except ImportError:
        print("FAIL: Pillow required")
        return 1

    rc = check_all_env_glbs()
    rc |= check_preview_glb_linkage()
    for asset_id in ENV_IDS:
        rc |= check_env_preview(asset_id)
    hero_rgb = PREVIEW / "garage-land-for-sale-sign-hero.png"
    hero_mask = PREVIEW / "garage-land-for-sale-sign-hero.mask.png"
    hero_rc = 0
    if hero_rgb.is_file() and hero_mask.is_file():
        for err in check_backdrop_mid_grey(hero_rgb):
            print(f"FAIL garage-land-for-sale-sign-hero {err}")
            hero_rc = 1
        for err in check_sign_for_sale_contrast(hero_rgb, hero_mask):
            print(f"FAIL garage-land-for-sale-sign-hero {err}")
            hero_rc = 1
        for err in check_sign_preview_board_text(hero_rgb, hero_mask, label="sign_hero"):
            print(f"FAIL garage-land-for-sale-sign-hero {err}")
            hero_rc = 1
        for err in check_sign_text_template_orientation(hero_rgb, hero_mask, label="sign_hero"):
            print(f"FAIL garage-land-for-sale-sign-hero {err}")
            hero_rc = 1
        hero_frame = PREVIEW / "garage-land-for-sale-sign-hero.frame.json"
        for err in check_sign_preview_yaw(hero_frame, expected_deg=35.0, label="sign_hero"):
            print(f"FAIL garage-land-for-sale-sign-hero {err}")
            hero_rc = 1
        for err in check_sign_assembly_metadata(hero_frame, label="sign_hero"):
            print(f"FAIL garage-land-for-sale-sign-hero {err}")
            hero_rc = 1
        for err in check_sign_preview_subject_framing(hero_rgb, hero_mask, label="sign_hero"):
            print(f"FAIL garage-land-for-sale-sign-hero {err}")
            hero_rc = 1
        for err in check_sign_text_inside_board_mask(hero_rgb, hero_mask, label="sign_hero"):
            print(f"FAIL garage-land-for-sale-sign-hero {err}")
            hero_rc = 1
        for err in check_sign_text_board_wcag_contrast(hero_rgb, hero_mask, label="sign_hero"):
            print(f"FAIL garage-land-for-sale-sign-hero {err}")
            hero_rc = 1
        if hero_rc == 0:
            print("OK garage-land-for-sale-sign-hero sign_hero")
        rc |= hero_rc
    else:
        print("FAIL garage-land-for-sale-sign-hero missing rgb/mask")
        rc = 1
    scope_errs = check_sign_preview_png_scope(PREVIEW)
    if scope_errs:
        for err in scope_errs:
            print(f"FAIL sign_scope {err}")
        rc |= 1
    else:
        print("OK sign_preview_png_scope")
    for asset_id in ALL_HUD_SPRITE_IDS:
        errs = check_icon_ortho(PREVIEW / f"{asset_id}.frame.json")
        errs.extend(check_icon_sat_coverage(asset_id, PREVIEW))
        if errs:
            for err in errs:
                print(f"FAIL {asset_id} {err}")
            rc |= 1
        else:
            print(f"OK {asset_id} hud_sprite")
    for err in check_near_duplicates(PREVIEW, ICON_IDS):
        print(f"FAIL {err}")
        rc |= 1
    for err in check_near_duplicates(PREVIEW, TEX_IDS, min_sim=0.90):
        print(f"FAIL {err}")
        rc |= 1
    for asset_id in TEX_IDS:
        tex_errs = check_texture_swatch(PREVIEW, asset_id)
        if asset_id == "tex-garage-trim-orange":
            tex_errs.extend(check_tile2x2_seam(PREVIEW, asset_id))
        if tex_errs:
            for err in tex_errs:
                print(f"FAIL {asset_id} {err}")
            rc |= 1
        else:
            print(f"OK {asset_id} texture_swatch")
    for err in check_icon_sheet(PREVIEW):
        print(f"FAIL {err}")
        rc |= 1
    else:
        print("OK icon-sheet-64")
    from check_preview_sign_regression import main as check_sign_regression  # noqa: E402

    rc |= check_sign_regression()
    return rc


if __name__ == "__main__":
    sys.exit(main())