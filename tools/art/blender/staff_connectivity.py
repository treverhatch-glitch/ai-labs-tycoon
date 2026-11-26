"""Staff part connectivity (pre-join names + imported GLB islands)."""
from __future__ import annotations

import re
from typing import Iterable

import bpy
from mathutils import Vector

MAX_GAP_M = 0.02
PROP_RE = re.compile(
    r"(Mug|Laptop|Keyboard|Mouse|Monitor|Briefcase|Clipboard|Phone|Card|Wrench|Desk|Chair|Gpu|Clip)",
    re.I,
)
SIGNIFICANT_ISLAND_VERTS = 14


def _world_bbox(ob: bpy.types.Object) -> tuple[Vector, Vector]:
    corners = [ob.matrix_world @ Vector(c) for c in ob.bound_box]
    xs = [c.x for c in corners]
    ys = [c.y for c in corners]
    zs = [c.z for c in corners]
    return Vector((min(xs), min(ys), min(zs))), Vector((max(xs), max(ys), max(zs)))


def bbox_gap(a: tuple[Vector, Vector], b: tuple[Vector, Vector]) -> float:
    lo_a, hi_a = a
    lo_b, hi_b = b
    dx = max(0.0, max(lo_a.x - hi_b.x, lo_b.x - hi_a.x))
    dy = max(0.0, max(lo_a.y - hi_b.y, lo_b.y - hi_a.y))
    dz = max(0.0, max(lo_a.z - hi_b.z, lo_b.z - hi_a.z))
    return (dx * dx + dy * dy + dz * dz) ** 0.5


def _by_name(mesh_obs: Iterable[bpy.types.Object]) -> dict[str, bpy.types.Object]:
    return {o.name.split(".")[0]: o for o in mesh_obs}


def check_prejoin_connectivity(mesh_obs: list[bpy.types.Object], asset_id: str) -> tuple[bool, str]:
    names = _by_name(mesh_obs)

    def need(*keys: str) -> dict[str, bpy.types.Object]:
        missing = [k for k in keys if k not in names]
        if missing:
            raise KeyError(f"{asset_id} missing parts {missing}")
        return {k: names[k] for k in keys}

    try:
        core = need("Head", "Neck", "Torso", "Hips", "Leg_-1", "Leg_1")
    except KeyError as exc:
        return False, str(exc)

    rules: list[tuple[str, str]] = [
        ("Head", "Neck"),
        ("Neck", "Torso"),
        ("Torso", "Hips"),
        ("Hips", "Leg_-1"),
        ("Hips", "Leg_1"),
    ]
    for sx in ("-1", "1"):
        ak, hk = f"Arm_{sx}", f"Hand_{sx}"
        sk = f"Shoulder_{sx}"
        if ak in names and hk in names:
            rules.append((hk, ak))
        if sk in names and "Torso" in names:
            rules.append((sk, "Torso"))
        fk = f"Foot_{sx}"
        lk = f"Leg_{sx}"
        if fk in names and lk in names:
            rules.append((fk, lk))
        for ek in (f"Eye_{sx}", f"Pupil_{sx}"):
            if ek in names and "Head" in names:
                rules.append((ek, "Head"))

    if "Mouth" in names:
        rules.append(("Mouth", "Head"))

    for parent_key, child_key in rules:
        pa = _world_bbox(core.get(parent_key) or names[parent_key])
        pb = _world_bbox(core.get(child_key) or names[child_key])
        gap = bbox_gap(pa, pb)
        if gap > MAX_GAP_M:
            return False, f"{parent_key}->{child_key} gap={gap:.4f}m"

    for ob in mesh_obs:
        base = ob.name.split(".")[0]
        if not PROP_RE.search(base):
            continue
        if base.endswith(("Screen", "ScreenEmit", "Led", "Handle", "ClipMetal", "Stripe")):
            continue
        bb = _world_bbox(ob)
        if base in ("ChairBack",):
            torso_gap = bbox_gap(bb, _world_bbox(names["Torso"])) if "Torso" in names else 999.0
            if torso_gap <= MAX_GAP_M:
                continue
        if base in ("DeskEdge", "Monitor", "Keyboard", "Mouse"):
            desk_props = [p for p in names if p in ("DeskEdge", "Keyboard") or p.startswith("Monitor")]
            desk_d = min(bbox_gap(bb, _world_bbox(names[p])) for p in desk_props if p != base) if desk_props else 999.0
            foot_lo = names["Foot_-1"].matrix_world.translation.y if "Foot_-1" in names else 0.02
            if desk_d <= MAX_GAP_M or bb[0].y <= foot_lo + 0.12:
                continue
        hand_d = min(
            bbox_gap(bb, _world_bbox(names[h])) for h in names if h.startswith("Hand_")
        ) if any(n.startswith("Hand_") for n in names) else 999.0
        foot_lo = names["Foot_-1"].matrix_world.translation.y if "Foot_-1" in names else 0.02
        on_ground = bb[0].y <= foot_lo + 0.12
        prop_neighbors = [
            bbox_gap(bb, _world_bbox(names[p]))
            for p in names
            if p != base
            and PROP_RE.search(p)
            and not p.endswith(("Screen", "ScreenEmit", "Led", "Handle", "ClipMetal"))
        ]
        prop_d = min(prop_neighbors) if prop_neighbors else 999.0
        if hand_d > MAX_GAP_M and not on_ground and prop_d > MAX_GAP_M:
            return False, f"prop {base} gap_to_hand={hand_d:.4f}m"

    for tag in names:
        if tag in ("Head", "Neck", "Torso", "Hips", "SpineCore", "Mouth", "Tie", "BadgeLanyard"):
            continue
        if tag.startswith(("Leg_", "Arm_", "Hand_", "Foot_", "Eye_", "Pupil_", "Shoulder_")):
            continue
        if PROP_RE.search(tag):
            continue
        if tag.startswith("Headset") or tag in (
            "Hair",
            "Hood",
            "HardHat",
            "HardHatEng",
            "Glasses",
            "HiVisVest",
            "HiVisStripe",
            "Blazer",
            "BlazerRec",
            "LabCoat",
        ):
            anchor = "Head" if tag.startswith(("Headset", "Hair", "Hood", "HardHat", "Glasses")) else "Torso"
            if anchor in names:
                gap = bbox_gap(_world_bbox(names[anchor]), _world_bbox(names[tag]))
                if gap > MAX_GAP_M:
                    return False, f"{anchor}->{tag} gap={gap:.4f}m"

    return True, "prejoin_ok"


def _mesh_islands(ob: bpy.types.Object) -> list[list[int]]:
    import bmesh

    bm = bmesh.new()
    bm.from_mesh(ob.data)
    bm.verts.ensure_lookup_table()
    visited: set[int] = set()
    islands: list[list[int]] = []
    for v in bm.verts:
        if v.index in visited:
            continue
        stack = [v]
        component: list[int] = []
        while stack:
            cur = stack.pop()
            if cur.index in visited:
                continue
            visited.add(cur.index)
            component.append(cur.index)
            for e in cur.link_edges:
                ov = e.other_vert(cur)
                if ov.index not in visited:
                    stack.append(ov)
        islands.append(component)
    bm.free()
    return islands


def _island_bbox(ob: bpy.types.Object, vert_ids: list[int]) -> tuple[Vector, Vector]:
    coords = [ob.matrix_world @ ob.data.vertices[i].co for i in vert_ids]
    xs = [c.x for c in coords]
    ys = [c.y for c in coords]
    zs = [c.z for c in coords]
    return Vector((min(xs), min(ys), min(zs))), Vector((max(xs), max(ys), max(zs)))


def check_gltf_connectivity(glb_path: str, asset_id: str) -> tuple[bool, str]:
    bpy.ops.wm.read_factory_settings(use_empty=True)
    bpy.ops.import_scene.gltf(filepath=glb_path)
    meshes = [o for o in bpy.context.scene.objects if o.type == "MESH"]
    if len(meshes) != 1:
        return False, f"mesh_objects={len(meshes)}"
    ob = meshes[0]
    islands = _mesh_islands(ob)
    if len(islands) < 2:
        return True, f"single_island verts={len(ob.data.vertices)}"

    bboxes = [_island_bbox(ob, ids) for ids in islands]
    sizes = [len(ids) for ids in islands]
    main_i = max(range(len(sizes)), key=lambda i: sizes[i])

    anchor = {main_i}
    for _ in range(len(islands)):
        added = False
        for i, bb in enumerate(bboxes):
            if i in anchor:
                continue
            if sizes[i] < SIGNIFICANT_ISLAND_VERTS:
                anchor.add(i)
                added = True
                continue
            near = min(bbox_gap(bb, bboxes[j]) for j in anchor)
            if near <= MAX_GAP_M:
                anchor.add(i)
                added = True
        if not added:
            break

    worst = 0.0
    worst_i = -1
    for i, bb in enumerate(bboxes):
        if sizes[i] < SIGNIFICANT_ISLAND_VERTS:
            continue
        if i in anchor:
            continue
        near = min(bbox_gap(bb, bboxes[j]) for j in anchor)
        if near > worst:
            worst = near
            worst_i = i
    if worst > MAX_GAP_M:
        return False, f"island={worst_i} float_gap={worst:.4f}m islands={len(islands)}"
    return True, f"islands={len(islands)} anchor={len(anchor)} worst_float={worst:.4f}"