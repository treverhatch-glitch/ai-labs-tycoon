#!/usr/bin/env python3
"""Prove committed 85c48f17 sign GLB fails board/text visibility gates (Hatch FAIL)."""
from __future__ import annotations

import subprocess
import sys
import tempfile
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
SHA = "85c48f17"
GLB_REL = "unity/Assets/Art/EarlyGame/garage-kit/garage-land-for-sale-sign.glb"

BLENDER_PROVE = """
import json
import sys
from pathlib import Path
import bpy

path = Path(sys.argv[-1])
bpy.ops.wm.read_factory_settings(use_empty=True)
bpy.ops.import_scene.gltf(filepath=str(path))
meshes = [o for o in bpy.context.scene.objects if o.type == "MESH"]
if not meshes:
    print(json.dumps({"error": "no_mesh"}))
    sys.exit(0)
ob = meshes[0]
red = sum(1 for p in ob.data.polygons if p.material_index == 1)
white = sum(1 for p in ob.data.polygons if p.material_index == 0)
dims = tuple(round(x, 3) for x in ob.dimensions)
mats = [m.name if m else "" for m in ob.data.materials]
# Board-ish shell: faces with |normal.z|>0.7 and center y in upper band
import bmesh
bm = bmesh.new()
bm.from_mesh(ob.data)
bm.faces.ensure_lookup_table()
ys = [v.co.y for v in bm.verts]
y0, y1 = min(ys), max(ys)
board_band = y0 + (y1 - y0) * 0.55
board_faces = 0
for f in bm.faces:
    c = f.calc_center_median()
    if c.y >= board_band and abs(f.normal.z) > 0.65:
        board_faces += 1
bm.free()
out = {
    "dims": dims,
    "materials": mats,
    "red_faces": red,
    "white_faces": white,
    "board_normal_faces": board_faces,
    "red_ratio": round(red / max(1, red + white), 4),
}
print(json.dumps(out))
"""


def main() -> int:
    glb = REPO / GLB_REL
    with tempfile.NamedTemporaryFile(suffix=".glb", delete=False) as tmp:
        tmp_path = Path(tmp.name)
    data = subprocess.run(
        ["git", "show", f"{SHA}:{GLB_REL}"],
        cwd=REPO,
        check=True,
        capture_output=True,
    ).stdout
    tmp_path.write_bytes(data)
    proc = subprocess.run(
        [
            "blender",
            "-b",
            "--python-expr",
            BLENDER_PROVE,
            "--",
            str(tmp_path),
        ],
        text=True,
        capture_output=True,
    )
    tmp_path.unlink(missing_ok=True)
    for line in proc.stdout.splitlines():
        line = line.strip()
        if line.startswith("{"):
            print(f"PROVE {SHA} {line}")
            info = __import__("json").loads(line)
            fail = []
            if info.get("board_normal_faces", 0) < 120:
                fail.append("no_board_face_shell")
            if info.get("red_faces", 0) > info.get("white_faces", 1) * 0.35:
                fail.append("red_tags_swamp_posts")
            if info.get("red_ratio", 0) > 0.22:
                fail.append(f"red_ratio={info.get('red_ratio')}")
            if fail:
                print(f"FAIL {SHA} " + " ".join(fail))
                return 0
            print(f"WARN {SHA} expected failure markers missing")
            return 1
    print("FAIL prove script no json output", file=sys.stderr)
    return 1


if __name__ == "__main__":
    sys.exit(main())