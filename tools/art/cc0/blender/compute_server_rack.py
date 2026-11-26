"""CC0 hero: server rack from Kenney Factory Kit."""
import json
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import cc0_common as cc  # noqa: E402


def build() -> dict:
    cc.reset_scene()
    pieces = []
    specs = [
        (cc.factory_glb("structure-tall"), (0, 0, 0.9), (1.0, 1.0, 1.0)),
        (cc.factory_glb("structure-wall"), (0.0, 0.62, 0.9), (0.24, 1.0, 1.8)),
        (cc.factory_glb("structure-wall"), (0.0, -0.62, 0.9), (0.24, 1.0, 1.8)),
        (cc.factory_glb("machine"), (0.0, 0.0, 0.45), (0.85, 0.85, 0.85)),
        (cc.factory_glb("machine-window"), (0.0, 0.28, 1.1), (0.7, 0.7, 0.7)),
        (cc.factory_glb("machine-window"), (0.0, -0.28, 1.1), (0.7, 0.7, 0.7)),
        (cc.factory_glb("screen-panel-wide"), (0.18, 0.0, 1.55), (0.55, 0.55, 0.55)),
        (cc.factory_glb("screen-panel-small"), (0.18, 0.38, 0.35), (0.5, 0.5, 0.5)),
        (cc.factory_glb("screen-panel-small"), (0.18, -0.38, 0.35), (0.5, 0.5, 0.5)),
    ]
    sources = []
    for path, loc, scale in specs:
        for ob in cc.import_glb(path):
            ob.location = loc
            ob.scale = scale
            cc.apply_transforms(ob)
            pieces.append(ob)
        sources.append(str(path.relative_to(cc.REPO)))
    root = cc.join_meshes(pieces)
    return cc.finish_cc0_asset("compute-server-rack", root, sources)


if __name__ == "__main__":
    print(json.dumps(build(), indent=2))