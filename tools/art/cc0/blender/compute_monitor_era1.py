"""CC0 hero: compute-monitor-era1 — chunky beige CRT."""
import json
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import cc0_common as cc  # noqa: E402
import monitor_era1_kit as kit  # noqa: E402


def build() -> dict:
    cc.reset_scene()
    root = kit.build_monitor_era1()
    sources = [
        "procedural:era1 chunky beige CRT 0.40x0.40x0.38",
        "style: curved screen, swivel base, power+LED",
    ]
    out = cc.finish_cc0_asset("compute-monitor-era1", root, sources)
    out["generator"] = "tools/art/cc0/blender/compute_monitor_era1.py"
    return out


if __name__ == "__main__":
    print(json.dumps(build(), indent=2))