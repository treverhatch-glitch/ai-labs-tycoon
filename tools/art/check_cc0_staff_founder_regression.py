#!/usr/bin/env python3
"""Regression: legacy bake + headstand PNG fixtures must fail upright gates."""
from __future__ import annotations

import subprocess
import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
BLENDER = Path(__import__("os").environ.get("BLENDER_BIN", "blender"))
BAKE_SCRIPT = REPO / "tools/art/blender/check_cc0_staff_founder_bake_regression_blender.py"

FIXTURES = (
    REPO / "tools/art/fixtures/staff-founder-idle-headstand-e3b3c52d.png",
    REPO / "tools/art/fixtures/staff-founder-idle-headstand-e164f581.png",
)
ARM_FIXTURE = REPO / "tools/art/fixtures/staff-founder-idle-tpose-c17bca1a.glb"
ARM_SCRIPT = REPO / "tools/art/blender/check_cc0_staff_founder_arm_pose_blender.py"
FACE_FIXTURE = REPO / "tools/art/fixtures/staff-founder-idle-cyan-face-fb35ba42.png"
JUMPSUIT_FIXTURE = REPO / "tools/art/fixtures/staff-founder-idle-jumpsuit-01e11dda.png"

sys.path.insert(0, str(REPO / "tools/art"))
from check_cc0_staff_founder_preview_silhouette import analyze as png_analyze  # noqa: E402
from check_cc0_staff_founder_eye_blobs import analyze_eye_blobs  # noqa: E402
from check_cc0_staff_founder_head_highlight import analyze_head_highlight  # noqa: E402
from check_cc0_staff_founder_face_band import analyze_face_cyan_mask  # noqa: E402


def main() -> int:
    proc = subprocess.run(
        [str(BLENDER), "-b", "--python", str(BAKE_SCRIPT), "--", str(REPO)],
        cwd=REPO,
        text=True,
        capture_output=True,
    )
    if proc.stdout:
        for line in proc.stdout.splitlines():
            if line.startswith("OK ") or line.startswith("FAIL "):
                print(line)
    if proc.returncode != 0:
        return 1
    for png in FIXTURES:
        if not png.is_file():
            print(f"FAIL staff-founder-idle missing fixture {png.name}")
            return 1
        ok, msg = png_analyze(png)
        if ok:
            print(f"FAIL staff-founder-idle fixture_passed {png.name}: {msg}")
            return 1
        if "headstand" not in msg and "head" not in msg:
            print(f"FAIL staff-founder-idle fixture_unexpected {png.name}: {msg}")
            return 1
        print(f"OK staff-founder-idle fixture_rejected {png.name}: {msg}")
    if not ARM_FIXTURE.is_file():
        print(f"FAIL staff-founder-idle missing fixture {ARM_FIXTURE.name}")
        return 1
    arm = subprocess.run(
        [str(BLENDER), "-b", "--python", str(ARM_SCRIPT), "--", str(REPO), str(ARM_FIXTURE)],
        cwd=REPO,
        text=True,
        capture_output=True,
    )
    fail_line = ""
    for line in (arm.stdout or "").splitlines():
        if line.startswith("FAIL staff-founder-idle arm_"):
            fail_line = line
            break
    if arm.returncode == 0:
        print("FAIL staff-founder-idle fixture_arm_passed c17bca1a")
        return 1
    if not fail_line:
        tail = (arm.stdout or "").strip().splitlines()[-1:] if arm.stdout else []
        print(f"FAIL staff-founder-idle fixture_arm_unexpected {tail}")
        return 1
    print(f"OK staff-founder-idle fixture_rejected {ARM_FIXTURE.name}: {fail_line}")
    if not FACE_FIXTURE.is_file():
        print(f"FAIL staff-founder-idle missing fixture {FACE_FIXTURE.name}")
        return 1
    ok_cyan, msg_cyan = analyze_face_cyan_mask(FACE_FIXTURE)
    if ok_cyan:
        print(f"FAIL staff-founder-idle fixture_face_passed {FACE_FIXTURE.name}: {msg_cyan}")
        return 1
    if "cyan" not in msg_cyan:
        print(f"FAIL staff-founder-idle fixture_face_unexpected {FACE_FIXTURE.name}: {msg_cyan}")
        return 1
    print(f"OK staff-founder-idle fixture_rejected {FACE_FIXTURE.name}: {msg_cyan}")
    for label, path in (("01e11dda", JUMPSUIT_FIXTURE), ("fb35ba42", FACE_FIXTURE)):
        if not path.is_file():
            print(f"FAIL staff-founder-idle missing fixture {path.name}")
            return 1
        ok_eyes, msg_eyes = analyze_eye_blobs(path)
        ok_hi, msg_hi = analyze_head_highlight(path)
        if ok_eyes and ok_hi:
            print(f"FAIL staff-founder-idle fixture_passed_{label} eyes={msg_eyes} hi={msg_hi}")
            return 1
        print(f"OK staff-founder-idle fixture_rejected {path.name}: {msg_eyes}; {msg_hi}")
    return 0


if __name__ == "__main__":
    sys.exit(main())