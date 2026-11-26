#!/usr/bin/env python3
"""Low-poly staff silhouettes for dial-up era (CC0 procedural)."""
from __future__ import annotations

import json
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
from generate_campus_kit import Mesh, box, merge  # noqa: E402

REPO_ROOT = Path(__file__).resolve().parents[2]
OUT_DIR = REPO_ROOT / "unity" / "Assets" / "Art" / "EarlyGame" / "staff-kit"


def _person(body_scale: float = 1.0, hat: bool = False, arms_down: bool = True) -> Mesh:
    torso = box(0.38 * body_scale, 0.55 * body_scale, 0.22 * body_scale, oy=0.95 * body_scale)
    head = box(0.26 * body_scale, 0.26 * body_scale, 0.24 * body_scale, oy=1.42 * body_scale)
    leg_l = box(0.12 * body_scale, 0.55 * body_scale, 0.12 * body_scale, -0.1, 0.275 * body_scale)
    leg_r = box(0.12 * body_scale, 0.55 * body_scale, 0.12 * body_scale, 0.1, 0.275 * body_scale)
    parts = merge(torso, head, leg_l, leg_r)
    if arms_down:
        arm_l = box(0.1 * body_scale, 0.42 * body_scale, 0.1 * body_scale, -0.28, 1.0 * body_scale)
        arm_r = box(0.1 * body_scale, 0.42 * body_scale, 0.1 * body_scale, 0.28, 1.0 * body_scale)
        parts = merge(parts, arm_l, arm_r)
    else:
        arm_l = box(0.1 * body_scale, 0.35 * body_scale, 0.1 * body_scale, -0.22, 1.15 * body_scale, 0.12)
        arm_r = box(0.1 * body_scale, 0.35 * body_scale, 0.1 * body_scale, 0.22, 1.15 * body_scale, -0.08)
        parts = merge(parts, arm_l, arm_r)
    if hat:
        brim = box(0.32 * body_scale, 0.04 * body_scale, 0.32 * body_scale, oy=1.58 * body_scale)
        parts = merge(parts, brim)
    return parts


def staff_researcher_idle() -> Mesh:
    return _person(hat=False, arms_down=True)


def staff_engineer_work() -> Mesh:
    base = _person(hat=True, arms_down=False)
    laptop = box(0.35, 0.03, 0.25, 0.0, 1.0, 0.2)
    return merge(base, laptop)


def staff_safety_idle() -> Mesh:
    base = _person(hat=False, arms_down=True)
    clip = box(0.12, 0.16, 0.04, 0.22, 0.95, 0.12)
    return merge(base, clip)


def staff_sales_idle() -> Mesh:
    base = _person(hat=False, arms_down=True)
    phone = box(0.08, 0.22, 0.06, 0.25, 1.05, 0.1)
    return merge(base, phone)


def staff_recruiter_idle() -> Mesh:
    base = _person(hat=False, arms_down=True)
    folder = box(0.18, 0.24, 0.04, -0.25, 0.95, 0.08)
    return merge(base, folder)


def staff_gpu_tech_idle() -> Mesh:
    base = _person(hat=True, arms_down=False)
    screwdriver = box(0.04, 0.18, 0.04, 0.3, 1.12, 0.05)
    return merge(base, screwdriver)


def staff_founder_idle() -> Mesh:
    base = _person(hat=False, arms_down=True)
    mug = box(0.1, 0.12, 0.1, 0.28, 0.98, 0.05)
    return merge(base, mug)


def staff_pose_typing() -> Mesh:
    return staff_engineer_work()


MESH_BUILDERS: dict[str, callable] = {
    "staff-researcher-idle": staff_researcher_idle,
    "staff-engineer-work": staff_engineer_work,
    "staff-safety-idle": staff_safety_idle,
    "staff-sales-idle": staff_sales_idle,
    "staff-recruiter-idle": staff_recruiter_idle,
    "staff-gpu-tech-idle": staff_gpu_tech_idle,
    "staff-founder-idle": staff_founder_idle,
    "staff-pose-typing": staff_pose_typing,
}


def main() -> None:
    manifest_entries = []
    for asset_id, builder in MESH_BUILDERS.items():
        mesh = builder()
        mesh.name = asset_id
        out = OUT_DIR / f"{asset_id}.obj"
        mesh.write_obj(out)
        manifest_entries.append(
            {
                "id": asset_id,
                "path": f"Assets/Art/EarlyGame/staff-kit/{asset_id}.obj",
                "license": "CC0-1.0 (procedural, in-repo)",
                "vertexCount": len(mesh.vertices),
                "faceCount": len(mesh.faces),
            }
        )

    manifest_path = REPO_ROOT / "unity" / "Assets" / "Art" / "EarlyGame" / "staff-manifest.json"
    manifest_path.write_text(
        json.dumps(
            {
                "generator": "tools/art/generate_early_staff.py",
                "assets": manifest_entries,
            },
            indent=2,
        )
        + "\n",
        encoding="utf-8",
    )
    print(f"Wrote {len(manifest_entries)} staff OBJ under {OUT_DIR}")


if __name__ == "__main__":
    main()