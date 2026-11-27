"""Textured hero walk strips — Kenney CC0 Run clip as walk cycle (see manifest)."""
from __future__ import annotations

import math
import os
from dataclasses import dataclass

import bpy
from mathutils import Quaternion

import kenney_founder_lib as kf

RUN_FBX = kf.INCOMING / "kenney/animated-characters-3/Animations/run.fbx"
KENNEY_RUN_LICENSE = "tools/art/cc0/manifest.json (Kenney Animated Characters 3)"
ARM_BLEND_BONES = ("LeftArm", "RightArm", "LeftForeArm", "RightForeArm")
IDLE_ARM_BLEND = 0.93


@dataclass
class WalkHeroScene:
    arm: bpy.types.Object
    body: bpy.types.Object
    render_meshes: list[bpy.types.Object]
    action_name: str
    walk_frames: list[int]
    labels: tuple[str, str, str, str]
    idle_arm_quats: dict[str, Quaternion]


def _purge_dup_armature() -> None:
    dup = bpy.data.objects.get("Root.001")
    if dup:
        bpy.data.objects.remove(dup, do_unlink=True)


def _pick_run_action() -> bpy.types.Action:
    for act in bpy.data.actions:
        if act.name.endswith("Run") or "|Run" in act.name:
            return act
    raise RuntimeError("walk_strip missing Kenney Run action")


def _foot_z(arm: bpy.types.Object, bone: str) -> float:
    pb = arm.pose.bones.get(bone)
    if not pb:
        return 1e9
    return (arm.matrix_world @ pb.head).z


def _stance_foot_min_z(arm: bpy.types.Object, side: str) -> float:
    zs = []
    for name in (f"{side}Foot", f"{side}Toes"):
        z = _foot_z(arm, name)
        if z < 1e8:
            zs.append(z)
    return min(zs) if zs else 1e9


def ground_walk_cycle(
    arm: bpy.types.Object,
    mesh: bpy.types.Object,
    scene: bpy.types.Scene,
    start: int,
    end: int,
    idle_arm_quats: dict[str, Quaternion],
) -> float:
    """Shift hero so lowest foot (idle arms blended) over the clip rests on z=0."""
    mn = 1e9
    for fr in range(start, end + 1):
        scene.frame_set(fr)
        apply_idle_arm_blend(arm, idle_arm_quats)
        for side in ("Left", "Right"):
            mn = min(mn, _stance_foot_min_z(arm, side))
    if mn > 1e8:
        return 0.0
    dz = -mn
    if abs(dz) < 1e-6:
        return 0.0
    arm.location.z += dz
    mesh.location.z += dz
    bpy.context.view_layer.update()
    return dz


def _sample_feet(
    arm: bpy.types.Object,
    scene: bpy.types.Scene,
    start: int,
    end: int,
    idle_arm_quats: dict[str, Quaternion] | None,
) -> list[tuple[int, float, float]]:
    rows: list[tuple[int, float, float]] = []
    for fr in range(start, end + 1):
        scene.frame_set(fr)
        if idle_arm_quats:
            apply_idle_arm_blend(arm, idle_arm_quats)
        else:
            bpy.context.view_layer.update()
        lz = _stance_foot_min_z(arm, "Left")
        rz = _stance_foot_min_z(arm, "Right")
        rows.append((fr, lz, rz))
    return rows


def _pick_passing(
  rows: list[tuple[int, float, float]],
  exclude: set[int],
  *,
  stance_max_m: float,
) -> int:
    best_fr = rows[0][0]
    best_score = -1.0
    for fr, lz, rz in rows:
        if fr in exclude:
            continue
        low = min(lz, rz)
        if low > stance_max_m:
            continue
        score = max(lz, rz) - low
        if score > best_score:
            best_score = score
            best_fr = fr
    if best_score < 0:
        quarter = max(1, (rows[-1][0] - rows[0][0]) // 4)
        return rows[min(len(rows) // 2, len(rows) - 1)][0] if quarter else rows[0][0]
    return best_fr


def pick_walk_contact_frames(
    arm: bpy.types.Object,
    scene: bpy.types.Scene,
    start: int,
    end: int,
    idle_arm_quats: dict[str, Quaternion] | None = None,
) -> list[int]:
    """Contact L → passing → contact R → passing (locomotion subset of Kenney Run)."""
    rows = _sample_feet(arm, scene, start, end, idle_arm_quats)
    contact_l = min(rows, key=lambda t: t[1])[0]
    contact_r = min(rows, key=lambda t: t[2])[0]
    if contact_l == contact_r:
        quarter = max(1, (end - start) // 4)
        return [start, start + quarter, start + 2 * quarter, start + 3 * quarter]
    lo, hi = sorted((contact_l, contact_r))
    between = {fr for fr, _, _ in rows if lo < fr < hi}
    other = {fr for fr, _, _ in rows if fr < lo or fr > hi}
    passing_1 = _pick_passing(
        [r for r in rows if r[0] in between],
        exclude=set(),
        stance_max_m=0.12,
    )
    if passing_1 in (contact_l, contact_r):
        passing_1 = _pick_passing(rows, exclude={contact_l, contact_r}, stance_max_m=0.12)
    passing_2 = _pick_passing(
        [r for r in rows if r[0] in other] or rows,
        exclude={contact_l, contact_r, passing_1},
        stance_max_m=0.12,
    )
    if passing_2 in (contact_l, contact_r, passing_1):
        passing_2 = _pick_passing(rows, exclude={contact_l, contact_r, passing_1}, stance_max_m=0.22)
    return [contact_l, passing_1, contact_r, passing_2]


def _capture_arm_idle(arm: bpy.types.Object) -> dict[str, Quaternion]:
    out: dict[str, Quaternion] = {}
    for name in ARM_BLEND_BONES:
        pb = arm.pose.bones.get(name)
        if pb:
            out[name] = pb.rotation_quaternion.copy()
    return out


def apply_idle_arm_blend(arm: bpy.types.Object, idle: dict[str, Quaternion], blend: float = IDLE_ARM_BLEND) -> None:
    bpy.context.view_layer.objects.active = arm
    for name, q_idle in idle.items():
        pb = arm.pose.bones.get(name)
        if not pb:
            continue
        pb.rotation_mode = "QUATERNION"
        pb.rotation_quaternion = q_idle.slerp(pb.rotation_quaternion, 1.0 - blend)
    bpy.context.view_layer.update()


def _assign_run_action(arm: bpy.types.Object) -> tuple[bpy.types.Action, int, int]:
    if not RUN_FBX.is_file():
        raise RuntimeError(f"missing Kenney run {RUN_FBX}")
    kf._import_fbx(str(RUN_FBX))
    _purge_dup_armature()
    act = _pick_run_action()
    arm.animation_data_create()
    arm.animation_data.action = act
    start, end = int(act.frame_range[0]), int(act.frame_range[1])
    return act, start, end


def build_founder_walk_scene() -> WalkHeroScene:
    from founder_face_decals import spawn_founder_eye_meshes
    from kenney_founder_stock_recolor import apply_founder_materials

    kf._clear_scene()
    arm, mesh = kf._import_rigged()
    kf._ground_rig(arm, mesh)
    kf._pose_kenney_idle_arms(arm)
    idle_arms = _capture_arm_idle(arm)
    bpy.context.view_layer.update()
    apply_founder_materials(mesh)
    mesh.name = "KenneyFounderStock"
    eyes = spawn_founder_eye_meshes(mesh)
    act, start, end = _assign_run_action(arm)
    scene = bpy.context.scene
    scene.frame_start = start
    scene.frame_end = end
    ground_walk_cycle(arm, mesh, scene, start, end, idle_arms)
    walk_frames = pick_walk_contact_frames(arm, scene, start, end, idle_arms)
    labels = ("contact_L", "passing", "contact_R", "passing")
    return WalkHeroScene(
        arm=arm,
        body=mesh,
        render_meshes=[mesh, *eyes],
        action_name=act.name,
        walk_frames=walk_frames,
        labels=labels,
        idle_arm_quats=idle_arms,
    )


def build_researcher_walk_scene() -> WalkHeroScene:
    from kenney_researcher_stock_recolor import (
        _pose_researcher_idle_arms,
        apply_researcher_materials,
        spawn_researcher_eye_meshes,
    )
    from researcher_hero_geometry import spawn_researcher_hair_cap, spawn_researcher_lab_coat

    kf._clear_scene()
    arm, mesh = kf._import_rigged()
    _pose_researcher_idle_arms(arm)
    kf._ground_rig(arm, mesh)
    idle_arms = _capture_arm_idle(arm)
    bpy.context.view_layer.update()
    apply_researcher_materials(mesh)
    mesh.name = "KenneyResearcherStock"
    eyes = spawn_researcher_eye_meshes(mesh)
    coat = spawn_researcher_lab_coat(mesh)
    hair = spawn_researcher_hair_cap(mesh)
    extras = [coat, hair]
    act, start, end = _assign_run_action(arm)
    scene = bpy.context.scene
    scene.frame_start = start
    scene.frame_end = end
    ground_walk_cycle(arm, mesh, scene, start, end, idle_arms)
    walk_frames = pick_walk_contact_frames(arm, scene, start, end, idle_arms)
    labels = ("contact_L", "passing", "contact_R", "passing")
    return WalkHeroScene(
        arm=arm,
        body=mesh,
        render_meshes=[mesh, *eyes, *extras],
        action_name=act.name,
        walk_frames=walk_frames,
        labels=labels,
        idle_arm_quats=idle_arms,
    )


def configure_preview_env(asset_id: str) -> None:
    os.environ["EARLY_ART_PREVIEW_RIG"] = "1"
    os.environ["STAFF_PREVIEW_ASSET"] = asset_id
    os.environ["STAFF_PREVIEW_YAW_DEG"] = "30"
    os.environ["STAFF_PREVIEW_ELEV_DEG"] = "10"
    os.environ["STAFF_PREVIEW_MARGIN"] = "1.44"
    if asset_id == "staff-founder-idle":
        os.environ.setdefault("STAFF_PREVIEW_FOUNDER_LIGHT", "0.30")
    if asset_id == "staff-researcher-idle":
        os.environ.setdefault("RESEARCHER_STOCK_RECOLOR", "1")


def founder_eye_preview_tweak(meshes: list[bpy.types.Object]) -> None:
    for ob in meshes:
        if ob.type != "MESH" or not ob.name.split(".")[0].startswith("FounderEye_"):
            continue
        mat = ob.active_material
        if not mat or not mat.use_nodes:
            continue
        bsdf = mat.node_tree.nodes.get("Principled BSDF")
        if bsdf:
            bsdf.inputs["Base Color"].default_value = (0.05, 0.05, 0.05, 1.0)
            bsdf.inputs["Roughness"].default_value = 1.0
            spec = bsdf.inputs.get("Specular") or bsdf.inputs.get("Specular IOR Level")
            if spec is not None:
                spec.default_value = 0.0