"""Procedural mesh helpers with UVs, normals, and per-part materials (CC0)."""
from __future__ import annotations

import math
from dataclasses import dataclass, field
from pathlib import Path


@dataclass
class MaterialDef:
    name: str
    kd: tuple[float, float, float]


@dataclass
class RichMesh:
    name: str = "mesh"
    vertices: list[tuple[float, float, float]] = field(default_factory=list)
    uvs: list[tuple[float, float]] = field(default_factory=list)
    normals: list[tuple[float, float, float]] = field(default_factory=list)
    faces: list[tuple[tuple[int, int, int], ...]] = field(default_factory=list)
    face_materials: list[str] = field(default_factory=list)
    material: str | None = None


def _add_quad(
    m: RichMesh,
    p0: tuple[float, float, float],
    p1: tuple[float, float, float],
    p2: tuple[float, float, float],
    p3: tuple[float, float, float],
    normal: tuple[float, float, float],
) -> None:
  base = len(m.vertices)
  m.vertices.extend([p0, p1, p2, p3])
  m.normals.extend([normal] * 4)
  m.uvs.extend([(0, 0), (1, 0), (1, 1), (0, 1)])
  m.faces.append(((base, 0, 0), (base + 1, 1, 1), (base + 2, 2, 2), (base + 3, 3, 3)))
  m.face_materials.append(m.material or "default")


def _subdiv_face(m: RichMesh, quads: int = 2) -> None:
    """No-op placeholder; subdiv done by builders."""


def bevel_box(
    w: float,
    h: float,
    d: float,
    ox: float = 0,
    oy: float = 0,
    oz: float = 0,
    bevel: float = 0.02,
    mat: str | None = None,
) -> RichMesh:
    """Axis-aligned box with slightly inset top edge (bevel read)."""
    m = RichMesh(material=mat)
    hw, hh, hd = w * 0.5 - bevel, h * 0.5, d * 0.5 - bevel
    y0, y1 = oy, oy + h
    # bottom
    _add_quad(
        m,
        (ox - hw, y0, oz - hd),
        (ox + hw, y0, oz - hd),
        (ox + hw, y0, oz + hd),
        (ox - hw, y0, oz + hd),
        (0, -1, 0),
    )
    # top (inset)
    ti = bevel * 0.8
    _add_quad(
        m,
        (ox - hw + ti, y1, oz - hd + ti),
        (ox - hw + ti, y1, oz + hd - ti),
        (ox + hw - ti, y1, oz + hd - ti),
        (ox + hw - ti, y1, oz - hd + ti),
        (0, 1, 0),
    )
    # sides
    _add_quad(
        m,
        (ox - hw, y0, oz + hd),
        (ox + hw, y0, oz + hd),
        (ox + hw, y1, oz + hd),
        (ox - hw, y1, oz + hd),
        (0, 0, 1),
    )
    _add_quad(
        m,
        (ox + hw, y0, oz + hd),
        (ox + hw, y0, oz - hd),
        (ox + hw, y1, oz - hd),
        (ox + hw, y1, oz + hd),
        (1, 0, 0),
    )
    _add_quad(
        m,
        (ox + hw, y0, oz - hd),
        (ox - hw, y0, oz - hd),
        (ox - hw, y1, oz - hd),
        (ox + hw, y1, oz - hd),
        (0, 0, -1),
    )
    _add_quad(
        m,
        (ox - hw, y0, oz - hd),
        (ox - hw, y0, oz + hd),
        (ox - hw, y1, oz + hd),
        (ox - hw, y1, oz - hd),
        (-1, 0, 0),
    )
    return m


def cylinder(
    radius: float,
    height: float,
    segments: int = 10,
    ox: float = 0,
    oy: float = 0,
    oz: float = 0,
    mat: str | None = None,
) -> RichMesh:
    m = RichMesh(material=mat)
    for i in range(segments):
        a0 = 2 * math.pi * i / segments
        a1 = 2 * math.pi * (i + 1) / segments
        x0, z0 = ox + math.cos(a0) * radius, oz + math.sin(a0) * radius
        x1, z1 = ox + math.cos(a1) * radius, oz + math.sin(a1) * radius
        nx0, nz0 = math.cos(a0), math.sin(a0)
        nx1, nz1 = math.cos(a1), math.sin(a1)
        base = len(m.vertices)
        m.vertices.extend([(x0, oy, z0), (x1, oy, z1), (x1, oy + height, z1), (x0, oy + height, z0)])
        m.normals.extend([(nx0, 0, nz0), (nx1, 0, nz1), (nx1, 0, nz1), (nx0, 0, nz0)])
        m.uvs.extend([(0, 0), (1, 0), (1, 1), (0, 1)])
        m.faces.append(((base, 0, 0), (base + 1, 1, 1), (base + 2, 2, 2), (base + 3, 3, 3)))
    # caps
    base = len(m.vertices)
    m.vertices.append((ox, oy, oz))
    m.vertices.append((ox, oy + height, oz))
    m.normals.extend([(0, -1, 0), (0, 1, 0)])
    m.uvs.extend([(0.5, 0.5), (0.5, 0.5)])
    cap_b = base
    cap_t = base + 1
    for i in range(segments):
        a0 = 2 * math.pi * i / segments
        a1 = 2 * math.pi * (i + 1) / segments
        vb = len(m.vertices)
        m.vertices.append((ox + math.cos(a0) * radius, oy, oz + math.sin(a0) * radius))
        m.normals.append((0, -1, 0))
        m.uvs.append((0, 0))
        vb2 = len(m.vertices)
        m.vertices.append((ox + math.cos(a1) * radius, oy, oz + math.sin(a1) * radius))
        m.normals.append((0, -1, 0))
        m.uvs.append((0, 0))
        m.faces.append(((cap_b, cap_b, cap_b), (vb, vb, vb), (vb2, vb2, vb2)))
        vt = len(m.vertices)
        m.vertices.append((ox + math.cos(a0) * radius, oy + height, oz + math.sin(a0) * radius))
        m.normals.append((0, 1, 0))
        m.uvs.append((0, 0))
        vt2 = len(m.vertices)
        m.vertices.append((ox + math.cos(a1) * radius, oy + height, oz + math.sin(a1) * radius))
        m.normals.append((0, 1, 0))
        m.uvs.append((0, 0))
        m.faces.append(((cap_t, cap_t, cap_t), (vt2, vt2, vt2), (vt, vt, vt)))
    return m


def append_mesh(dst: RichMesh, src: RichMesh) -> None:
    bv, bt, bn = len(dst.vertices), len(dst.uvs), len(dst.normals)
    dst.vertices.extend(src.vertices)
    dst.uvs.extend(src.uvs)
    dst.normals.extend(src.normals)
    for f in src.faces:
        dst.faces.append(tuple((v + bv, t + bt, n + bn) for v, t, n in f))
    dst.face_materials.extend([src.material or "default"] * len(src.faces))


def merge_rich(*parts: RichMesh) -> RichMesh:
    out = RichMesh()
    out.face_materials = []
    for p in parts:
        append_mesh(out, p)
    return out


def mesh_stats(m: RichMesh) -> tuple[int, int]:
    tris = sum(max(0, len(f) - 2) for f in m.faces)
    return len(m.vertices), tris


def write_obj_with_mtl(m: RichMesh, obj_path: Path, materials: dict[str, MaterialDef]) -> None:
    obj_path.parent.mkdir(parents=True, exist_ok=True)
    mtl_name = obj_path.stem + ".mtl"
    mtl_path = obj_path.parent / mtl_name
    mtl_lines = []
    for mat in materials.values():
        r, g, b = mat.kd
        mtl_lines.append(f"newmtl {mat.name}\nKd {r:.4f} {g:.4f} {b:.4f}\nKa 0.05 0.05 0.05\nKs 0.2 0.2 0.2\nNs 32\n")
    mtl_path.write_text("".join(mtl_lines), encoding="utf-8")

    lines = [
        f"# {m.name} — procedural CC0",
        f"mtllib {mtl_name}",
        f"o {m.name}",
    ]
    for x, y, z in m.vertices:
        lines.append(f"v {x:.6f} {y:.6f} {z:.6f}")
    for u, v in m.uvs:
        lines.append(f"vt {u:.6f} {v:.6f}")
    for nx, ny, nz in m.normals:
        lines.append(f"vn {nx:.6f} {ny:.6f} {nz:.6f}")

    materials_list = m.face_materials if m.face_materials else [m.material or "default"] * len(m.faces)
    cur_mat = None
    for face, mat in zip(m.faces, materials_list, strict=False):
        if mat != cur_mat:
            lines.append(f"usemtl {mat}")
            cur_mat = mat
        parts = [f"{vi + 1}/{ti + 1}/{ni + 1}" for vi, ti, ni in face]
        if len(parts) == 3:
            lines.append("f " + " ".join(parts))
        else:
            lines.append("f " + " ".join(parts))
    obj_path.write_text("\n".join(lines) + "\n", encoding="utf-8")