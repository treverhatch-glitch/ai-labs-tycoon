#!/usr/bin/env python3
"""Packet 3–4 env GLB gates on committed files (no Blender bpy authoring checks)."""
from __future__ import annotations

import json
import struct
import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
GLB_DIR = REPO / "unity/Assets/Art/EarlyGame/garage-kit"

ENV_IDS = (
    "garage-exterior-wall",
    "garage-land-for-sale-sign",
    "garage-lot-curb",
    "garage-window-frame",
)

PARTS_MIN = {
    "garage-exterior-wall": 12,
    "garage-lot-curb": 1,
    "garage-land-for-sale-sign": 8,
    "garage-window-frame": 9,
}

REQUIRED_IMAGE = {
    "garage-exterior-wall": ("tex-garage-wall-beige",),
    "garage-land-for-sale-sign": (),
    "garage-lot-curb": (),
    "garage-window-frame": (),
}

PRIMARY_MATERIAL = {
    "garage-exterior-wall": "CinderblockWall",
    "garage-lot-curb": "ConcreteCurb",
    "garage-window-frame": "WindowFrame",
    "garage-land-for-sale-sign": "SignBoard",
}


def glb_json(path: Path) -> dict:
    data = path.read_bytes()
    if data[:4] != b"glTF":
        raise ValueError(f"{path.name}: not GLB")
    _version, _length = struct.unpack_from("<II", data, 4)
    chunk_len, chunk_type = struct.unpack_from("<I4s", data, 12)
    if chunk_type != b"JSON":
        raise ValueError(f"{path.name}: missing JSON chunk")
    return json.loads(data[20 : 20 + chunk_len])


def _extras_part_count(gltf: dict) -> int | None:
    for node in gltf.get("nodes", []):
        extras = node.get("extras") or {}
        if "packet34_part_count" in extras:
            return int(extras["packet34_part_count"])
    for mesh in gltf.get("meshes", []):
        extras = mesh.get("extras") or {}
        if "packet34_part_count" in extras:
            return int(extras["packet34_part_count"])
    return None


def _material_names(gltf: dict) -> list[str]:
    return [str(m.get("name") or "") for m in gltf.get("materials", [])]


def _image_names(gltf: dict) -> list[str]:
    names: list[str] = []
    for img in gltf.get("images", []):
        name = str(img.get("name") or img.get("uri") or "")
        names.append(name)
    return names


def check_env_glb(asset_id: str) -> list[str]:
    glb = GLB_DIR / f"{asset_id}.glb"
    errs: list[str] = []
    if not glb.is_file():
        return [f"missing_glb"]
    try:
        gltf = glb_json(glb)
    except ValueError as exc:
        return [str(exc)]

    mats = _material_names(gltf)
    imgs = _image_names(gltf)
    parts = _extras_part_count(gltf)
    min_parts = PARTS_MIN.get(asset_id, 3)

    if parts is None:
        errs.append("missing_glb_extras_packet34_part_count")
    elif parts < min_parts:
        errs.append(f"glb_part_count={parts} need>={min_parts}")

    if not imgs:
        errs.append("glb_no_embedded_images (vcol-only export)")
    if any("_vcol" in m for m in mats):
        errs.append(f"glb_vertex_color_material {mats!r}")

    want_mat = PRIMARY_MATERIAL.get(asset_id)

    def _mat_present(want: str) -> bool:
        return any(m == want or m.startswith(f"{want}.") for m in mats)

    if want_mat and not _mat_present(want_mat):
        errs.append(f"missing_material {want_mat}")

    for sub in REQUIRED_IMAGE.get(asset_id, ()):
        if not any(sub in n for n in imgs):
            errs.append(f"missing_image_ref {sub}")

    mesh_count = len(gltf.get("meshes", []))
    mesh_nodes = sum(1 for n in gltf.get("nodes", []) if n.get("mesh") is not None)
    if mesh_nodes != 1:
        errs.append(f"glb_mesh_nodes={mesh_nodes} need 1")
    if mesh_count != 1:
        errs.append(f"glb_meshes={mesh_count} need 1")

    errs.extend(check_glb_proportions(asset_id, gltf))

    return errs


def _mesh_position_bounds(gltf: dict) -> tuple[list[float], list[float]] | None:
    accs = gltf.get("accessors", [])
    meshes = gltf.get("meshes", [])
    if not accs or not meshes:
        return None
    prim = meshes[0].get("primitives", [{}])[0]
    pos_idx = prim.get("attributes", {}).get("POSITION")
    if pos_idx is None:
        return None
    acc = accs[pos_idx]
    mn = acc.get("min")
    mx = acc.get("max")
    if not mn or not mx or len(mn) < 3 or len(mx) < 3:
        return None
    return mn, mx


def check_glb_proportions(asset_id: str, gltf: dict) -> list[str]:
    """Reject tall backdrop slabs embedded in env GLBs (curb especially)."""
    bounds = _mesh_position_bounds(gltf)
    if not bounds:
        return []
    mn, mx = bounds
    ex = sorted(mx[i] - mn[i] for i in range(3))
    height, mid, long_axis = ex[0], ex[1], ex[2]
    errs: list[str] = []
    if asset_id == "garage-lot-curb":
        if long_axis < 0.75 or long_axis > 1.35:
            errs.append(f"curb_length={long_axis:.2f} need~1.0m")
        if height > 0.22:
            errs.append(f"curb_too_tall h={height:.2f} need~0.15")
        if height > mid * 0.85:
            errs.append(f"curb_backdrop_slab h={height:.2f} mid={mid:.2f}")
    elif asset_id in {"garage-exterior-wall", "garage-window-frame", "garage-land-for-sale-sign"}:
        if height > long_axis * 1.85 and asset_id != "garage-exterior-wall":
            errs.append(f"vertical_slab_glb h={height:.2f} long={long_axis:.2f}")
    return errs


def glb_manifest_summary(asset_id: str) -> dict | None:
    """Part count + material names from committed GLB (for PR body)."""
    glb = GLB_DIR / f"{asset_id}.glb"
    if not glb.is_file():
        return None
    try:
        gltf = glb_json(glb)
    except ValueError:
        return None
    parts = _extras_part_count(gltf)
    mats = [m for m in _material_names(gltf) if m and "_vcol" not in m]
    imgs = _image_names(gltf)
    return {"parts": parts, "materials": mats, "images": imgs}


def check_all_env_glbs() -> int:
    rc = 0
    for asset_id in ENV_IDS:
        errs = check_env_glb(asset_id)
        if errs:
            for err in errs:
                print(f"FAIL {asset_id} glb {err}")
            rc = 1
        else:
            info = glb_manifest_summary(asset_id)
            print(f"OK {asset_id} glb parts={info and info.get('parts')} mats={info and info.get('materials')}")
    return rc


def main() -> int:
    return check_all_env_glbs()


if __name__ == "__main__":
    sys.exit(main())