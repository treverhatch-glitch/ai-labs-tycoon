#!/usr/bin/env python3
"""CC0 Kenney character → researcher hero (recolour lab coat, shirt, glasses)."""
from __future__ import annotations

import os
import sys
from pathlib import Path

import bmesh
import bpy
from mathutils import Vector

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_common as bc  # noqa: E402
import kenney_researcher_lib as kr  # noqa: E402
from kenney_face_axis import face_shell_coord, lateral_from_front, toe_forward
from kenney_geometry_landmarks import compute_landmarks, torso_half_width
from kenney_researcher_from_founder import (
    STOCK_OBJECT_NAME,
    collect_researcher_founder_meshes,
)
from kenney_researcher_stock_recolor import _vg_mean_on_face

TROUSER_GREY_NAME = "trouser_grey"
TROUSER_GREY_RGBA = (100 / 255, 100 / 255, 102 / 255, 1.0)

LEG_VG = frozenset(
    {"LeftUpLeg", "RightUpLeg", "LeftLeg", "RightLeg", "LeftFoot", "RightFoot"}
)
HAND_VG = frozenset(
    {
        "LeftHand",
        "RightHand",
        "LeftHandIndex1",
        "LeftHandIndex2",
        "LeftHandIndex3",
        "LeftHandThumb1",
        "LeftHandThumb2",
        "RightHandIndex1",
        "RightHandIndex2",
        "RightHandIndex3",
        "RightHandThumb1",
        "RightHandThumb2",
    }
)


def _ensure_trouser_grey_material(roughness: float = 0.58) -> bpy.types.Material:
    mat = bpy.data.materials.get(TROUSER_GREY_NAME)
    if mat is None:
        mat = bpy.data.materials.new(TROUSER_GREY_NAME)
    mat.use_nodes = True
    nt = mat.node_tree
    for n in list(nt.nodes):
        nt.nodes.remove(n)
    out = nt.nodes.new("ShaderNodeOutputMaterial")
    bsdf = nt.nodes.new("ShaderNodeBsdfPrincipled")
    bsdf.inputs["Base Color"].default_value = TROUSER_GREY_RGBA
    bsdf.inputs["Roughness"].default_value = roughness
    spec = bsdf.inputs.get("Specular") or bsdf.inputs.get("Specular IOR Level")
    if spec is not None:
        spec.default_value = 0.35
    nt.links.new(bsdf.outputs["BSDF"], out.inputs["Surface"])
    return mat


def _coat_hem_normalized(
    body: bpy.types.Object, meshes: list[bpy.types.Object], lm
) -> float:
    skirt = next(
        (o for o in meshes if o.name.split(".")[0] == "ResearcherCoatSkirt"),
        None,
    )
    if skirt is not None:
        zs = [(skirt.matrix_world @ v.co).z for v in skirt.data.vertices]
        if zs:
            hem_z = min(zs)
            return (hem_z - lm.zmin) / max(1e-6, lm.span)
    hem = lm.h_waist + 0.50
    if hem < lm.h_shoe + 0.12:
        hem = max(lm.h_waist - 0.15, lm.h_shoe + 0.12)
    return hem


def apply_trouser_grey_leg_faces() -> int:
    """Leg faces below coat hem → trouser_grey; hands excluded."""
    meshes = collect_researcher_founder_meshes()
    body = next(
        o for o in meshes if o.type == "MESH" and o.name.split(".")[0] == STOCK_OBJECT_NAME
    )
    trouser_mat = _ensure_trouser_grey_material()
    lm = compute_landmarks(body)
    lo, span = lm.zmin, lm.span
    ws = [body.matrix_world @ v.co for v in body.data.vertices]
    fwd = toe_forward([body])
    hc, _y_face_fn = face_shell_coord(body, fwd)
    hem_h = _coat_hem_normalized(body, meshes, lm)
    h_lo = lm.h_shoe - 0.058
    slot_index = len(body.data.materials)
    body.data.materials.append(trouser_mat)

    bm = bmesh.new()
    bm.from_mesh(body.data)
    bm.faces.ensure_lookup_table()
    changed = 0
    has_vg = bool(body.vertex_groups)
    for face in bm.faces:
        wc = Vector()
        for v in face.verts:
            wc += body.matrix_world @ v.co
        wc /= len(face.verts)
        h = (wc.z - lo) / span
        if h >= hem_h or h < h_lo:
            continue
        lat = lateral_from_front(wc, hc, fwd)
        thw = torso_half_width(ws, lo, span, h, lm.cx, lm.cy)
        if has_vg:
            leg_w = _vg_mean_on_face(body, face, LEG_VG)
            hand_w = _vg_mean_on_face(body, face, HAND_VG)
            if hand_w > 0.55:
                continue
            in_leg_column = lat > thw * 0.48 or leg_w >= 0.05
            if lat <= thw * 0.85 and leg_w < 0.08:
                continue
        else:
            if lat <= thw * 0.52:
                continue
            in_leg_column = lat > thw * 0.48
        if not in_leg_column:
            continue
        face.material_index = slot_index
        changed += 1
    bm.to_mesh(body.data)
    bm.free()
    body.data.update()
    return changed


def main():
    os.environ["RESEARCHER_STOCK_RECOLOR"] = "1"
    kr.build_researcher_kenney()
    n_faces = apply_trouser_grey_leg_faces()
    print(f"OK staff-researcher-idle trouser_grey faces={n_faces}")
    entry = bc.finish_asset(
        "staff-researcher-idle",
        "staff-kit",
        license_note="CC0-1.0 (Kenney Animated Characters 3 + in-repo lab-coat recolour)",
    )
    print("OK", entry["id"], entry["triangleCount"], "tris")
    return entry


if __name__ == "__main__":
    bc.run_script(main)