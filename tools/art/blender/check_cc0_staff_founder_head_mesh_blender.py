"""Import founder GLB — head bone mesh must have real geometry."""
from __future__ import annotations

import sys
from pathlib import Path

import bpy

sys.path.insert(0, str(Path(__file__).resolve().parent))
import cc0_staff_lib as c0  # noqa: E402


def main() -> None:
    glb = Path(sys.argv[-1])
    bpy.ops.wm.read_factory_settings(use_empty=True)
    bpy.ops.import_scene.gltf(filepath=str(glb))
    meshes = [o for o in bpy.context.scene.objects if o.type == "MESH"]
    ok, msg = c0.check_founder_head_geometry(meshes)
    print(f"{'OK' if ok else 'FAIL'} staff-founder-idle {msg}")
    raise SystemExit(0 if ok else 1)


if __name__ == "__main__":
    main()