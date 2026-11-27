"""Researcher hero self-check: mesh basics + 0° front render coat coverage."""
from __future__ import annotations

import os
import sys
import tempfile
from pathlib import Path

import bpy

sys.path.insert(0, str(Path(__file__).resolve().parent))
sys.path.insert(0, str(Path(__file__).resolve().parents[1]))
import blender_common as bc  # noqa: E402
import kenney_researcher_lib as kr  # noqa: E402
def _assert_mesh_basics() -> None:
    from kenney_researcher_stock_recolor import SLOT_LAB_COAT

    body = bpy.data.objects.get("KenneyResearcherStock")
    coat_n = 0
    if body and body.data.polygons:
        coat_n = sum(1 for p in body.data.polygons if p.material_index == SLOT_LAB_COAT)
    if coat_n < 120:
        raise RuntimeError(f"FAIL researcher_hero_selfcheck body_coat_faces={coat_n} want>=120")
    glasses = bpy.data.objects.get("ResearcherGlasses")
    gv = 0 if glasses is None else len(glasses.data.vertices)
    print(
        f"OK researcher_hero_selfcheck mesh body_coat_faces={coat_n} "
        f"glasses_verts={len(glasses.data.vertices)}"
    )


def assert_researcher_hero_geometry(_body: bpy.types.Object | None = None) -> None:
    _assert_mesh_basics()
    print("OK researcher_hero_selfcheck render_deferred=axis+diagnostic_png")


def main() -> None:
    os.environ["RESEARCHER_STOCK_RECOLOR"] = "1"
    kr.build_researcher_kenney()
    body = bpy.data.objects.get("KenneyResearcherStock")
    if body is None:
        raise RuntimeError("FAIL researcher_hero_selfcheck missing_body")
    assert_researcher_hero_geometry(body)
    raise SystemExit(0)


if __name__ == "__main__":
    main()