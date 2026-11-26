#!/usr/bin/env python3
"""Import Poly Haven CC0 GLTF, restyle, bake, export one garage hero asset."""
from __future__ import annotations

import json
import sys
from pathlib import Path

import bpy
from mathutils import Vector

REPO = Path(__file__).resolve().parents[3]
MAP = REPO / "tools/art/cc0/garage_cc0_map.json"
INCOMING = REPO / "third_party/cc0/polyhaven"


def _world_span(objs: list[bpy.types.Object]) -> float:
    mins = Vector((1e9, 1e9, 1e9))
    maxs = Vector((-1e9, -1e9, -1e9))
    for ob in objs:
        if ob.type != "MESH":
            continue
        for corner in ob.bound_box:
            w = ob.matrix_world @ Vector(corner)
            mins.x = min(mins.x, w.x)
            mins.y = min(mins.y, w.y)
            mins.z = min(mins.z, w.z)
            maxs.x = max(maxs.x, w.x)
            maxs.y = max(maxs.y, w.y)
            maxs.z = max(maxs.z, w.z)
    return max((maxs - mins).x, (maxs - mins).y, (maxs - mins).z, 1e-6)


def _normalize_scale(mesh_obs: list[bpy.types.Object], target: float, mul: float = 1.0) -> None:
    span = _world_span(mesh_obs)
    factor = (target * mul) / span
    if abs(factor - 1.0) < 1e-4:
        return
    bpy.ops.object.select_all(action="DESELECT")
    for ob in mesh_obs:
        ob.select_set(True)
    bpy.context.view_layer.objects.active = mesh_obs[0]
    bpy.ops.transform.resize(value=(factor, factor, factor))
    bpy.ops.object.transform_apply(scale=True)


def _gltf_path(polyhaven_id: str) -> Path:
    pack = INCOMING / polyhaven_id
    if not pack.is_dir():
        raise FileNotFoundError(f"missing pack {pack} — run fetch_polyhaven_garage.py")
    entry = pack / "entry.gltf"
    if entry.exists():
        return pack / entry.read_text(encoding="utf-8").strip()
    gltfs = sorted(pack.glob("*.gltf"))
    if not gltfs:
        raise FileNotFoundError(f"no .gltf in {pack}")
    return gltfs[0]


def _extras_for_desk() -> None:
    import blender_common as bc  # type: ignore

    def cb(n, w, h, d, x, y, z):
        return bc.add_cube(n, (w, d, h), (x, z, y))

    cb("Monitor", 0.42, 0.32, 0.06, 0.22, 0.05, 0.92)
    cb("Bezel", 0.46, 0.36, 0.08, 0.22, 0.03, 0.88)
    led = cb("Screen", 0.36, 0.26, 0.02, 0.22, 0.09, 0.92)
    bc.assign_emission(led, (0.12, 0.72, 0.45, 1.0), 2.4)


def build_cc0_garage(asset_id: str) -> dict:
    import blender_common as bc  # type: ignore

    bc.reset_scene()
    row = next(r for r in json.loads(MAP.read_text(encoding="utf-8"))["assets"] if r["id"] == asset_id)
    gltf = _gltf_path(row["polyhavenId"])
    bpy.ops.object.select_all(action="SELECT")
    bpy.ops.object.delete()
    bpy.ops.import_scene.gltf(filepath=str(gltf))
    mesh_obs = [o for o in bpy.context.scene.objects if o.type == "MESH"]
    if not mesh_obs:
        raise RuntimeError(f"{asset_id}: import produced no meshes")
    if len(mesh_obs) > 8:
        bpy.ops.object.select_all(action="DESELECT")
        for ob in mesh_obs:
            ob.select_set(True)
        bpy.context.view_layer.objects.active = mesh_obs[0]
        bpy.ops.object.join()
        mesh_obs = [bpy.context.view_layer.objects.active]
    _normalize_scale(mesh_obs, float(row.get("targetMaxDim", 1.0)), float(row.get("scaleMul", 1.0)))
    mesh_obs = [o for o in bpy.context.scene.objects if o.type == "MESH"]
    if asset_id == "garage-desk-crt":
        _extras_for_desk()
    entry = bc.finish_asset(
        asset_id,
        "garage-kit",
        license_note=f"CC0-1.0 (Poly Haven: {row['polyhavenId']}, restyled in-repo)",
    )
    entry["source"] = "cc0-polyhaven"
    entry["sourceId"] = row["polyhavenId"]
    return entry


def main() -> None:
    import blender_common as bc  # type: ignore

    if len(sys.argv) < 2:
        print("usage: cc0_garage_asset.py <garage-id|ALL>", file=sys.stderr)
        raise SystemExit(2)
    arg = sys.argv[-1]
    ids = [a["id"] for a in json.loads(MAP.read_text(encoding="utf-8"))["assets"]]
    targets = ids if arg == "ALL" else [arg]
    entries: list[dict] = []
    for asset_id in targets:
        if asset_id not in ids:
            raise SystemExit(f"unknown id {asset_id}")
        entries.append(build_cc0_garage(asset_id))
        print("OK", asset_id, "cc0")
    if arg == "ALL":
        manifest = REPO / "unity/Assets/Art/EarlyGame/garage-manifest.json"
        payload = json.loads(manifest.read_text(encoding="utf-8"))
        by_id = {e["id"]: e for e in entries}
        for i, a in enumerate(payload["assets"]):
            if a["id"] in by_id:
                payload["assets"][i] = {**a, **by_id[a["id"]]}
        payload["fidelity"] = "cc0-import-restyle"
        manifest.write_text(json.dumps(payload, indent=2) + "\n", encoding="utf-8")


if __name__ == "__main__":
    sys.path.insert(0, str(Path(__file__).resolve().parent))
    import blender_common as bc  # noqa: E402

    def _job():
        main()
        return None

    bc.run_script(_job)