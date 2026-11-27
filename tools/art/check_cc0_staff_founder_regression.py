#!/usr/bin/env python3
"""Regression: legacy PNG fixtures must fail new face/torso/arm/framing gates."""
from __future__ import annotations

import subprocess
import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
BLENDER = Path(__import__("os").environ.get("BLENDER_BIN", "blender"))
BAKE_SCRIPT = REPO / "tools/art/blender/check_cc0_staff_founder_bake_regression_blender.py"

FIXTURES_HEADSTAND = (
    REPO / "tools/art/fixtures/staff-founder-idle-headstand-e3b3c52d.png",
    REPO / "tools/art/fixtures/staff-founder-idle-headstand-e164f581.png",
)
VISUAL_FAIL_FIXTURES = (
    REPO / "tools/art/fixtures/staff-founder-idle-procedural-fb1d6fee.png",
    REPO / "tools/art/fixtures/staff-founder-idle-jumpsuit-01e11dda.png",
    REPO / "tools/art/fixtures/staff-founder-idle-cyan-face-fb35ba42.png",
    REPO / "tools/art/fixtures/staff-founder-idle-headstand-e164f581.png",
)
ARM_FIXTURE = REPO / "tools/art/fixtures/staff-founder-idle-tpose-c17bca1a.glb"
ARM_SCRIPT = REPO / "tools/art/blender/check_cc0_staff_founder_arm_pose_blender.py"

sys.path.insert(0, str(REPO / "tools/art"))
from check_cc0_staff_founder_preview_silhouette import analyze as png_analyze  # noqa: E402
from check_cc0_staff_founder_face_read import analyze_face_read  # noqa: E402
from check_cc0_staff_founder_torso_hue import analyze_torso_hue  # noqa: E402
from check_cc0_staff_founder_arm_symmetry import analyze_arm_symmetry  # noqa: E402
from check_cc0_staff_founder_framing_width import analyze_framing_width  # noqa: E402
from check_cc0_staff_founder_face_band import analyze_face_cyan_mask  # noqa: E402


def _reject_visual_fixture(png: Path) -> tuple[bool, str]:
    if not png.is_file():
        return False, f"missing fixture {png.name}"
    checks = (
        analyze_face_read,
        analyze_torso_hue,
        analyze_arm_symmetry,
        analyze_framing_width,
    )
    for fn in checks:
        ok, msg = fn(png)
        if not ok:
            return True, msg
    ok_cyan, msg_cyan = analyze_face_cyan_mask(png)
    if not ok_cyan:
        return True, msg_cyan
    return False, "all new gates passed unexpectedly"


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
    for png in FIXTURES_HEADSTAND:
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
    seen: set[str] = set()
    for png in VISUAL_FAIL_FIXTURES:
        key = png.name
        if key in seen:
            continue
        seen.add(key)
        rejected, msg = _reject_visual_fixture(png)
        if not rejected:
            print(f"FAIL staff-founder-idle fixture_passed {key}: {msg}")
            return 1
        print(f"OK staff-founder-idle fixture_rejected {key}: {msg}")
    return 0


if __name__ == "__main__":
    sys.exit(main())