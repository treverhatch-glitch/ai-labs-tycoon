"""CC0 hero: PC tower from Quaternius Modular Sci-Fi MegaKit."""
import json
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import cc0_common as cc  # noqa: E402


def build() -> dict:
    cc.reset_scene()
    path = cc.quaternius_obj("Prop_Computer")
    meshes = []
    for ob in cc.import_obj(path):
        ob.scale = (1.1, 1.1, 1.1)
        cc.apply_transforms(ob)
        meshes.append(ob)
    if not meshes:
        raise RuntimeError(f"no mesh from {path}")
    root = cc.join_meshes(meshes)
    sources = [str(path.relative_to(cc.REPO))]
    return cc.finish_cc0_asset("compute-pc-tower", root, sources)


if __name__ == "__main__":
    print(json.dumps(build(), indent=2))