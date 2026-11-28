"""Height landmarks from stock mesh XY cross-sections (neck / waist)."""
from __future__ import annotations

from dataclasses import dataclass

from mathutils import Vector

import bpy


@dataclass(frozen=True)
class FounderLandmarks:
    zmin: float
    zmax: float
    span: float
    cx: float
    cy: float
    h_shoe: float
    h_waist: float
    h_neck: float
    h_eye: float


def _world_verts(ob: bpy.types.Object) -> list[Vector]:
    return [ob.matrix_world @ v.co for v in ob.data.vertices]


def _slice_points(ws: list[Vector], lo: float, span: float, h: float, band: float) -> list[Vector]:
    return [p for p in ws if abs((p.z - lo) / span - h) < band]


def _xy_extent(pts: list[Vector]) -> float:
    if len(pts) < 4:
        return 1e9
    xs = [p.x for p in pts]
    ys = [p.y for p in pts]
    return max(max(xs) - min(xs), max(ys) - min(ys))


def _narrowest_h(ws: list[Vector], lo: float, span: float, h_lo: float, h_hi: float, step: float = 0.005) -> float:
    best_h = (h_lo + h_hi) * 0.5
    best = 1e9
    h = h_lo
    while h <= h_hi + 1e-6:
        ext = _xy_extent(_slice_points(ws, lo, span, h, 0.012))
        if ext < best:
            best = ext
            best_h = h
        h += step
    return best_h


def torso_half_width(ws: list[Vector], lo: float, span: float, h: float, cx: float, cy: float) -> float:
    pts = _slice_points(ws, lo, span, h, 0.014)
    if len(pts) < 6:
        return span * 0.09
    return max((Vector((p.x - cx, p.y - cy, 0.0))).length for p in pts)


def compute_landmarks(ob: bpy.types.Object) -> FounderLandmarks:
    ws = _world_verts(ob)
    zs = [p.z for p in ws]
    zmin, zmax = min(zs), max(zs)
    span = max(1e-4, zmax - zmin)
    cx = sum(p.x for p in ws) / len(ws)
    cy = sum(p.y for p in ws) / len(ws)
    h_waist = _narrowest_h(ws, zmin, span, 0.40, 0.50)
    h_neck = _narrowest_h(ws, zmin, span, 0.55, 0.80)
    head_pts = [p for p in ws if (p.z - zmin) / span > h_neck + 0.02]
    head_top = max((p.z - zmin) / span for p in head_pts) if head_pts else 0.97
    h_eye = h_neck + 0.55 * (head_top - h_neck)
    return FounderLandmarks(
        zmin=zmin,
        zmax=zmax,
        span=span,
        cx=cx,
        cy=cy,
        h_shoe=0.07,
        h_waist=h_waist,
        h_neck=h_neck,
        h_eye=h_eye,
    )