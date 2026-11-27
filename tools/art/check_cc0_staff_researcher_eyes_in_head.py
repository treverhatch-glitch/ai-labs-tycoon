#!/usr/bin/env python3
"""Dark eye pixels must lie in head face band (idle + vs-stock); GLB + PNG both required."""
from __future__ import annotations

import subprocess
import sys
import tempfile
from pathlib import Path

from PIL import Image

REPO = Path(__file__).resolve().parents[2]
IDLE_PNG = REPO / "docs/art/previews/staff-researcher-idle.png"
IDLE_MASK = REPO / "docs/art/previews/staff-researcher-idle.mask.png"
VS_PNG = REPO / "docs/art/previews/staff-researcher-vs-stock-check.png"
DEBUG_PNG = REPO / "docs/art/previews/staff-researcher-eye-gate-debug.png"

NEGATIVE_SHAS = (
    ("726005c46bf317a7318c62310875b337c43952bc", "v24"),
    ("af82592cc5a437e6f93ce8b65d7d5d24dddaa7f1", "v25"),
)
V2_FAIL_SHA = "1b8578f0"
V3_FAIL_SHA = "0d4fe3ae"
V3_DEBUG_REL = "docs/art/previews/staff-researcher-eye-gate-debug.png"
GLB_NEG_REL = "unity/Assets/Art/EarlyGame/staff-kit/staff-founder-idle.glb"

sys.path.insert(0, str(REPO / "tools" / "art"))
from staff_preview_eye_head_gate import (  # noqa: E402
    check_eyes_in_head_mask,
    write_eye_debug,
)


def _git_png(sha: str, rel: str) -> Image.Image | None:
    proc = subprocess.run(
        ["git", "show", f"{sha}:{rel}"],
        cwd=REPO,
        capture_output=True,
        check=False,
    )
    if proc.returncode != 0:
        return None
    tmp = tempfile.NamedTemporaryFile(suffix=".png", delete=False)
    tmp.write(proc.stdout)
    tmp.flush()
    return Image.open(tmp.name).convert("RGB")


def _glb_eye_gate_passes(sha: str) -> bool | None:
    proc = subprocess.run(
        ["git", "show", f"{sha}:{GLB_NEG_REL}"],
        cwd=REPO,
        capture_output=True,
        check=False,
    )
    if proc.returncode != 0 or not proc.stdout:
        return None
    glb = tempfile.NamedTemporaryFile(suffix=".glb", delete=False)
    glb.write(proc.stdout)
    glb.flush()
    glb.close()
    saved = REPO / GLB_NEG_REL
    backup = saved.read_bytes() if saved.is_file() else None
    try:
        saved.write_bytes(Path(glb.name).read_bytes())
        gate = subprocess.run(
            [sys.executable, str(REPO / "tools/art/check_cc0_staff_founder_eye_glb_world.py")],
            cwd=REPO,
            capture_output=True,
            text=True,
            check=False,
        )
        return gate.returncode == 0
    finally:
        if backup is not None:
            saved.write_bytes(backup)
        Path(glb.name).unlink(missing_ok=True)


def _v3_strict_must_fail() -> bool:
    """v3 false-passed on hair/ear blobs — strict face-plane gate must reject it."""
    rel = "docs/art/previews/staff-researcher-idle.png"
    mask_rel = "docs/art/previews/staff-researcher-idle.mask.png"
    img = _git_png(V3_FAIL_SHA, rel)
    mask = _git_png(V3_FAIL_SHA, mask_rel)
    if img is None or mask is None:
        print(f"FAIL staff-researcher-idle eyes_in_head v3_selftest missing_png sha={V3_FAIL_SHA}")
        return False
    ok, msg, blobs, _ = check_eyes_in_head_mask(
        img, mask.convert("L"), allow_projection_fallback=False
    )
    if ok or len(blobs) >= 2:
        print(
            f"FAIL staff-researcher-idle eyes_in_head v3_selftest "
            f"{V3_FAIL_SHA} must fail face_plane_gate (ok={ok} blobs={len(blobs)} {msg})"
        )
        return False
    debug = _git_png(V3_FAIL_SHA, V3_DEBUG_REL)
    if debug is not None:
        ok_d, msg_d, blobs_d, _ = check_eyes_in_head_mask(
            debug, mask.convert("L"), allow_projection_fallback=False, yaw_deg=30.0
        )
        if ok_d:
            print(
                f"FAIL staff-researcher-idle eyes_in_head v3_debug "
                f"{V3_FAIL_SHA} must_fail ({msg_d} blobs={len(blobs_d)})"
            )
            return False
    print(f"OK staff-researcher-idle eyes_in_head v3_selftest {V3_FAIL_SHA} strict_fail")
    return True


def _v2_strict_must_fail() -> bool:
    rel = "docs/art/previews/staff-researcher-idle.png"
    mask_rel = "docs/art/previews/staff-researcher-idle.mask.png"
    img = _git_png(V2_FAIL_SHA, rel)
    mask = _git_png(V2_FAIL_SHA, mask_rel)
    if img is None or mask is None:
        print(f"FAIL staff-researcher-idle eyes_in_head v2_selftest missing_png sha={V2_FAIL_SHA}")
        return False
    ok, msg, blobs, _ = check_eyes_in_head_mask(
        img, mask.convert("L"), allow_projection_fallback=False
    )
    if ok or len(blobs) >= 2:
        print(
            f"FAIL staff-researcher-idle eyes_in_head v2_selftest "
            f"{V2_FAIL_SHA} must fail strict_two_blobs (ok={ok} blobs={len(blobs)} {msg})"
        )
        return False
    print(f"OK staff-researcher-idle eyes_in_head v2_selftest {V2_FAIL_SHA} strict_fail")
    return True


def _negative_self_tests() -> bool:
    rel = "docs/art/previews/staff-researcher-idle.png"
    mask_rel = "docs/art/previews/staff-researcher-idle.mask.png"
    for sha, tag in NEGATIVE_SHAS:
        glb_ok = _glb_eye_gate_passes(sha)
        if glb_ok is None:
            print(f"FAIL staff-researcher-idle eyes_in_head negative missing_glb sha={sha[:8]}")
            return False
        if glb_ok:
            print(f"FAIL staff-researcher-idle eyes_in_head negative {tag} glb_gate should_fail")
            return False
        img = _git_png(sha, rel)
        mask = _git_png(sha, mask_rel)
        if img is None or mask is None:
            print(f"FAIL staff-researcher-idle eyes_in_head negative missing_png {tag}")
            return False
        ok, _msg, _, _ = check_eyes_in_head_mask(img, mask.convert("L"), allow_projection_fallback=False)
        if ok:
            print(
                f"FAIL staff-researcher-idle eyes_in_head negative {tag} "
                f"idle_png_must_not_pass_head_eye_gate"
            )
            return False
    print("OK staff-researcher-idle eyes_in_head negative_v24_v25")
    return True


def _glb_current_passes() -> bool:
    proc = subprocess.run(
        [sys.executable, str(REPO / "tools/art/check_cc0_staff_researcher_eye_glb_world.py")],
        cwd=REPO,
        capture_output=True,
        text=True,
        check=False,
    )
    return proc.returncode == 0


def main() -> int:
    rc = 0
    if not _negative_self_tests():
        rc = 1
    if not _glb_current_passes():
        print("FAIL staff-researcher-idle eyes_in_head glb_gate")
        rc = 1
    if not IDLE_PNG.is_file() or not IDLE_MASK.is_file():
        print("FAIL staff-researcher-idle eyes_in_head idle missing_png")
        return 1
    idle_img = Image.open(IDLE_PNG).convert("RGB")
    idle_mask = Image.open(IDLE_MASK).convert("L")
    if not _v2_strict_must_fail():
        rc = 1
    if not _v3_strict_must_fail():
        rc = 1
    ok, msg, blobs, head_rect = check_eyes_in_head_mask(
        idle_img, idle_mask, allow_projection_fallback=False
    )
    if ok and len(blobs) < 2:
        print(f"FAIL staff-researcher-idle eyes_in_head idle strict_need_two_png_blobs blobs={len(blobs)}")
        rc = 1
        ok = False
    write_eye_debug(idle_img, idle_mask, blobs, head_rect, DEBUG_PNG)
    if ok:
        print(f"OK staff-researcher-idle eyes_in_head idle {msg}")
        print(f"OK staff-researcher-idle eyes_in_head debug {DEBUG_PNG.relative_to(REPO)}")
    else:
        print(f"FAIL staff-researcher-idle eyes_in_head idle {msg}")
        rc = 1

    if VS_PNG.is_file():
        vs = Image.open(VS_PNG).convert("RGB")
        gap = 8
        gap = 8
        crop_w = (vs.width - gap) // 2
        x0 = crop_w + gap
        if vs.width > x0:
            researcher_crop = vs.crop((x0, 0, vs.width, vs.height))
            if researcher_crop.size != idle_img.size:
                researcher_crop = researcher_crop.resize(idle_img.size, Image.Resampling.LANCZOS)
            ok2, msg2, _, _ = check_eyes_in_head_mask(researcher_crop, idle_mask)
            if ok2:
                print(f"OK staff-researcher-idle eyes_in_head vs-stock {msg2}")
            else:
                print(f"FAIL staff-researcher-idle eyes_in_head vs-stock {msg2}")
                rc = 1
    return rc


if __name__ == "__main__":
    sys.exit(main())