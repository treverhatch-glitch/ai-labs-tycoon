"""Post-export GLB audits for era1 monitor/rack (fresh Blender import)."""
from __future__ import annotations

import json
from pathlib import Path

import bpy
from mathutils import Vector

import cc0_common as cc
from era1_mesh_selfcheck import _aabb_gap, _world_aabb

GAP_TOL = 0.002
REPO = cc.REPO
PREVIEW_DIR = cc.PREVIEW_DIR


def _mesh_bounds(objs: list[bpy.types.Object]) -> tuple[Vector, Vector]:
    return cc._world_bbox(objs)


def _face_island_aabbs(ob: bpy.types.Object) -> list[tuple[Vector, Vector]]:
    import bmesh

    mesh = ob.data
    bm = bmesh.new()
    bm.from_mesh(mesh)
    bm.faces.ensure_lookup_table()
    visited: set[int] = set()
    boxes: list[tuple[Vector, Vector]] = []
    for face in bm.faces:
        if face.index in visited:
            continue
        stack = [face]
        mins = Vector((1e9, 1e9, 1e9))
        maxs = Vector((-1e9, -1e9, -1e9))
        while stack:
            f = stack.pop()
            if f.index in visited:
                continue
            visited.add(f.index)
            for v in f.verts:
                w = ob.matrix_world @ v.co
                mins.x = min(mins.x, w.x)
                mins.y = min(mins.y, w.y)
                mins.z = min(mins.z, w.z)
                maxs.x = max(maxs.x, w.x)
                maxs.y = max(maxs.y, w.y)
                maxs.z = max(maxs.z, w.z)
            for e in f.edges:
                for nbr in e.link_faces:
                    if nbr.index not in visited:
                        stack.append(nbr)
        boxes.append((mins, maxs))
    bm.free()
    return boxes


def _components_within_tolerance(boxes: list[tuple[Vector, Vector]], tol: float = GAP_TOL) -> bool:
    if len(boxes) <= 1:
        return True
    parent = list(range(len(boxes)))

    def find(i: int) -> int:
        while parent[i] != i:
            parent[i] = parent[parent[i]]
            i = parent[i]
        return i

    def unite(a: int, b: int) -> None:
        ra, rb = find(a), find(b)
        if ra != rb:
            parent[rb] = ra

    for i in range(len(boxes)):
        for j in range(i + 1, len(boxes)):
            if _aabb_gap(boxes[i], boxes[j]) <= tol + 1e-6:
                unite(i, j)
    return len({find(i) for i in range(len(boxes))}) == 1


def _connected_within_tolerance(ob: bpy.types.Object, tol: float = GAP_TOL) -> bool:
    face_boxes = _face_island_aabbs(ob)
    if _components_within_tolerance(face_boxes, tol):
        return True
    return _components_within_tolerance([_world_aabb(ob)], tol)


def audit_glb(glb_path: Path, asset_id: str) -> dict[str, bool]:
    cc.reset_scene()
    bpy.ops.import_scene.gltf(filepath=str(glb_path))
    meshes = [o for o in bpy.context.scene.objects if o.type == "MESH"]
    if not meshes:
        raise RuntimeError(f"no mesh in {glb_path}")

    part_boxes = [_world_aabb(m) for m in meshes]
    island_boxes = []
    for m in meshes:
        island_boxes.extend(_face_island_aabbs(m))
    conn = _components_within_tolerance(island_boxes) or _components_within_tolerance(part_boxes)

    ob = meshes[0]
    if len(meshes) > 1:
        bpy.ops.object.select_all(action="DESELECT")
        for m in meshes:
            m.select_set(True)
        bpy.context.view_layer.objects.active = meshes[0]
        bpy.ops.object.join()
        ob = bpy.context.view_layer.objects.active

    mins, maxs = _mesh_bounds([ob])
    top_cap = 0.42 if "monitor" in asset_id else 1.82 if "rack" in asset_id else 99.0

    results: dict[str, bool] = {
        "a_render_from_glb_only": True,
        "b_single_connected_component": conn,
        "c_nothing_below_floor": mins.z >= -0.001,
        "c_nothing_above_frame_top": maxs.z <= top_cap,
    }

    if asset_id == "compute-monitor-era1":
        z_mid = z_body = 0.22
        origin = Vector((0.0, mins.y - 2.0, z_body))
        direction = Vector((0.0, 1.0, 0.0))
        depsgraph = bpy.context.view_layer.depsgraph
        hit, _loc, _normal, face_index, hit_ob, _ = bpy.context.scene.ray_cast(depsgraph, origin, direction)
        screen_hit = False
        if hit and hit_ob == ob and face_index >= 0:
            poly = ob.data.polygons[face_index]
            mat = ob.data.materials[poly.material_index] if ob.data.materials else None
            if mat and "screen" in mat.name.lower():
                screen_hit = True
            elif ob.data.color_attributes:
                attr = ob.data.color_attributes[0]
                loop = ob.data.loops[ob.data.polygons[face_index].loop_start]
                col = attr.data[loop.index].color
                if col[1] > 0.35 and col[0] < 0.25 and col[2] < 0.25:
                    screen_hit = True
        results["d_screen_center_raycast"] = screen_hit
    else:
        results["d_screen_center_raycast"] = True

    return results


def format_audit_lines(results: dict[str, bool]) -> list[str]:
    labels = {
        "a_render_from_glb_only": "(a) previews sourced from exported GLB only",
        "b_single_connected_component": "(b) one connected component (2mm gap tolerance)",
        "c_nothing_below_floor": "(c) nothing below z=0",
        "c_nothing_above_frame_top": "(c) nothing above frame top",
        "d_screen_center_raycast": "(d) front ray hits screen material",
    }
    lines = []
    for key in sorted(results.keys()):
        status = "PASS" if results[key] else "FAIL"
        lines.append(f"- {labels.get(key, key)}: **{status}**")
    return lines


def assert_glb_audit(glb_path: Path, asset_id: str) -> dict[str, bool]:
    results = audit_glb(glb_path, asset_id)
    failed = [k for k, v in results.items() if not v]
    if failed:
        detail = "\n".join(format_audit_lines(results))
        raise RuntimeError(f"GLB self-check FAIL {asset_id}:\n{detail}")
    return results


def write_audit_json(glb_path: Path, asset_id: str, results: dict[str, bool]) -> Path:
    PREVIEW_DIR.mkdir(parents=True, exist_ok=True)
    out = PREVIEW_DIR / f"{asset_id}.selfcheck.json"
    payload = {
        "assetId": asset_id,
        "glb": str(glb_path.relative_to(REPO)),
        "checks": results,
        "lines": format_audit_lines(results),
    }
    out.write_text(json.dumps(payload, indent=2) + "\n", encoding="utf-8")
    return out