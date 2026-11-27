"""Numeric geometry self-check for researcher hero (fail build before preview render)."""
from __future__ import annotations

import math
import sys
from pathlib import Path

import bpy
from mathutils import Vector

sys.path.insert(0, str(Path(__file__).resolve().parent))
import kenney_researcher_lib as kr  # noqa: E402
from kenney_geometry_landmarks import compute_landmarks  # noqa: E402
from researcher_hero_geometry import (  # noqa: E402
    _fwd_right_flat,
    _head_width_at,
    _sleeve_axis_world,
    _vg_weight,
    _researcher_head_top,
)


def _world_verts(ob: bpy.types.Object) -> list[Vector]:
    return [ob.matrix_world @ v.co for v in ob.data.vertices]


def _coat_bounds(body: bpy.types.Object, coat: bpy.types.Object, lm) -> dict[str, float]:
    lo, span = lm.zmin, lm.span
    wv = _world_verts(coat)
    if not wv:
        raise RuntimeError("FAIL researcher_hero_selfcheck coat_verts=0")
    neck_z = lo + span * (lm.h_neck - 0.02)
    shoulder_z = lo + span * (lm.h_neck - 0.14)
    mid_thigh_z = lo + span * (lm.h_waist - 0.10)
    head_w = _head_width_at(_world_verts(body), lo, span, lm.h_eye)
    torso_half = head_w * 0.36
    above_neck = 0
    torso_zs: list[float] = []
    for wc in wv:
        if wc.z > neck_z + span * 0.03 and abs(wc.x) < torso_half:
            above_neck += 1
        if wc.z <= shoulder_z + span * 0.03:
            torso_zs.append(wc.z)
    zs = [p.z for p in wv]
    z_top = max(torso_zs) if torso_zs else max(zs)
    hem_zs = [
        wc.z
        for wc in wv
        if wc.z >= mid_thigh_z - span * 0.12 and abs(wc.x) < torso_half * 1.25
    ]
    z_bottom = min(hem_zs) if hem_zs else min(zs)
    return {
        "z_top": z_top,
        "z_bottom": z_bottom,
        "shoulder_z": shoulder_z,
        "mid_thigh_z": mid_thigh_z,
        "neck_z": neck_z,
        "above_neck_verts": float(above_neck),
        "above_neck_limit": span * 0.03,
    }


def _sleeve_metrics(body: bpy.types.Object, side: str, lm, hc: Vector, fwd: Vector, ws) -> dict[str, float]:
    sh, wr, r_top, r_bot = _sleeve_axis_world(body, side, lm, hc, fwd, ws)
    arm = wr - sh
    arm_len = arm.length
    bones = frozenset({f"{side}Shoulder", f"{side}Arm"})
    fore = frozenset({f"{side}ForeArm", f"{side}Hand"})
    lo, span = lm.zmin, lm.span
    from researcher_hero_geometry import _vg_weighted_centroid

    elbow = _vg_weighted_centroid(
        body,
        frozenset({f"{side}ForeArm"}),
        lo=lo,
        span=span,
        h_lo=lm.h_waist - 0.14,
        h_hi=lm.h_neck - 0.20,
        min_w=0.06,
    )
    angle_upper = angle_lower = 0.0
    if elbow is not None and arm_len > 1e-6:
        arm_n = arm / arm_len
        u = (elbow - sh)
        l = (wr - elbow)
        if u.length > 1e-6:
            angle_upper = math.degrees(math.acos(max(-1.0, min(1.0, u.normalized().dot(arm_n)))))
        if l.length > 1e-6:
            angle_lower = math.degrees(math.acos(max(-1.0, min(1.0, l.normalized().dot(arm_n)))))
    cuff = max(r_top, r_bot)
    return {
        "length": arm_len,
        "angle_upper_deg": angle_upper,
        "angle_lower_deg": angle_lower,
        "cuff_to_sleeve": r_bot / max(r_top, 1e-6),
        "cuff_max_ratio": 1.15,
        "r_top": r_top,
        "r_bot": r_bot,
    }


def _glasses_metrics(body: bpy.types.Object, glasses: bpy.types.Object) -> dict[str, float]:
    from kenney_face_axis import face_shell_coord, toe_forward

    ws = _world_verts(body)
    lm = compute_landmarks(body)
    head_w = _head_width_at(ws, lm.zmin, lm.span, lm.h_eye)
    eye_width = head_w * 0.036 * 2.0
    min_frame = eye_width * 1.4
    fwd = toe_forward([body]).normalized()
    _, right = _fwd_right_flat(fwd)
    gverts = _world_verts(glasses)
    lat = [v.dot(right) for v in gverts]
    lateral_span = (max(lat) - min(lat)) if lat else 0.0
    frame_outer = lateral_span * 0.26
    return {
        "eye_width": eye_width,
        "min_frame_width": min_frame,
        "frame_outer_lat": frame_outer,
        "temple_lateral_span": lateral_span,
        "temple_min": head_w * 0.72,
    }


def assert_researcher_hero_geometry(body: bpy.types.Object) -> None:
    lm = compute_landmarks(body)
    from kenney_face_axis import face_shell_coord, toe_forward

    fwd = toe_forward([body]).normalized()
    hc, _ = face_shell_coord(body, fwd)
    ws = _world_verts(body)
    coat = bpy.data.objects.get("ResearcherLabCoat")
    glasses = bpy.data.objects.get("ResearcherGlasses")
    if coat is None or glasses is None:
        raise RuntimeError("FAIL researcher_hero_selfcheck missing_coat_or_glasses")

    c = _coat_bounds(body, coat, lm)
    if c["above_neck_verts"] > 0:
        raise RuntimeError(
            f"FAIL researcher_hero_selfcheck coat_above_neck verts={c['above_neck_verts']:.0f} "
            f"neck_z={c['neck_z']:.4f}"
        )
    if c["z_top"] > c["shoulder_z"] + c["above_neck_limit"]:
        raise RuntimeError(
            f"FAIL researcher_hero_selfcheck coat_z_top={c['z_top']:.4f} "
            f"shoulder_max={c['shoulder_z'] + c['above_neck_limit']:.4f}"
        )
    if c["z_bottom"] > c["mid_thigh_z"] + lm.span * 0.06:
        raise RuntimeError(
            f"FAIL researcher_hero_selfcheck coat_z_bottom={c['z_bottom']:.4f} "
            f"mid_thigh={c['mid_thigh_z']:.4f}"
        )
    if c["z_bottom"] < c["mid_thigh_z"] - lm.span * 0.22:
        raise RuntimeError(
            f"FAIL researcher_hero_selfcheck coat_z_bottom={c['z_bottom']:.4f} "
            f"too_low mid_thigh={c['mid_thigh_z']:.4f}"
        )

    print(
        "OK researcher_hero_selfcheck coat "
        f"z_top={c['z_top']:.4f} z_bottom={c['z_bottom']:.4f} "
        f"shoulder_z={c['shoulder_z']:.4f} mid_thigh_z={c['mid_thigh_z']:.4f} "
        f"above_neck_verts={c['above_neck_verts']:.0f}"
    )

    for side in ("Left", "Right"):
        m = _sleeve_metrics(body, side, lm, hc, fwd, ws)
        ang = max(m["angle_upper_deg"], m["angle_lower_deg"])
        if ang > 10.0:
            raise RuntimeError(
                f"FAIL researcher_hero_selfcheck sleeve_{side} angle_deg={ang:.2f} want<=10"
            )
        if m["length"] < lm.span * 0.28:
            raise RuntimeError(
                f"FAIL researcher_hero_selfcheck sleeve_{side} length={m['length']:.4f} too_short"
            )
        if m["cuff_to_sleeve"] > m["cuff_max_ratio"]:
            raise RuntimeError(
                f"FAIL researcher_hero_selfcheck sleeve_{side} cuff_ratio={m['cuff_to_sleeve']:.3f} "
                f"want<={m['cuff_max_ratio']}"
            )
        print(
            f"OK researcher_hero_selfcheck sleeve_{side} "
            f"len={m['length']:.4f} angle_upper={m['angle_upper_deg']:.2f} "
            f"angle_lower={m['angle_lower_deg']:.2f} cuff_ratio={m['cuff_to_sleeve']:.3f}"
        )

    g = _glasses_metrics(body, glasses)
    if g["frame_outer_lat"] < g["min_frame_width"] * 0.92:
        raise RuntimeError(
            f"FAIL researcher_hero_selfcheck glasses frame_lat={g['frame_outer_lat']:.4f} "
            f"want>={g['min_frame_width']:.4f}"
        )
    if g["temple_lateral_span"] < g["temple_min"]:
        raise RuntimeError(
            f"FAIL researcher_hero_selfcheck glasses temple_span={g['temple_lateral_span']:.4f} "
            f"want>={g['temple_min']:.4f}"
        )
    print(
        "OK researcher_hero_selfcheck glasses "
        f"eye_w={g['eye_width']:.4f} frame_outer_lat={g['frame_outer_lat']:.4f} "
        f"min_frame={g['min_frame_width']:.4f} temple_span={g['temple_lateral_span']:.4f}"
    )


def main() -> None:
    import os

    os.environ["RESEARCHER_STOCK_RECOLOR"] = "1"
    kr.build_researcher_kenney()
    body = bpy.data.objects.get("KenneyResearcherStock")
    if body is None:
        raise RuntimeError("FAIL researcher_hero_selfcheck missing_body")
    assert_researcher_hero_geometry(body)
    raise SystemExit(0)


if __name__ == "__main__":
    main()