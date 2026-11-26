"""Place wisp facial features on the front hemisphere of the body sphere."""
from __future__ import annotations

import math
from typing import TYPE_CHECKING

if TYPE_CHECKING:
    import bpy

SKIN_OFFSET_M = 0.012


def body_radius(body: "bpy.types.Object") -> float:
    xs = [body.matrix_world @ v.co for v in body.data.vertices]
    if not xs:
        return 0.42
    cx = sum(p.x for p in xs) / len(xs)
    cy = sum(p.y for p in xs) / len(xs)
    zs = [p.z for p in xs]
    cz = sum(zs) / len(zs) if zs else 0.0
    return max((p - __import__("mathutils").Vector((cx, cy, cz))).length for p in xs)


def sphere_point(
    cx: float, cy: float, cz: float, radius: float, colatitude_rad: float, azimuth_rad: float
) -> tuple[float, float, float]:
    """Y-up wisp: colatitude θ from +Y pole; azimuth φ in XZ from +X toward camera (−X face = π)."""
    r = radius + SKIN_OFFSET_M
    y = cy + r * math.cos(colatitude_rad)
    ring = r * math.sin(colatitude_rad)
    x = cx + ring * math.cos(azimuth_rad)
    z = cz + ring * math.sin(azimuth_rad)
    return x, y, z


def face_dims(radius: float) -> tuple[float, float, float]:
    """Eye row ~40% of sphere diameter; mouth ~40% width; thin along normal."""
    diam = 2.0 * radius
    eye = 0.26 * diam
    mouth_w = 0.44 * diam
    thick = 0.010
    return eye, mouth_w, thick