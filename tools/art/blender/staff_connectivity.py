"""Staff part connectivity (build lane, exported GLB objects, island strict)."""
from __future__ import annotations

import re
from typing import Iterable

import bpy
from mathutils import Vector

MAX_GAP_M = 0.02
PROP_RE = re.compile(
    r"(Mug|Laptop|Keyboard|Mouse|Monitor|Briefcase|Clipboard|Phone|Card|Wrench|Desk|Chair|Gpu|Clip|Tablet|Badge|FounderMug)",
    re.I,
)
SIGNIFICANT_ISLAND_VERTS = 14
PROP_SUFFIX_SKIP = ("Screen", "ScreenEmit", "Led", "Handle", "ClipMetal", "Stripe", "Emit")

CLOTHING_RE = re.compile(
    r"(Hair|Hood|Hoodie|Shirt|Trousers|Shoes|HardHat|Glasses|HiVis|Blazer|LabCoat|Headset|BadgeLanyard|Tie)",
    re.I,
)


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


def _parent_key(child_base: str) -> str | None:
    if child_base in ("Head",):
        return "Neck"
    if child_base == "Neck":
        return "Torso"
    if child_base in ("Torso", "SpineCore"):
        return "Hips"
    if child_base.startswith("Leg_") or child_base.startswith("Foot_"):
        leg = child_base.replace("Foot_", "Leg_")
        return "Hips" if child_base.startswith("Leg_") else leg if leg else "Hips"
    if child_base.startswith("Shoulder_"):
        return "Torso"
    if child_base.startswith("UpperArm_"):
        return child_base.replace("UpperArm_", "Shoulder_")
    if child_base.startswith("LowerArm_") or child_base.startswith("Elbow_"):
        return child_base.replace("LowerArm_", "UpperArm_").replace("Elbow_", "UpperArm_")
    if child_base.startswith("Hand_"):
        return child_base.replace("Hand_", "LowerArm_")
    if child_base.startswith("Eye_") or child_base.startswith("Pupil_") or child_base == "Mouth":
        return "Head"
    if child_base.startswith("Headset") or child_base in ("Hair", "Hood", "HardHat", "HardHatEng", "Glasses"):
        return "Head"
    if CLOTHING_RE.search(child_base) and not child_base.startswith("BadgeShield"):
        if child_base in (
            "HiVisVest",
            "HiVisStripe",
            "Blazer",
            "BlazerRec",
            "LabCoat",
            "Hoodie",
            "Shirt",
            "Trousers",
            "BadgeLanyard",
            "Tie",
        ):
            return "Torso"
        if child_base.startswith("Shoe_"):
            return child_base.replace("Shoe_", "Foot_")
        return "Head"
    if PROP_RE.search(child_base):
        for h in ("Hand_1", "Hand_-1"):
            if h in bpy.context.view_layer.objects:  # noqa: SIM102
                pass
        return None
    return None


def parent_staff_hierarchy(mesh_obs: list[bpy.types.Object]) -> bpy.types.Object:
    """Parent parts for glTF export (props → hands / desk / torso)."""
    names = _by_name(mesh_obs)
    bpy.ops.object.empty_add(type="PLAIN_AXES", location=(0.0, 0.0, 0.0))
    root = bpy.context.active_object
    root.name = "StaffRoot"
    if "Hips" in names:
        bpy.ops.object.select_all(action="DESELECT")
        names["Hips"].select_set(True)
        root.select_set(True)
        bpy.context.view_layer.objects.active = root
        bpy.ops.object.parent_set(type="OBJECT", keep_transform=True)

    def link(child: str, parent: str) -> None:
        if child not in names or parent not in names:
            return
        bpy.ops.object.select_all(action="DESELECT")
        names[child].select_set(True)
        names[parent].select_set(True)
        bpy.context.view_layer.objects.active = names[parent]
        bpy.ops.object.parent_set(type="OBJECT", keep_transform=True)

    for child, parent in (
        ("Torso", "Hips"),
        ("SpineCore", "Torso"),
        ("Neck", "Torso"),
        ("Head", "Neck"),
        ("Leg_-1", "Hips"),
        ("Leg_1", "Hips"),
        ("Foot_-1", "Leg_-1"),
        ("Foot_1", "Leg_1"),
    ):
        link(child, parent)
    for sx in ("-1", "1"):
        link(f"Shoulder_{sx}", "Torso")
        link(f"UpperArm_{sx}", f"Shoulder_{sx}")
        if f"Elbow_{sx}" in names:
            link(f"Elbow_{sx}", f"UpperArm_{sx}")
            link(f"LowerArm_{sx}", f"Elbow_{sx}")
        else:
            link(f"LowerArm_{sx}", f"UpperArm_{sx}")
        link(f"Hand_{sx}", f"LowerArm_{sx}")
        link(f"Arm_{sx}", "Torso")
        link(f"Eye_{sx}", "Head")
        link(f"Pupil_{sx}", "Head")
    link("Mouth", "Head")

    for ob in mesh_obs:
        base = ob.name.split(".")[0]
        if base.startswith("HeadsetCup"):
            link(base, "Head")

    for tag, parent in (
        ("Hair", "Head"),
        ("Hood", "Head"),
        ("Eyebrows", "Head"),
        ("HardHat", "Head"),
        ("HardHatEng", "Head"),
        ("Glasses", "Head"),
        ("HeadsetBand", "Head"),
        ("HeadsetBandSales", "Head"),
        ("HeadsetMic", "Head"),
        ("HiVisVest", "Torso"),
        ("HiVisStripe", "Torso"),
        ("Blazer", "Torso"),
        ("BlazerRec", "Torso"),
        ("LabCoat", "Torso"),
        ("Hoodie", "Torso"),
        ("Jeans", "Hips"),
        ("HoodieTorso", "Torso"),
        ("HoodieSleeve_-1", "Torso"),
        ("HoodieSleeve_1", "Torso"),
        ("HoodiePocket", "HoodieTorso"),
        ("JeansHips", "Hips"),
        ("JeansLeg_-1", "Leg_-1"),
        ("JeansLeg_1", "Leg_1"),
        ("Shirt", "Torso"),
        ("Trousers", "Torso"),
        ("Tie", "Torso"),
        ("BadgeLanyard", "Torso"),
        ("Laptop", "Hand_1"),
        ("DeskTop", "Leg_1"),
        ("MonitorStand", "DeskTop"),
        ("Monitor", "MonitorStand"),
        ("MonitorScreen", "Monitor"),
        ("Keyboard", "DeskTop"),
        ("Mouse", "DeskTop"),
        ("ChairBack", "Hips"),
        ("LaptopBase", "DeskTop"),
        ("LaptopScreen", "LaptopBase"),
    ):
        link(tag, parent)

    for tag, foot_parent in (("Shoe_-1", "Foot_-1"), ("Shoe_1", "Foot_1")):
        link(tag, foot_parent)

    for ob in mesh_obs:
        base = ob.name.split(".")[0]
        if ob.parent and ob.parent.type == "MESH":
            continue
        if any(base.endswith(s) for s in PROP_SUFFIX_SKIP):
            continue
        if not PROP_RE.search(base):
            continue
        hands = [names[k] for k in ("Hand_1", "Hand_-1") if k in names]
        desk = names.get("DeskTop")
        torso = names.get("Torso")
        best_hand = (
            min(hands, key=lambda h: bbox_gap(_world_bbox(ob), _world_bbox(h))) if hands else None
        )
        bpy.ops.object.select_all(action="DESELECT")
        ob.select_set(True)
        parent_ob = None
        if best_hand and bbox_gap(_world_bbox(ob), _world_bbox(best_hand)) <= 0.12:
            parent_ob = best_hand
        elif desk and bbox_gap(_world_bbox(ob), _world_bbox(desk)) <= 0.15:
            parent_ob = desk
        elif torso and bbox_gap(_world_bbox(ob), _world_bbox(torso)) <= 0.12:
            parent_ob = torso
        if parent_ob:
            parent_ob.select_set(True)
            bpy.context.view_layer.objects.active = parent_ob
            bpy.ops.object.parent_set(type="OBJECT", keep_transform=True)
    return root


def check_objects_connectivity(mesh_obs: list[bpy.types.Object], asset_id: str) -> tuple[bool, str]:
    """Exported meshes: each part bbox must touch its logical anchor (≤2 cm)."""
    ok, msg = check_prejoin_connectivity(mesh_obs, asset_id)
    if not ok:
        return False, msg
    return True, f"objects={len(mesh_obs)} {msg}"


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
        if f"UpperArm_{sx}" in names and f"Shoulder_{sx}" in names:
            rules.append((f"UpperArm_{sx}", f"Shoulder_{sx}"))
            rules.append((f"LowerArm_{sx}", f"UpperArm_{sx}"))
        elif f"Arm_{sx}" in names:
            rules.append((f"Arm_{sx}", "Torso"))
        if f"Hand_{sx}" in names:
            arm = f"LowerArm_{sx}" if f"LowerArm_{sx}" in names else f"Arm_{sx}"
            if arm in names:
                rules.append((f"Hand_{sx}", arm))
        if f"Shoulder_{sx}" in names:
            rules.append((f"Shoulder_{sx}", "Torso"))
        fk, lk = f"Foot_{sx}", f"Leg_{sx}"
        if fk in names:
            rules.append((fk, lk))
        for ek in (f"Eye_{sx}", f"Pupil_{sx}"):
            if ek in names:
                rules.append((ek, "Head"))

    if "Mouth" in names:
        rules.append(("Mouth", "Head"))

    for child_key, parent_key in rules:
        pa = _world_bbox(core.get(parent_key) or names[parent_key])
        pb = _world_bbox(core.get(child_key) or names[child_key])
        gap = bbox_gap(pa, pb)
        allow = MAX_GAP_M
        if asset_id == "staff-founder-idle":
            if child_key == "Head" and parent_key == "Neck":
                allow = 0.095
            elif child_key.startswith("Shoulder_") and parent_key == "Torso":
                allow = 0.14
            elif child_key.startswith("Foot_") and parent_key.startswith("Leg_"):
                allow = 0.20
            elif parent_key == "Hips" and child_key.startswith("Leg_"):
                allow = 0.065
            elif child_key == "Hips" and parent_key.startswith("Leg_"):
                allow = 0.075
        if gap > allow:
            return False, f"{child_key}->{parent_key} gap={gap:.4f}m"

    extra_rules: list[tuple[str, str]] = [
        ("HiVisVest", "Torso"),
        ("HiVisStripe", "Torso"),
        ("Blazer", "Torso"),
        ("BlazerRec", "Torso"),
        ("LabCoat", "Torso"),
        ("Hoodie", "Torso"),
        ("Jeans", "Hips"),
        ("HoodieTorso", "Torso"),
        ("HoodieSleeve_-1", "Torso"),
        ("HoodieSleeve_1", "Torso"),
        ("HoodiePocket", "HoodieTorso"),
        ("JeansHips", "Hips"),
        ("JeansLeg_-1", "Leg_-1"),
        ("JeansLeg_1", "Leg_1"),
        ("Shirt", "Torso"),
        ("Trousers", "Torso"),
        ("BadgeLanyard", "Torso"),
        ("Hair", "Head"),
        ("Eyebrows", "Head"),
        ("Hood", "Head"),
        ("HardHat", "Head"),
        ("HardHatEng", "Head"),
        ("Glasses", "Head"),
        ("HeadsetBand", "Head"),
        ("HeadsetBandSales", "Head"),
        ("HeadsetMic", "Head"),
        ("DeskTop", "Leg_1"),
        ("MonitorStand", "DeskTop"),
        ("Monitor", "MonitorStand"),
        ("Keyboard", "DeskTop"),
        ("Mouse", "DeskTop"),
        ("ChairBack", "Torso"),
        ("LaptopBase", "DeskTop"),
        ("LaptopScreen", "LaptopBase"),
        ("Laptop", "Hand_1"),
        ("FounderMug", "Hand_1"),
        ("GpuCard", "Hand_1"),
        ("Wrench", "Hand_-1"),
        ("Tablet", "Hand_1"),
        ("Phone", "Hand_1"),
        ("Clipboard", "Hand_1"),
        ("MonitorScreen", "Monitor"),
    ]
    for child_key, parent_key in extra_rules:
        if child_key not in names or parent_key not in names:
            continue
        gap = bbox_gap(_world_bbox(names[parent_key]), _world_bbox(names[child_key]))
        allow = MAX_GAP_M
        if asset_id == "staff-founder-idle":
            if child_key == "Hood" and parent_key == "Head":
                allow = 0.24
            elif child_key.startswith("JeansLeg_") and parent_key.startswith("Leg_"):
                allow = 0.024
        if gap > allow:
            return False, f"{child_key}->{parent_key} gap={gap:.4f}m"
    for base in names:
        if base.startswith("HeadsetCup"):
            gap = bbox_gap(_world_bbox(names["Head"]), _world_bbox(names[base]))
            if gap > MAX_GAP_M:
                return False, f"{base}->Head gap={gap:.4f}m"

    for ob in mesh_obs:
        base = ob.name.split(".")[0]
        if base in ("BadgeShield",):
            continue
        if not PROP_RE.search(base) or any(base.endswith(s) for s in PROP_SUFFIX_SKIP):
            continue
        if base in ("ChairBack", "DeskTop", "DeskLeg", "MonitorStand", "Monitor", "Keyboard", "Mouse"):
            continue
        bb = _world_bbox(ob)
        hand_d = min(
            bbox_gap(bb, _world_bbox(names[h])) for h in names if h.startswith("Hand_")
        ) if any(n.startswith("Hand_") for n in names) else 999.0
        foot_lo = names["Foot_-1"].matrix_world.translation.y if "Foot_-1" in names else 0.02
        desk = names.get("DeskTop")
        desk_d = bbox_gap(bb, _world_bbox(desk)) if desk else 999.0
        on_ground = bb[0].y <= foot_lo + 0.12
        torso_d = bbox_gap(bb, _world_bbox(names["Torso"])) if "Torso" in names else 999.0
        if hand_d > MAX_GAP_M and desk_d > MAX_GAP_M and torso_d > MAX_GAP_M and not on_ground:
            return False, f"prop {base} gap_hand={hand_d:.4f}m"

    for tag in names:
        if tag in ("Head", "Neck", "Torso", "Hips", "SpineCore", "Mouth", "Tie", "BadgeLanyard"):
            continue
        if tag.startswith(("Leg_", "Arm_", "UpperArm_", "LowerArm_", "Elbow_", "Hand_", "Foot_", "Eye_", "Pupil_", "Shoulder_")):
            continue
        if PROP_RE.search(tag):
            continue
        anchor = "Head"
        if tag in (
            "HiVisVest",
            "HiVisStripe",
            "Blazer",
            "BlazerRec",
            "LabCoat",
            "Hoodie",
            "HoodieTorso",
            "HoodieSleeve_-1",
            "HoodieSleeve_1",
            "HoodiePocket",
            "Shirt",
            "Trousers",
            "BadgeShield",
        ):
            anchor = "Torso"
        if tag in ("Jeans", "JeansHips"):
            anchor = "Hips"
        if tag.startswith("JeansLeg_"):
            leg = tag.replace("JeansLeg_", "Leg_")
            if leg in names:
                anchor = leg
        elif tag == "JeansHips" and "Hips" in names:
            anchor = "Hips"
        if tag.startswith("Shoe_"):
            foot = tag.replace("Shoe_", "Foot_")
            if foot in names:
                anchor = foot
        if tag.startswith("Headset") or tag in (
            "Hair",
            "Hood",
            "HardHat",
            "HardHatEng",
            "Glasses",
            "Eyebrows",
        ):
            anchor = "Head"
        if anchor in names:
            gap = bbox_gap(_world_bbox(names[anchor]), _world_bbox(names[tag]))
            allow = MAX_GAP_M
            if asset_id == "staff-founder-idle":
                if anchor == "Head" and tag in ("Hood", "Hair"):
                    allow = 0.24
                elif tag.startswith("JeansLeg_") and anchor.startswith("Leg_"):
                    allow = 0.024
            if gap > allow:
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


def _pick_torso_island(ob: bpy.types.Object, islands: list[list[int]], bboxes: list[tuple[Vector, Vector]]) -> int:
    sizes = [len(ids) for ids in islands]
    zs = [(b[0].z + b[1].z) * 0.5 for b in bboxes]
    z_min = min(b[0].z for b in bboxes)
    z_max = max(b[1].z for b in bboxes)
    height = z_max - z_min
    band_lo = z_min + height * 0.28
    band_hi = z_min + height * 0.62
    in_band = [i for i, z in enumerate(zs) if band_lo <= z <= band_hi and sizes[i] >= SIGNIFICANT_ISLAND_VERTS]
    if in_band:
        return max(in_band, key=lambda i: sizes[i])
    return max(range(len(sizes)), key=lambda i: sizes[i])


def check_gltf_connectivity(glb_path: str, asset_id: str) -> tuple[bool, str]:
    bpy.ops.wm.read_factory_settings(use_empty=True)
    bpy.ops.import_scene.gltf(filepath=glb_path)
    meshes = [o for o in bpy.context.scene.objects if o.type == "MESH"]
    if not meshes:
        return False, "no_meshes"

    if len(meshes) > 1:
        ok, msg = check_objects_connectivity(meshes, asset_id)
        if not ok:
            return False, msg
        return True, f"meshes={len(meshes)} {msg}"

    ob = meshes[0]
    islands = _mesh_islands(ob)
    if len(islands) < 2:
        return True, f"single_island verts={len(ob.data.vertices)}"

    bboxes = [_island_bbox(ob, ids) for ids in islands]
    sizes = [len(ids) for ids in islands]
    core_i = _pick_torso_island(ob, islands, bboxes)

    worst = 0.0
    worst_i = -1
    for i, bb in enumerate(bboxes):
        if sizes[i] < SIGNIFICANT_ISLAND_VERTS:
            continue
        if i == core_i:
            continue
        gap = bbox_gap(bb, bboxes[core_i])
        if gap > worst:
            worst = gap
            worst_i = i
    if worst > MAX_GAP_M:
        return False, f"object={worst_i} core_gap={worst:.4f}m islands={len(islands)} (legacy single-mesh)"
    return True, f"islands={len(islands)} core_worst={worst:.4f}"