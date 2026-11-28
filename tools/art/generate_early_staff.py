#!/usr/bin/env python3
"""Distinct low-poly staff silhouettes (top-down readable, UVs + materials)."""
from __future__ import annotations

import json
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
from garage_kit_builders import MATS  # noqa: E402
from mesh_lib import MaterialDef, RichMesh, bevel_box, cylinder, merge_rich, mesh_stats, write_obj_with_mtl  # noqa: E402

REPO_ROOT = Path(__file__).resolve().parents[2]
OUT_DIR = REPO_ROOT / "unity" / "Assets" / "Art" / "EarlyGame" / "staff-kit"
MATERIAL_DEFS = {k: MaterialDef(k, v) for k, v in MATS.items()}


def _torso(mat_shirt: str, mat_pants: str, shoulder_w: float = 0.38) -> RichMesh:
    return merge_rich(
        bevel_box(shoulder_w, 0.52, 0.24, oy=0.92, mat=mat_shirt),
        bevel_box(0.34, 0.52, 0.22, oy=0.46, mat=mat_pants),
        cylinder(0.11, 0.52, segments=8, ox=-0.12, mat=mat_pants),
        cylinder(0.11, 0.52, segments=8, ox=0.12, mat=mat_pants),
    )


def staff_researcher_idle() -> RichMesh:
    head = merge_rich(
        bevel_box(0.26, 0.26, 0.26, oy=1.38, mat="plastic_keys"),
        bevel_box(0.08, 0.22, 0.12, ox=-0.18, oy=1.42, mat="wood_dark"),
    )
    body = _torso("fabric_back", "fabric_seat", 0.36)
    m = merge_rich(head, body, bevel_box(0.18, 0.24, 0.04, 0.22, 0.95, 0.1, mat="plastic_keys"))
    m.name = "staff-researcher-idle"
    return m


def staff_engineer_work() -> RichMesh:
    hat = bevel_box(0.36, 0.08, 0.36, oy=1.52, mat="trim_orange")
    head = bevel_box(0.26, 0.24, 0.26, oy=1.38, mat="plastic_keys")
    body = _torso("rack_paint", "metal_frame", 0.42)
    laptop = bevel_box(0.38, 0.04, 0.28, 0.0, 1.0, 0.18, mat="metal_scratched")
    screen = bevel_box(0.32, 0.22, 0.03, 0.0, 1.08, 0.22, mat="crt_glass")
    m = merge_rich(hat, head, body, laptop, screen)
    m.name = "staff-engineer-work"
    return m


def staff_safety_idle() -> RichMesh:
    vest = bevel_box(0.4, 0.45, 0.26, oy=0.98, mat="trim_orange")
    head = bevel_box(0.26, 0.26, 0.26, oy=1.38, mat="plastic_keys")
    body = _torso("fabric_seat", "fabric_back")
    clip = bevel_box(0.14, 0.2, 0.06, 0.24, 0.92, 0.12, mat="metal_scratched")
    m = merge_rich(vest, head, body, clip)
    m.name = "staff-safety-idle"
    return m


def staff_sales_idle() -> RichMesh:
    head = bevel_box(0.26, 0.26, 0.26, oy=1.38, mat="plastic_keys")
    body = _torso("fabric_back", "fabric_seat", 0.4)
    phone = merge_rich(
        bevel_box(0.06, 0.14, 0.05, 0.28, 1.02, 0.1, mat="plastic_char"),
        bevel_box(0.12, 0.22, 0.12, 0.28, 1.12, 0.12, mat="wood_oak"),
    )
    m = merge_rich(head, body, phone)
    m.name = "staff-sales-idle"
    return m


def staff_recruiter_idle() -> RichMesh:
    head = bevel_box(0.26, 0.26, 0.26, oy=1.38, mat="plastic_keys")
    body = _torso("wood_oak", "fabric_seat", 0.37)
    folder = bevel_box(0.22, 0.28, 0.06, -0.28, 0.96, 0.1, mat="sign_board")
    tie = bevel_box(0.06, 0.2, 0.03, 0.0, 1.05, 0.14, mat="trim_orange")
    m = merge_rich(head, body, folder, tie)
    m.name = "staff-recruiter-idle"
    return m


def staff_gpu_tech_idle() -> RichMesh:
    cap = cylinder(0.14, 0.06, segments=10, oy=1.48, mat="gpu_green")
    head = bevel_box(0.26, 0.26, 0.26, oy=1.38, mat="plastic_keys")
    body = _torso("rack_paint", "metal_frame")
    tool = cylinder(0.02, 0.2, segments=6, ox=0.3, oy=1.05, oz=0.05, mat="metal_scratched")
    m = merge_rich(cap, head, body, tool)
    m.name = "staff-gpu-tech-idle"
    return m


def staff_founder_idle() -> RichMesh:
    head = merge_rich(
        bevel_box(0.26, 0.26, 0.26, oy=1.38, mat="plastic_keys"),
        bevel_box(0.3, 0.06, 0.24, oy=1.52, mat="wood_dark"),
    )
    body = _torso("wood_oak", "fabric_seat")
    mug = cylinder(0.05, 0.12, segments=8, ox=0.26, oy=0.98, mat="trim_orange")
    m = merge_rich(head, body, mug)
    m.name = "staff-founder-idle"
    return m


def staff_pose_typing() -> RichMesh:
    return staff_engineer_work()


MESH_BUILDERS = {
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
        out = OUT_DIR / f"{asset_id}.obj"
        write_obj_with_mtl(mesh, out, MATERIAL_DEFS)
        verts, tris = mesh_stats(mesh)
        manifest_entries.append(
            {
                "id": asset_id,
                "path": f"Assets/Art/EarlyGame/staff-kit/{asset_id}.obj",
                "license": "CC0-1.0 (procedural, in-repo)",
                "vertexCount": verts,
                "triangleCount": tris,
            }
        )
    manifest_path = REPO_ROOT / "unity" / "Assets" / "Art" / "EarlyGame" / "staff-manifest.json"
    manifest_path.write_text(
        json.dumps({"generator": "tools/art/generate_early_staff.py", "fidelity": "packet-2", "assets": manifest_entries}, indent=2)
        + "\n",
        encoding="utf-8",
    )
    print(f"Wrote {len(manifest_entries)} staff OBJ under {OUT_DIR}")


if __name__ == "__main__":
    main()