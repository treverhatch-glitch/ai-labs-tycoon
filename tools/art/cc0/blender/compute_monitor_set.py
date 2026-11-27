"""CC0 hero: monitor + keyboard + mouse (Kenney Furniture); larger screen frame."""
import json
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import cc0_common as cc  # noqa: E402
import mesh_builders as mb  # noqa: E402


def build() -> dict:
    cc.reset_scene()
    pieces = []
    sources = []
    frame_scale = 1.68
    layout = [
        (cc.furniture_obj("computerScreen"), (0, 0, 0.32), (frame_scale, frame_scale, frame_scale)),
        (cc.furniture_obj("computerKeyboard"), (0, 0.38, 0.02), (1.05, 1.05, 1.05)),
        (cc.furniture_obj("computerMouse"), (0.32, 0.38, 0.02), (1.05, 1.05, 1.05)),
    ]
    for path, loc, scale in layout:
        for ob in cc.import_obj(path):
            ob.location = loc
            ob.scale = scale
            cc.apply_transforms(ob)
            pieces.append(ob)
        sources.append(str(path.relative_to(cc.REPO)))
    root = cc.join_meshes(pieces)
    mb.snap_root_to_floor(root)
    return cc.finish_cc0_asset("compute-monitor-set", root, sources)


if __name__ == "__main__":
    print(json.dumps(build(), indent=2))