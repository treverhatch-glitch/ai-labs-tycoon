"""Re-run founder chunky gates on exported GLB."""
from __future__ import annotations

import sys
from pathlib import Path

import bpy

sys.path.insert(0, str(Path(__file__).resolve().parent))
import founder_chunky_gates as fcg  # noqa: E402


def main() -> None:
    glb = Path(sys.argv[-1])
    bpy.ops.wm.read_factory_settings(use_empty=True)
    bpy.ops.import_scene.gltf(filepath=str(glb))
    meshes = [o for o in bpy.context.scene.objects if o.type == "MESH"]
    stock = next((o for o in meshes if o.name.split(".")[0] == "KenneyFounderStock"), None)
    if stock and len(meshes) in (1, 3):
        import cc0_staff_lib as staff_c0

        ok, msg = staff_c0.check_founder_stock_single_mesh(stock)
        if ok:
            msg = f"stock_recolor {msg}"
    else:
        ok, msg = fcg.check_founder_chunky_bake(meshes)
    print(f"{'OK' if ok else 'FAIL'} staff-founder-idle {msg}")
    raise SystemExit(0 if ok else 1)


if __name__ == "__main__":
    main()